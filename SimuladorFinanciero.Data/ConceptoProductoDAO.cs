using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimuladorFinanciero.Data.Interface;
using SimuladorFinanciero.Entities;

namespace SimuladorFinanciero.Data
{
    public class ConceptoProductoDAO : IConceptoProducto
    {
        SimuladorFinancieroEntities Context = new SimuladorFinancieroEntities();
        public bool Delete(ConceptoProducto entidad)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool Insert(ConceptoProducto entidad)
        {
            Context.ConceptoProducto.Add(entidad);
            return (Context.SaveChanges() != 0);
        }

        public ConceptoProducto Select(string id)
        {
            throw new NotImplementedException();
        }

        public IList<ConceptoProducto> SelectAll()
        {
            throw new NotImplementedException();
        }

        public bool Update(ConceptoProducto entidad)
        {
            throw new NotImplementedException();
        }
    }
}
