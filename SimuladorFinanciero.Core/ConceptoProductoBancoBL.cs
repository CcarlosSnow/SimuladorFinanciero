using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimuladorFinanciero.Data;
using SimuladorFinanciero.Entities;
using SimuladorFinanciero.Data.Interface;
using System.Data;

namespace SimuladorFinanciero.Core
{
    public class ConceptoProductoBancoBL : IDisposable
    {
        private ConceptoProductoBancoDAO oConceptoProductoBancoDAO = null;

        public ConceptoProductoBancoBL()
        {
            oConceptoProductoBancoDAO = new ConceptoProductoBancoDAO();
        }

        public IList<ConceptoProductoBanco> SelectAll()
        {
            try
            {
                return oConceptoProductoBancoDAO.SelectAll();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ConceptoProductoBanco Select(int id)
        {
            try
            {
                return oConceptoProductoBancoDAO.Select(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Insert(ConceptoProductoBanco entidad)
        {
            try
            {
                return oConceptoProductoBancoDAO.Insert(entidad);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Update(ConceptoProductoBanco entidad)
        {
            try
            {
                return oConceptoProductoBancoDAO.Update(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Delete(ConceptoProductoBanco entidad)
        {
            try
            {
                return oConceptoProductoBancoDAO.Delete(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool BulkInsert(List<ConceptoProductoBanco> ConceptosProductosBancos)
        {
            try
            {
                ProductoBL oProductoBL = new ProductoBL();
                ConceptoBL oConceptoBL = new ConceptoBL();
                ParametroBL oParametroBL = new ParametroBL();
                foreach (ConceptoProductoBanco i in ConceptosProductosBancos)
                {
                    i.IdProducto = oProductoBL.GetIdProducto(i.ProductoBanco.Producto.Nombre);
                    i.IdConcepto = oConceptoBL.GetIdConcepto(i.Concepto.Nombre);
                    i.TipoComision = oParametroBL.GetIdParametro(i.TipoComision);

                    i.ProductoBanco = null;
                    i.Concepto = null;
                    Insert(i);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool DeleteAll()
        {
            try
            {
                return oConceptoProductoBancoDAO.DeleteAll();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void Dispose()
        {
            var item = (ConceptoProductoBancoDAO)oConceptoProductoBancoDAO;
            if (item != null)
            {
                item.Dispose();
            }
        }
    }
}
