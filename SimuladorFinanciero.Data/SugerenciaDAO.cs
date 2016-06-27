using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimuladorFinanciero.Entities;
using SimuladorFinanciero.Data.Interface;
using SimuladorFinanciero.Helpers;

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
            throw new NotImplementedException();
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
                               where (i.Fecha >= ConstantesHelpers.FechaDesde && i.Fecha <= ConstantesHelpers.FechaHasta)
                               select i).ToList();
            }
            else
            {
                if (Tipo.Trim().Length == 0 && Estado.Trim().Length == 0)
                {
                    Sugerencias = (from i in Context.Sugerencia
                                   where i.Fecha >= Desde && i.Fecha <= Hasta
                                   select i).ToList();
                }
                else
                {
                    if (Tipo.Trim().Length != 0 && Estado.Trim().Length == 0)
                    {
                        Sugerencias = (from i in Context.Sugerencia
                                       where (i.Fecha >= Desde && i.Fecha <= Hasta) && i.Tipo == Tipo
                                       select i).ToList();
                    }
                    else if (Tipo.Trim().Length == 0 && Estado.Trim().Length != 0)
                    {
                        Sugerencias = (from i in Context.Sugerencia
                                       where (i.Fecha >= Desde && i.Fecha <= Hasta) && i.Estado == Estado
                                       select i).ToList();
                    }
                    else
                    {
                        Sugerencias = (from i in Context.Sugerencia
                                       where (i.Fecha >= Desde && i.Fecha <= Hasta) && i.Tipo == Tipo && i.Estado == Estado
                                       select i).ToList();
                    }
                }
            }
            return Sugerencias;
        }
    }
}