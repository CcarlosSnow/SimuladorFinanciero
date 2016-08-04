using System;
using System.Collections.Generic;
using System.Linq;
using SimuladorFinanciero.Data.Interface;
using SimuladorFinanciero.Entities;
using EntityFramework.Extensions;

namespace SimuladorFinanciero.Data
{
    public class ConceptoProductoBancoDAO : IConceptoProductoBanco
    {
        SimuladorFinancieroEntities Context = new SimuladorFinancieroEntities();
        public bool Delete(ConceptoProductoBanco entidad)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool Insert(ConceptoProductoBanco entidad)
        {
            try
            {
                Context.ConceptoProductoBanco.Add(entidad);
                return (Context.SaveChanges() != 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ConceptoProductoBanco Select(int id)
        {
            throw new NotImplementedException();
        }

        public ConceptoProductoBanco Select(ConceptoProductoBanco entidad)
        {
            try
            {
                var ConceptoProductoBanco = from i in Context.ConceptoProductoBanco
                                            where i.IdConcepto == entidad.IdConcepto &&
                                                  i.IdProducto == entidad.IdProducto &&
                                                  i.IdBanco == entidad.IdBanco &&
                                                  i.Tasa30 == entidad.Tasa30 &&
                                                  i.Tasa60 == entidad.Tasa60 &&
                                                  i.Tasa90 == entidad.Tasa90 &&
                                                  i.Minimo == entidad.Minimo &&
                                                  i.Maximo == entidad.Maximo &&
                                                  i.METasaMax == entidad.METasaMax &&
                                                  i.METasaMin == entidad.METasaMin &&
                                                  i.MEMin == entidad.MEMin &&
                                                  i.MEMax == entidad.MEMax
                                            select i;

                return ConceptoProductoBanco.SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<ConceptoProductoBanco> SelectAll()
        {
            throw new NotImplementedException();
        }

        public bool Update(ConceptoProductoBanco entidad)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAll()
        {
            var ConceptosProductosBancos = from i in Context.ConceptoProductoBanco
                                           select i;
            ConceptosProductosBancos.Delete();
            return (Context.SaveChanges() != 0);
        }

        public List<ConceptoProductoBancoDTO> SelectByProductoAndBancoAndTipoComision(int IdProducto, string IdBanco, string TipoComision, int Periodo)
        {
            IQueryable<ConceptoProductoBancoDTO> ConceptosProductosBancos = null;
            ProductoDAO oProductoDAO = new ProductoDAO();
            Producto oProducto = oProductoDAO.Select(IdProducto);
            if (oProducto.Nombre.Substring(0,3) == "1.5" || oProducto.Nombre.Substring(0, 3) == "4.1")
            {
                ConceptosProductosBancos = from i in Context.ConceptoProductoBanco
                                           where i.IdBanco == IdBanco && i.IdProducto == IdProducto && i.TipoComision == TipoComision
                                           select new ConceptoProductoBancoDTO
                                           {
                                               IdConcepto = i.IdConcepto,
                                               IdProducto = i.IdProducto,
                                               IdBanco = i.IdBanco,
                                               TipoComision = i.TipoComision,
                                               Tasa30 = i.METasaMax,
                                               Tasa60 = i.Tasa60,
                                               Tasa90 = i.Tasa90,
                                               Minimo = i.MEMin,
                                               Maximo = i.MEMax,
                                               METasaMax = i.METasaMax,
                                               METasaMin = i.METasaMin,
                                               MEMin = i.MEMin,
                                               MEMax = i.MEMax,
                                               Observaciones = i.Observaciones,
                                               Concepto = i.Concepto,
                                               ProductoBanco = i.ProductoBanco,
                                               Parametro = i.Parametro
                                           };
                return ConceptosProductosBancos.ToList();
            }
            switch (Periodo)
            {
                case 30:
                    ConceptosProductosBancos = from i in Context.ConceptoProductoBanco
                                               where i.IdBanco == IdBanco && i.IdProducto == IdProducto && i.TipoComision == TipoComision
                                               select new ConceptoProductoBancoDTO
                                               {
                                                   IdConcepto = i.IdConcepto,
                                                   IdProducto = i.IdProducto,
                                                   IdBanco = i.IdBanco,
                                                   TipoComision = i.TipoComision,
                                                   Tasa30 = i.Tasa30,
                                                   Tasa60 = i.Tasa60,
                                                   Tasa90 = i.Tasa90,
                                                   Minimo = i.Minimo,
                                                   Maximo = i.Maximo,
                                                   METasaMax = i.METasaMax,
                                                   METasaMin = i.METasaMin,
                                                   MEMin = i.MEMin,
                                                   MEMax = i.MEMax,
                                                   Observaciones = i.Observaciones,
                                                   Concepto = i.Concepto,
                                                   ProductoBanco = i.ProductoBanco,
                                                   Parametro = i.Parametro
                                               };
                    break;
                case 60:
                    ConceptosProductosBancos = from i in Context.ConceptoProductoBanco
                                               where i.IdBanco == IdBanco && i.IdProducto == IdProducto && i.TipoComision == TipoComision
                                               select new ConceptoProductoBancoDTO
                                               {
                                                   IdConcepto = i.IdConcepto,
                                                   IdProducto = i.IdProducto,
                                                   IdBanco = i.IdBanco,
                                                   TipoComision = i.TipoComision,
                                                   Tasa30 = i.Tasa60,
                                                   Tasa60 = i.Tasa60,
                                                   Tasa90 = i.Tasa90,
                                                   Minimo = i.Minimo,
                                                   Maximo = i.Maximo,
                                                   METasaMax = i.METasaMax,
                                                   METasaMin = i.METasaMin,
                                                   MEMin = i.MEMin,
                                                   MEMax = i.MEMax,
                                                   Observaciones = i.Observaciones,
                                                   Concepto = i.Concepto,
                                                   ProductoBanco = i.ProductoBanco,
                                                   Parametro = i.Parametro
                                               };
                    break;
                case 90:
                    ConceptosProductosBancos = from i in Context.ConceptoProductoBanco
                                               where i.IdBanco == IdBanco && i.IdProducto == IdProducto && i.TipoComision == TipoComision
                                               select new ConceptoProductoBancoDTO
                                               {
                                                   IdConcepto = i.IdConcepto,
                                                   IdProducto = i.IdProducto,
                                                   IdBanco = i.IdBanco,
                                                   TipoComision = i.TipoComision,
                                                   Tasa30 = i.Tasa90,
                                                   Tasa60 = i.Tasa60,
                                                   Tasa90 = i.Tasa90,
                                                   Minimo = i.Minimo,
                                                   Maximo = i.Maximo,
                                                   METasaMax = i.METasaMax,
                                                   METasaMin = i.METasaMin,
                                                   MEMin = i.MEMin,
                                                   MEMax = i.MEMax,
                                                   Observaciones = i.Observaciones,
                                                   Concepto = i.Concepto,
                                                   ProductoBanco = i.ProductoBanco,
                                                   Parametro = i.Parametro
                                               };
                    break;
                default:
                    ConceptosProductosBancos = from i in Context.ConceptoProductoBanco
                                               where i.IdBanco == IdBanco && i.IdProducto == IdProducto && i.TipoComision == TipoComision
                                               select new ConceptoProductoBancoDTO
                                               {
                                                   IdConcepto = i.IdConcepto,
                                                   IdProducto = i.IdProducto,
                                                   IdBanco = i.IdBanco,
                                                   TipoComision = i.TipoComision,
                                                   Tasa30 = i.Tasa30,
                                                   Tasa60 = i.Tasa60,
                                                   Tasa90 = i.Tasa90,
                                                   Minimo = i.Minimo,
                                                   Maximo = i.Maximo,
                                                   METasaMax = i.METasaMax,
                                                   METasaMin = i.METasaMin,
                                                   MEMin = i.MEMin,
                                                   MEMax = i.MEMax,
                                                   Observaciones = i.Observaciones,
                                                   Concepto = i.Concepto,
                                                   ProductoBanco = i.ProductoBanco,
                                                   Parametro = i.Parametro
                                               };
                    break;
            }

            return ConceptosProductosBancos.ToList();
        }
    }
}
