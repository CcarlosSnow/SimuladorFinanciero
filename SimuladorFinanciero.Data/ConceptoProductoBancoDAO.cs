using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public IList<ConceptoProductoBanco> SelectByProductoAndBancoAndTipoComision(int IdProducto, string IdBanco, string TipoComision)
        {
            var ConceptosProductosBancos = from i in Context.ConceptoProductoBanco
                                           where i.IdBanco == IdBanco && i.IdProducto == IdProducto && i.TipoComision == TipoComision
                                           select i;

            return ConceptosProductosBancos.ToList();
        }
    }
}
