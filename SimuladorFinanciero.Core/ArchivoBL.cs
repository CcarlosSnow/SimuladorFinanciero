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
    public class ArchivoBL : IDisposable
    {
        private IArchivo oArchivoDAO = null;
        public ArchivoBL()
        {
            oArchivoDAO = new ArchivoDAO();
        }

        public IList<Archivo> SelectAll()
        {
            try
            {
                return oArchivoDAO.SelectAll().Where(a => a.Estado != "0503").ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Archivo Select(string id)
        {
            try
            {
                return oArchivoDAO.Select(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Insert(Archivo entidad)
        {
            try
            {
                return oArchivoDAO.Insert(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Update(Archivo entidad)
        {
            try
            {
                return oArchivoDAO.Update(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Delete(Archivo entidad)
        {
            try
            {
                entidad.Estado = "0503";
                return oArchivoDAO.Update(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Dispose()
        {
            var item = (ArchivoDAO)oArchivoDAO;
            if (item != null)
            {
                item.Dispose();
            }
        }
    }
}
