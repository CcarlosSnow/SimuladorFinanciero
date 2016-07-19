using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimuladorFinanciero.Data;
using SimuladorFinanciero.Data.Interface;
using SimuladorFinanciero.Entities;

namespace SimuladorFinanciero.Core
{
    public class ProductoBancoBL : IDisposable
    {
        ProductoBancoDAO oProductoBancoDAO;
        public ProductoBancoBL()
        {
            oProductoBancoDAO = new ProductoBancoDAO();
        }

        public IList<ProductoBanco> SelectAll()
        {
            try
            {
                return oProductoBancoDAO.SelectAll();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ProductoBanco Select(int id)
        {
            try
            {
                return oProductoBancoDAO.Select(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Insert(ProductoBanco entidad)
        {
            try
            {
                return oProductoBancoDAO.Insert(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Update(ProductoBanco entidad)
        {
            try
            {
                return oProductoBancoDAO.Update(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Delete(ProductoBanco entidad)
        {
            try
            {
                return oProductoBancoDAO.Delete(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool BulkInsert(IEnumerable<ProductoBanco> ProductoBancos)
        {
            try
            {
                ProductoBL oProductoBL = new ProductoBL();
                foreach (ProductoBanco i in ProductoBancos)
                {
                    i.IdProducto = oProductoBL.GetIdProducto(i.Producto.Nombre);
                    i.Producto = null;
                    Insert(i);
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool DeleteAll()
        {
            try
            {
                return oProductoBancoDAO.DeleteAll();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<Banco> SelectByIdProducto(int IdProducto)
        {
            try
            {
                return oProductoBancoDAO.SelectByIdProducto(IdProducto);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ProductoBanco SelectByIdProductoAndIdBanco(int IdProducto, string IdBanco)
        {
            try
            {
                return oProductoBancoDAO.SelectByIdProductoAndIdBanco(IdProducto, IdBanco);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void Dispose()
        {
            var item = (ProductoBancoDAO)oProductoBancoDAO;
            if (item != null)
            {
                item.Dispose();
            }
        }
    }
}
