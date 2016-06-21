﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimuladorFinanciero.Data.Interface;
using SimuladorFinanciero.Entities;
using EntityFramework.Extensions;

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

        public ProductoBanco Select(int id)
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

        public bool DeleteAll()
        {
            var ProductosBancos = from i in Context.ProductoBanco
                                  select i;
            ProductosBancos.Delete();
            return (Context.SaveChanges() != 0);
        }
    }
}
