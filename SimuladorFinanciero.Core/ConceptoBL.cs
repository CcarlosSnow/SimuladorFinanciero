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
    public class ConceptoBL : IDisposable
    {
        private ConceptoDAO oConceptoDAO = null;
        public ConceptoBL()
        {
            oConceptoDAO = new ConceptoDAO();
        }

        public IList<Concepto> SelectAll()
        {
            try
            {
                return oConceptoDAO.SelectAll();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Concepto Select(string id)
        {
            try
            {
                return oConceptoDAO.Select(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Insert(Concepto entidad)
        {
            try
            {
                entidad.Estado = "0301";
                return oConceptoDAO.Insert(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Update(Concepto entidad)
        {
            try
            {
                return oConceptoDAO.Update(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Delete(Concepto entidad)
        {
            try
            {
                entidad.Estado = "0102";
                return oConceptoDAO.Update(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool BulkInsert(IEnumerable<Concepto> Conceptos)
        {
            try
            {
                foreach (Concepto i in Conceptos)
                {
                    Insert(i);
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int GetIdConcepto(string Nombre)
        {
            return oConceptoDAO.GetIdConcepto(Nombre);
        }

        public void Dispose()
        {
            var item = (ConceptoDAO)oConceptoDAO;
            if (item != null)
            {
                item.Dispose();
            }
        }
    }
}
