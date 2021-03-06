﻿using System;
using System.Collections.Generic;
using System.Linq;
using SimuladorFinanciero.Data;
using SimuladorFinanciero.Entities;

namespace SimuladorFinanciero.Core
{
    public class ArchivoBL : IDisposable
    {
        private ArchivoDAO oArchivoDAO = null;
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
            catch (Exception ex)
            {
                throw;
            }
        }

        public Archivo Select(int id)
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

        public Archivo SelectActive()
        {
            try
            {
                return oArchivoDAO.SelectActive();
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
                entidad.Fecha = DateTime.Now;
                entidad.Estado = "0501";
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

        public bool Delete(int ArchivoId)
        {
            try
            {
                Archivo entidad = Select(ArchivoId);
                entidad.Estado = "0503";
                return oArchivoDAO.Update(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool UploadExcelFile(Archivo entidad)
        {
            try
            {
                if (oArchivoDAO.DisableActiveExcel())
                {
                    return Insert(entidad);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ActiveExcelFile(int IdArchivo)
        {
            try
            {
                if (oArchivoDAO.DisableActiveExcel())
                {
                    Archivo entidad = Select(IdArchivo);
                    entidad.Estado = "0501";
                    return Update(entidad);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool InsertExcel(IEnumerable<Banco> Bancos, IEnumerable<Producto> Productos, IEnumerable<Concepto> Conceptos, IEnumerable<ProductoBanco> ProductosBancos, IEnumerable<ConceptoProductoBanco> ConceptosProductosBancos)
        {
            try
            {
                return oArchivoDAO.InsertExcel(Bancos, Productos, Conceptos, ProductosBancos, ConceptosProductosBancos);
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
