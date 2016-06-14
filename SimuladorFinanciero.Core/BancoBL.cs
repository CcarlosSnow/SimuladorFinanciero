using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimuladorFinanciero.Data;
using SimuladorFinanciero.Data.Interface;
using SimuladorFinanciero.Entities;
using System.Collections;
namespace SimuladorFinanciero.Core
{
    public class BancoBL : IDisposable
    {
        private IBanco oBancoDAO = null;

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
                entidad.Estado = "0101";
                return oBancoDAO.Insert(entidad);
            }
            catch (Exception)
            {
                throw;
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

        public void Dispose()
        {
            var item = (BancoDAO)oBancoDAO;
            if (item != null)
            {
                item.Dispose();
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
    }
}
