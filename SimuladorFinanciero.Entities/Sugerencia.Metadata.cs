using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SimuladorFinanciero.Helpers;

namespace SimuladorFinanciero.Entities
{
    [MetadataType(typeof(SugerenciaAnnotation))]
    public partial class Sugerencia
    {
    }

    internal sealed class SugerenciaAnnotation
    {
        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = Formatos.FechaAnnotation)]
        public System.DateTime Fecha { get; set; }
    }
}
