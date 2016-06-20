using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorFinanciero.Helpers
{
    public static class ConstantesHelpers
    {
        public static DateTime FechaDesde
        {
            get { return DateTime.Parse("01/01/" + DateTime.Now.Year.ToString()); }
        }
        public static DateTime FechaHasta
        {
            get { return DateTime.Parse(DateTime.Now.ToString(Formatos.FechaFormat)); }
        }
    }
}
