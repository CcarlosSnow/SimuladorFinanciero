using System;
using System.Collections.Generic;
using System.Linq;
using SimuladorFinanciero.Data.Interface;
using SimuladorFinanciero.Entities;
using System.Data;

namespace SimuladorFinanciero.Data
{
    public class ArchivoDAO : IArchivo
    {
        SimuladorFinancieroEntities Context = new SimuladorFinancieroEntities();
        public bool Delete(Archivo entidad)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool Insert(Archivo entidad)
        {
            Context.Archivo.Add(entidad);
            return (Context.SaveChanges() != 0);
        }

        public Archivo Select(int id)
        {
            var Archivos = from i in Context.Archivo
                           where i.ArchivoId == id
                           select i;
            return Archivos.SingleOrDefault();
        }

        public Archivo SelectActive()
        {
            var Archivo = from i in Context.Archivo
                          where i.Estado == "0501"
                          select i;
            return Archivo.FirstOrDefault();
        }

        public IList<Archivo> SelectAll()
        {
            var Archivos = from i in Context.Archivo
                           orderby i.ArchivoId descending
                           select i;
            return Archivos.ToList();
        }

        public bool Update(Archivo entidad)
        {
            Context.Archivo.Attach(entidad);
            Context.Entry(entidad).State = System.Data.Entity.EntityState.Modified;
            return (Context.SaveChanges() != 0);
        }

        public bool DisableActiveExcel()
        {
            IList<Archivo> Archivos = (from i in Context.Archivo
                                       where i.Estado == "0501"
                                       select i).ToList();

            foreach (Archivo i in Archivos)
            {
                i.Estado = "0502";
                Update(i);
            }
            return true;
        }

        public bool InsertExcel(IEnumerable<Banco> Bancos, IEnumerable<Producto> Productos, IEnumerable<Concepto> Conceptos, IEnumerable<ProductoBanco> ProductosBancos, IEnumerable<ConceptoProductoBanco> ConceptosProductosBancos)
        {
            BancoDAO oBancoDAO = new BancoDAO();
            ProductoDAO oProductoDAO = new ProductoDAO();
            ConceptoDAO oConceptoDAO = new ConceptoDAO();
            ProductoBancoDAO oProductoBancoDAO = new ProductoBancoDAO();
            ConceptoProductoBancoDAO oConceptoProductoBancoDAO = new ConceptoProductoBancoDAO();
            ParametroDAO oParametroDAO = new ParametroDAO();

            using (var Transaction = Context.Database.BeginTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    oConceptoProductoBancoDAO.DeleteAll();
                    oProductoBancoDAO.DeleteAll();
                    oConceptoDAO.DeleteAll();
                    oProductoDAO.DeleteAll();
                    oBancoDAO.DeleteAll();

                    foreach (Banco i in Bancos)
                    {
                        i.Estado = "0101";
                        Context.Banco.Add(i);
                        Context.SaveChanges();
                    }

                    foreach (Producto i in Productos)
                    {
                        i.Estado = "0301";
                        Context.Producto.Add(i);
                        Context.SaveChanges();
                    }

                    foreach (Concepto i in Conceptos)
                    {
                        i.Estado = "0301";
                        Context.Concepto.Add(i);
                        Context.SaveChanges();
                    }

                    foreach (ProductoBanco i in ProductosBancos)
                    {
                        var IdProducto = (from x in Context.Producto
                                         where x.Nombre == i.Producto.Nombre
                                         select x.IdProducto).First();

                        i.IdProducto = IdProducto;
                        i.Producto = null;
                        Context.ProductoBanco.Add(i);
                        Context.SaveChanges();
                    }

                    foreach (ConceptoProductoBanco i in ConceptosProductosBancos)
                    {
                        var IdProducto = (from x in Context.Producto
                                          where x.Nombre == i.ProductoBanco.Producto.Nombre
                                          select x.IdProducto).First();

                        i.IdProducto = IdProducto;

                        var IdConcepto = (from x in Context.Concepto
                                         where x.Nombre == i.Concepto.Nombre
                                          select x.IdConcepto).First();

                        i.IdConcepto = IdConcepto;

                        var IdParametro = (from x in Context.Parametro
                                          where x.Nombre == i.TipoComision
                                           select x.IdParametro).First();

                        i.TipoComision = IdParametro;

                        i.ProductoBanco = null;
                        i.Concepto = null;

                        var ConceptoProductoBanco = (from x in Context.ConceptoProductoBanco
                                                    where x.IdConcepto == i.IdConcepto &&
                                                          x.IdProducto == i.IdProducto &&
                                                          x.IdBanco == i.IdBanco &&
                                                          x.Tasa30 == i.Tasa30 &&
                                                          x.Tasa60 == i.Tasa60 &&
                                                          x.Tasa90 == i.Tasa90 &&
                                                          x.Minimo == i.Minimo &&
                                                          x.Maximo == i.Maximo &&
                                                          x.METasaMax == i.METasaMax &&
                                                          x.METasaMin == i.METasaMin &&
                                                          x.MEMin == i.MEMin &&
                                                          x.MEMax == i.MEMax
                                                    select x).SingleOrDefault();

                        if (ConceptoProductoBanco == null)
                        {
                            Context.ConceptoProductoBanco.Add(i);
                        }
                        Context.SaveChanges();
                    }

                    Transaction.Commit();
                }
                    catch (Exception ex)
                {
                    Transaction.Rollback();
                    throw ex;
                }
            }

            return true;
        }
    }
}
