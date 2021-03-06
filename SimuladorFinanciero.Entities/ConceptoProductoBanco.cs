//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class ConceptoProductoBanco
    {
        public int IdConcepto { get; set; }
        public int IdProducto { get; set; }
        public string IdBanco { get; set; }
        public string TipoComision { get; set; }
        public decimal Tasa30 { get; set; }
        public decimal Tasa60 { get; set; }
        public decimal Tasa90 { get; set; }
        public decimal Minimo { get; set; }
        public decimal Maximo { get; set; }
        public decimal METasaMax { get; set; }
        public decimal METasaMin { get; set; }
        public decimal MEMin { get; set; }
        public decimal MEMax { get; set; }
        public string Observaciones { get; set; }
    
        public virtual Concepto Concepto { get; set; }
        public virtual Parametro Parametro { get; set; }
        public virtual ProductoBanco ProductoBanco { get; set; }
    }
}
