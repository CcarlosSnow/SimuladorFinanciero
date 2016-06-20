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
    public class ProductoDAO : IProducto
    {
        SimuladorFinancieroEntities Context = new SimuladorFinancieroEntities();

        public bool Delete(Producto entidad)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool Insert(Producto entidad)
        {
            Context.Producto.Add(entidad);
            return (Context.SaveChanges() != 0);
        }

        public Producto Select(string id)
        {
            throw new NotImplementedException();
        }

        public IList<Producto> SelectAll()
        {
            throw new NotImplementedException();
        }

        public bool Update(Producto entidad)
        {
            throw new NotImplementedException();
        }

        public int GetIdProducto(string Nombre)
        {
            var IdProducto = from i in Context.Producto
                             where i.Nombre == Nombre
                             select i.IdProducto;
            return IdProducto.First();
        }

        public bool DeleteAll()
        {
            var Productos = from i in Context.Producto
                            select i;
            Productos.Delete();
            return (Context.SaveChanges() != 0);
        }
    }
}
