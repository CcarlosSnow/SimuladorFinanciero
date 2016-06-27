using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimuladorFinanciero.Entities;
using SimuladorFinanciero.Data;

namespace SimuladorFinanciero.Core
{
    public class SugerenciaBL : IDisposable
    {
        SugerenciaDAO oSugerenciaDAO = null;

        public SugerenciaBL()
        {
            oSugerenciaDAO = new SugerenciaDAO();
        }

        public IList<Sugerencia> SelectAll()
        {
            try
            {
                return oSugerenciaDAO.SelectAll();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Sugerencia Select(int id)
        {
            try
            {
                return oSugerenciaDAO.Select(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Insert(Sugerencia entidad)
        {
            try
            {
                return oSugerenciaDAO.Insert(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Update(Sugerencia entidad)
        {
            try
            {
                return oSugerenciaDAO.Update(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Delete(Sugerencia entidad)
        {
            try
            {
                return oSugerenciaDAO.Update(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool BulkInsert(IEnumerable<Sugerencia> Sugerencias)
        {
            try
            {
                foreach (Sugerencia i in Sugerencias)
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

        public IList<Sugerencia> SelectByFechaAndTipo(DateTime? Desde = null, DateTime? Hasta = null, string Tipo = "", string Estado = "")
        {
            try
            {
                return oSugerenciaDAO.SelectByFechaAndTipo(Desde, Hasta, Tipo, Estado);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool UpdateEstado(int IdSugerencia, string Estado)
        {
            try
            {
                Sugerencia entidad = Select(IdSugerencia);
                entidad.Estado = Estado;
                return Update(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Dispose()
        {
            var item = (SugerenciaDAO)oSugerenciaDAO;
            if (item != null)
            {
                item.Dispose();
            }
        }
    }
}

