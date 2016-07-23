using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorFinanciero.Entities
{
    public class ConceptoProductoBancoDTO
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
