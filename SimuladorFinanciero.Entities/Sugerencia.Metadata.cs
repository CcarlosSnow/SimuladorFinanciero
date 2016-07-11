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

        public int IdSugerencia { get; set; }

        [Display(Name = "Descripción")]
        [Required]
        public string Descripcion { get; set; }

        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = Formatos.FechaAnnotation)]
        [Required]
        public System.DateTime Fecha { get; set; }

        [Display(Name = "Tipo")]
        [Required]
        public string Tipo { get; set; }

        [Display(Name = "Nombres y apellidos")]
        [Required]
        public string Nombre { get; set; }

        [Display(Name = "Correo Electrónico")]
        [Required]
        public string Correo { get; set; }

        [Display(Name = "Estado")]
        [Required]
        public string Estado { get; set; }
    }
}
