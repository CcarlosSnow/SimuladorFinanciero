﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimuladorFinanciero.Data.Interface;
using SimuladorFinanciero.Entities;
using EntityFramework.Extensions;

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

        public Archivo Select(string id)
        {
            var Archivos = from i in Context.Archivo
                           where i.ArchivoId == int.Parse(id)
                           select i;
            return Archivos.SingleOrDefault();
        }

        public IList<Archivo> SelectAll()
        {
            var Archivos = from i in Context.Archivo
                           orderby i.Fecha descending
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
        public bool BulkDelete()
        {
            var Archivos = from i in Context.Archivo
                           select i;

            Archivos.Delete();
            return (Context.SaveChanges() != 0);
        }
    }
}
