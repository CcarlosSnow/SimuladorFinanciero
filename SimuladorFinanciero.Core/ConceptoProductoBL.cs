using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimuladorFinanciero.Data;
using SimuladorFinanciero.Entities;
using SimuladorFinanciero.Data.Interface;

namespace SimuladorFinanciero.Core
{
    public class ConceptoProductoBL : IDisposable
    {
        private IConceptoProducto oConceptoProductoDAO = null;

        public ConceptoProductoBL()
        {
            oConceptoProductoDAO = new ConceptoProductoDAO();
        }

        public IList<ConceptoProducto> SelectAll()
        {
            try
            {
                return oConceptoProductoDAO.SelectAll();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ConceptoProducto Select(string id)
        {
            try
            {
                return oConceptoProductoDAO.Select(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Insert(ConceptoProducto entidad)
        {
            try
            {
                return oConceptoProductoDAO.Insert(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Update(ConceptoProducto entidad)
        {
            try
            {
                return oConceptoProductoDAO.Update(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Delete(ConceptoProducto entidad)
        {
            try
            {
                return oConceptoProductoDAO.Delete(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool BulkInsert(IEnumerable<ConceptoProducto> ConceptoProductos)
        {
            int x = 0;
            try
            {
                ProductoBL oProductoBL = new ProductoBL();
                ConceptoBL oConceptoBL = new ConceptoBL();
                foreach (ConceptoProducto i in ConceptoProductos)
                {
                    i.IdProducto = oProductoBL.GetIdProducto(i.Producto.Nombre);
                    i.IdConcepto = oConceptoBL.GetIdConcepto(i.Concepto.Nombre);
                    i.Producto = null;
                    i.Concepto = null;
                    Insert(i);
                    x++;
                }
                return true;
            }
            catch (Exception)
            {
                throw new Exception(x.ToString());
            }
        }

        public void Dispose()
        {
            var item = (ConceptoProductoDAO)oConceptoProductoDAO;
            if (item != null)
            {
                item.Dispose();
            }
        }
    }
}
