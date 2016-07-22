using System.ComponentModel.DataAnnotations;
using SimuladorFinanciero.Helpers;

namespace SimuladorFinanciero.Entities
{
    [MetadataType(typeof(ArchivoAnnotation))]
    public partial class Archivo
    {
    }
    internal sealed class ArchivoAnnotation
    {
        public int ArchivoId { get; set; }

        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = Formatos.FechaAnnotation)]
        public System.DateTime Fecha { get; set; }
    }
}
