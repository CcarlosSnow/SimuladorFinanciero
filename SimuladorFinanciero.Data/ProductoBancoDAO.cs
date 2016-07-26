using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<Banco> SelectByIdProducto(int IdProducto)
        {
            var Bancos = from i in Context.ProductoBanco
                         where i.IdProducto == IdProducto
                         select i.Banco;

            return Bancos.ToList();
        }

        public ProductoBanco SelectByIdProductoAndIdBanco(int IdProducto, string IdBanco)
        {
            var ProductoBanco = from i in Context.ProductoBanco
                                where i.IdBanco == IdBanco && i.IdProducto == IdProducto
                                select i;

            return ProductoBanco.SingleOrDefault();
        }

        public string SelectIdProductoByBanco(string IdBanco)
        {
            var ProductosBancos = (from i in Context.ProductoBanco
                                   where i.IdBanco == IdBanco
                                   select i).ToList();
            int Conteo = ProductosBancos.Count;
            int Contador = 0;
            string Resultado = "";
            if (Conteo == 1)
            {
                Resultado = "[" + ProductosBancos[0].IdProducto + "]";
            }
            else
            {
                foreach (ProductoBanco i in ProductosBancos)
                {
                    Contador++;
                    if (Contador == 1)
                    {
                        Resultado = "[" + i.IdProducto + ",";
                    }
                    else if (Contador < Conteo)
                    {
                        Resultado = Resultado + i.IdProducto + ",";
                    }
                    else if (Contador == Conteo)
                    {
                        Resultado = Resultado + i.IdProducto + "]";
                    }
                    
                }
            }
            return Resultado;
        }
    }
}
