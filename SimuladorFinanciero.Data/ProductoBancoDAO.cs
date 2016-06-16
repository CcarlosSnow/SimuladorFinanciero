using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimuladorFinanciero.Data.Interface;
using SimuladorFinanciero.Entities;

namespace SimuladorFinanciero.Data
{
    public class ProductoBancoDAO : IProductoBanco
    {
        SimuladorFinancieroEntities Context = new SimuladorFinancieroEntities();
        public bool Delete(ProductoBanco entidad)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool Insert(ProductoBanco entidad)
        {
            Context.ProductoBanco.Add(entidad);
            return (Context.SaveChanges() != 0);
        }

        public ProductoBanco Select(string id)
        {
            throw new NotImplementedException();
        }

        public IList<ProductoBanco> SelectAll()
        {
            throw new NotImplementedException();
        }

        public bool Update(ProductoBanco entidad)
        {
            throw new NotImplementedException();
        }
    }
}
