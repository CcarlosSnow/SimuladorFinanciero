using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimuladorFinanciero.Data.Interface;
using SimuladorFinanciero.Data;
using SimuladorFinanciero.Entities;
using EntityFramework.Extensions;

namespace SimuladorFinanciero.Data
{
    public class BancoDAO : IBanco
    {
        SimuladorFinancieroEntities Context = new SimuladorFinancieroEntities();
        public bool Delete(Banco entidad)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool Insert(Banco entidad)
        {
            try
            {
                Context.Banco.Add(entidad);
                return (Context.SaveChanges() != 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Banco Select(string id)
        {
            var Bancos = from i in Context.Banco
                         where i.IdBanco == id
                         select i;
            return Bancos.SingleOrDefault();
        }

        public IList<Banco> SelectAll()
        {
            var Bancos = from i in Context.Banco
                         select i;
            return Bancos.ToList();
        }

        public bool Update(Banco entidad)
        {
            Context.Banco.Attach(entidad);
            Context.Entry(entidad).State = System.Data.Entity.EntityState.Modified;
            return (Context.SaveChanges() != 0);
        }

        public bool DeleteAll()
        {
            var Bancos = from i in Context.Banco
                         select i;
            Bancos.Delete();
            return (Context.SaveChanges() != 0);
        }

        public Banco Select(int id)
        {
            throw new NotImplementedException();
        }
    }
}
