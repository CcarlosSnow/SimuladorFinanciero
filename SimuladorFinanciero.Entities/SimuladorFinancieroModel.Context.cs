﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SimuladorFinanciero.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SimuladorFinancieroEntities : DbContext
    {
        public SimuladorFinancieroEntities()
            : base("name=SimuladorFinancieroEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Archivo> Archivo { get; set; }
        public virtual DbSet<Banco> Banco { get; set; }
        public virtual DbSet<Concepto> Concepto { get; set; }
        public virtual DbSet<ConceptoProducto> ConceptoProducto { get; set; }
        public virtual DbSet<Parametro> Parametro { get; set; }
        public virtual DbSet<Producto> Producto { get; set; }
        public virtual DbSet<ProductoBanco> ProductoBanco { get; set; }
        public virtual DbSet<Sugerencia> Sugerencia { get; set; }
    }
}
