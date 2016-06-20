using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimuladorFinanciero.Data.Interface;
using SimuladorFinanciero.Entities;
using EntityFramework.Extensions;

namespace SimuladorFinanciero.Data
{
    public class ConceptoProductoBancoDAO : IConceptoProductoBanco
    {
        SimuladorFinancieroEntities Context = new SimuladorFinancieroEntities();
        public bool Delete(ConceptoProductoBanco entidad)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool Insert(ConceptoProductoBanco entidad)
        {
            try
            {
                Context.ConceptoProductoBanco.Add(entidad);
                return (Context.SaveChanges() != 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ConceptoProductoBanco Select(string id)
        {
            throw new NotImplementedException();
        }

        public IList<ConceptoProductoBanco> SelectAll()
        {
            throw new NotImplementedException();
        }

        public bool Update(ConceptoProductoBanco entidad)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAll()
        {
            var ConceptosProductosBancos = from i in Context.ConceptoProductoBanco
                                           select i;
            ConceptosProductosBancos.Delete();
            return (Context.SaveChanges() != 0);
        }
    }
}
