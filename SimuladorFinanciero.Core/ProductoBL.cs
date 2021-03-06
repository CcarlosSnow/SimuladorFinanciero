﻿using System;
using System.Collections.Generic;
using SimuladorFinanciero.Entities;
using SimuladorFinanciero.Data;

namespace SimuladorFinanciero.Core
{
    public class ProductoBL : IDisposable
    {
        ProductoDAO oProductoDAO = null;

        public ProductoBL()
        {
            oProductoDAO = new ProductoDAO();
        }

        public IList<Producto> SelectAll()
        {
            try
            {
                return oProductoDAO.SelectAll();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Producto Select(int id)
        {
            try
            {
                return oProductoDAO.Select(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Insert(Producto entidad)
        {
            try
            {
                return oProductoDAO.Insert(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Update(Producto entidad)
        {
            try
            {
                return oProductoDAO.Update(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Delete(Producto entidad)
        {
            try
            {
                entidad.Estado = "0302";
                return oProductoDAO.Update(entidad);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool BulkInsert(IEnumerable<Producto> Productos)
        {
            try
            {
                foreach (Producto i in Productos)
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

        public int GetIdProducto(string Nombre)
        {
            try
            {
                return oProductoDAO.GetIdProducto(Nombre);
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
                return oProductoDAO.DeleteAll();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<Producto> SelectByTipo(int Tipo)
        {
            try
            {
                return oProductoDAO.SelectByTipo(Tipo);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Dispose()
        {
            var item = (ProductoDAO)oProductoDAO;
            if (item != null)
            {
                item.Dispose();
            }
        }
    }
}
