using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimuladorFinanciero.Data;
using SimuladorFinanciero.Entities;

namespace SimuladorFinanciero.Core
{
    public class ParametroBL : IDisposable
    {
        private ParametroDAO oParametroDAO = null;
        public ParametroBL()
        {
            oParametroDAO = new ParametroDAO();
        }

        public IList<Parametro> SelectAll()
        {
            try
            {
                return oParametroDAO.SelectAll();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Parametro Select(string id)
        {
            try
            {
                return oParametroDAO.Select(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Insert(Parametro entidad)
        {
            try
            {
                return oParametroDAO.Insert(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Update(Parametro entidad)
        {
            try
            {
                return oParametroDAO.Update(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Delete(Parametro entidad)
        {
            try
            {
                return oParametroDAO.Update(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool BulkInsert(IEnumerable<Parametro> Parametros)
        {
            try
            {
                foreach (Parametro i in Parametros)
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

        public string GetIdParametro(string Nombre)
        {
            try
            {
                return oParametroDAO.GetIdParametro(Nombre);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<Parametro> SelectByStart(string Start)
        {
            return oParametroDAO.SelectByStart(Start);
        }

        public void Dispose()
        {
            var item = (ParametroDAO)oParametroDAO;
            if (item != null)
            {
                item.Dispose();
            }
        }
    }
}