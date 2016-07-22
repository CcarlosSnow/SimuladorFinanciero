using System;
using System.Collections.Generic;
using System.Linq;
using SimuladorFinanciero.Entities;
using SimuladorFinanciero.Data.Interface;

namespace SimuladorFinanciero.Data
{
    public class ParametroDAO : IParametro
    {
        SimuladorFinancieroEntities Context = new SimuladorFinancieroEntities();
        public bool Delete(Parametro entidad)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool Insert(Parametro entidad)
        {
            throw new NotImplementedException();
        }

        public Parametro Select(int id)
        {
            throw new NotImplementedException();
        }

        public IList<Parametro> SelectAll()
        {
            throw new NotImplementedException();
        }

        public bool Update(Parametro entidad)
        {
            throw new NotImplementedException();
        }

        public string GetIdParametro(string Nombre)
        {
            var IdParametro = from i in Context.Parametro
                              where i.Nombre == Nombre
                              select i.IdParametro;
            return IdParametro.First();
        }

        public IList<Parametro> SelectByStart(string Start)
        {
            var Parametros = from i in Context.Parametro
                             where i.IdParametro.Substring(0, 2) == Start && i.IdParametro != Start + "00"
                             select i;
            return Parametros.ToList();
        }
    }
}
