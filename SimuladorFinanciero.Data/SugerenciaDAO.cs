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
            throw new NotImplementedException();
        }

        public IList<Sugerencia> SelectAll()
        {
            var Sugerencias = from i in Context.Sugerencia
                              select i;
            return Sugerencias.ToList();
        }

        public bool Update(Sugerencia entidad)
        {
            throw new NotImplementedException();
        }

        public IList<Sugerencia> SelectByFechaAndTipo(DateTime? Desde = null, DateTime? Hasta = null, string Tipo = "")
        {
            IList<Sugerencia> Sugerencias = null;
            if (Desde == null && Hasta == null && Tipo.Trim().Length == 0)
            {
                Sugerencias = (from i in Context.Sugerencia
                               where (i.Fecha >= ConstantesHelpers.FechaDesde && i.Fecha <= ConstantesHelpers.FechaHasta)
                               select i).ToList();
            }
            else
            {
                if (Tipo.Trim().Length == 0)
                {
                    Sugerencias = (from i in Context.Sugerencia
                                   where i.Fecha >= Desde && i.Fecha <= Hasta
                                   select i).ToList();
                }
                else
                {
                    Sugerencias = (from i in Context.Sugerencia
                                   where (i.Fecha >= Desde && i.Fecha <= Hasta) && i.Tipo == Tipo
                                   select i).ToList();
                }
            }
            return Sugerencias;
        }
    }
}