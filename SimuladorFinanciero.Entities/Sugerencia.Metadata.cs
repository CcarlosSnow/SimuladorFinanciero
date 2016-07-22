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

        [Display(Name = "Mensaje")]
        //[Required(ErrorMessage = "(*)")]
        public string Descripcion { get; set; }

        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = Formatos.FechaAnnotation)]
        //[Required(ErrorMessage = "(*)")]
        public System.DateTime Fecha { get; set; }

        [Display(Name = "Asunto")]
        //[Required(ErrorMessage = "(*)")]
        public string Tipo { get; set; }

        [Display(Name = "Nombres y apellidos")]
        [Required(ErrorMessage = "(*)")]
        public string Nombre { get; set; }

        [Display(Name = "Correo Electrónico")]
        //[Required(ErrorMessage = "(*)")]
        public string Correo { get; set; }

        [Display(Name = "Estado")]
        //[Required(ErrorMessage = "(*)")]
        public string Estado { get; set; }
    }
}
