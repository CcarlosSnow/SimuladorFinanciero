using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimuladorFinanciero.Entities;
using SimuladorFinanciero.Data.Interface;
using SimuladorFinanciero.Helpers;
using System.Data.Entity;

namespace SimuladorFinanciero.Data
{
    public class SugerenciaDAO : ISugerencia
    {
        SimuladorFinancieroEntities Context = new SimuladorFinancieroEntities();
        public bool Delete(Sugerencia entidad)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool Insert(Sugerencia entidad)
        {
            Context.Sugerencia.Add(entidad);
            return (Context.SaveChanges() != 0);
        }

        public Sugerencia Select(int id)
        {
            var Sugerencia = from i in Context.Sugerencia
                             where i.IdSugerencia == id
                             select i;
            return Sugerencia.SingleOrDefault();
        }

        public IList<Sugerencia> SelectAll()
        {
            var Sugerencias = from i in Context.Sugerencia
                              select i;
            return Sugerencias.ToList();
        }

        public bool Update(Sugerencia entidad)
        {
            Context.Sugerencia.Attach(entidad);
            Context.Entry(entidad).State = System.Data.Entity.EntityState.Modified;
            return (Context.SaveChanges() != 0);
        }

        public IList<Sugerencia> SelectByFechaAndTipo(DateTime? Desde = null, DateTime? Hasta = null, string Tipo = "", string Estado = "")
        {
            IList<Sugerencia> Sugerencias = null;
            if (Desde == null && Hasta == null && Tipo.Trim().Length == 0 && Estado.Trim().Length == 0)
            {
                Sugerencias = (from i in Context.Sugerencia
                               where (DbFunctions.TruncateTime(i.Fecha) >= ConstantesHelpers.FechaDesde && DbFunctions.TruncateTime(i.Fecha) <= ConstantesHelpers.FechaHasta)
                               orderby i.Fecha descending
                               select i).ToList();
            }
            else
            {
                
                if (Tipo.Trim().Length == 0 && Estado.Trim().Length == 0)
                {
                    Sugerencias = (from i in Context.Sugerencia
                                   where DbFunctions.TruncateTime(i.Fecha) >= DbFunctions.TruncateTime(Desde) && DbFunctions.TruncateTime(i.Fecha) <= DbFunctions.TruncateTime(Hasta)
                                   orderby i.Fecha descending
                                   select i).ToList();
                }
                else
                {
                    if (Tipo.Trim().Length != 0 && Estado.Trim().Length == 0)
                    {
                        Sugerencias = (from i in Context.Sugerencia
                                       where (DbFunctions.TruncateTime(i.Fecha) >= Desde && DbFunctions.TruncateTime(i.Fecha) <= Hasta) && i.Tipo == Tipo
                                       orderby i.Fecha descending
                                       select i).ToList();
                    }
                    else if (Tipo.Trim().Length == 0 && Estado.Trim().Length != 0)
                    {
                        Sugerencias = (from i in Context.Sugerencia
                                       where (DbFunctions.TruncateTime(i.Fecha) >= Desde && DbFunctions.TruncateTime(i.Fecha) <= Hasta) && i.Estado == Estado
                                       orderby i.Fecha descending
                                       select i).ToList();
                    }
                    else
                    {
                        Sugerencias = (from i in Context.Sugerencia
                                       where (DbFunctions.TruncateTime(i.Fecha) >= Desde && DbFunctions.TruncateTime(i.Fecha) <= Hasta) && i.Tipo == Tipo && i.Estado == Estado
                                       orderby i.Fecha descending
                                       select i).ToList();
                    }
                }
            }
            return Sugerencias;
        }
    }
}