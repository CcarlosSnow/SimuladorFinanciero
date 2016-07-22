using System;
using System.Collections.Generic;
using SimuladorFinanciero.Data;
using SimuladorFinanciero.Entities;
namespace SimuladorFinanciero.Core
{
    public class BancoBL : IDisposable
    {
        private BancoDAO oBancoDAO = null;

        public BancoBL()
        {
            oBancoDAO = new BancoDAO();
        }

        public IList<Banco> SelectAll()
        {
            try
            {
                return oBancoDAO.SelectAll();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Banco Select(string id)
        {
            try
            {
                return oBancoDAO.Select(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Insert(Banco entidad)
        {
            try
            {
                return oBancoDAO.Insert(entidad);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Update(Banco entidad)
        {
            try
            {
                return oBancoDAO.Update(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Delete(Banco entidad)
        {
            try
            {
                entidad.Estado = "0102";
                return oBancoDAO.Update(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }
    
        public bool BulkInsert(IEnumerable<Banco> Bancos)
        {
            try
            {
                foreach (Banco i in Bancos)
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

        public bool DeleteAll()
        {
            try
            {
                return oBancoDAO.DeleteAll();
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void Dispose()
        {
            var item = (BancoDAO)oBancoDAO;
            if (item != null)
            {
                item.Dispose();
            }
        }
    }
}
