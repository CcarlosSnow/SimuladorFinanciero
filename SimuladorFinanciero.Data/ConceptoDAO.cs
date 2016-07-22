using System;
using System.Collections.Generic;
using System.Linq;
using SimuladorFinanciero.Data.Interface;
using SimuladorFinanciero.Entities;
using EntityFramework.Extensions;

namespace SimuladorFinanciero.Data
{
    public class ConceptoDAO : IConcepto
    {
        SimuladorFinancieroEntities Context = new SimuladorFinancieroEntities();
        public bool Delete(Concepto entidad)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool Insert(Concepto entidad)
        {
            entidad.Estado = "0301";
            Context.Concepto.Add(entidad);
            return (Context.SaveChanges() != 0);
        }

        public Concepto Select(int id)
        {
            throw new NotImplementedException();
        }

        public IList<Concepto> SelectAll()
        {
            throw new NotImplementedException();
        }

        public bool Update(Concepto entidad)
        {
            throw new NotImplementedException();
        }

        public int GetIdConcepto(string Nombre)
        {
            var IdConcepto = from i in Context.Concepto
                             where i.Nombre == Nombre
                             select i.IdConcepto;
            return IdConcepto.First();
        }

        public bool DeleteAll()
        {
            var Conceptos = from i in Context.Concepto
                            select i;
            Conceptos.Delete();
            return (Context.SaveChanges() != 0);
        }
    }
}
