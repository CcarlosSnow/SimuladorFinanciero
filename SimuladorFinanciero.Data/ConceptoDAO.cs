using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimuladorFinanciero.Data.Interface;
using SimuladorFinanciero.Entities;

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
            Context.Concepto.Add(entidad);
            return (Context.SaveChanges() != 0);
        }

        public Concepto Select(string id)
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
    }
}
