using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorFinanciero
{
    public static class Enumerators
    {
        public enum RespuestaCargaExcel
        {
            Correcto = 1,
            Error = 2,
            ExcelVacio = 3,
            ArchivoNoExiste = 4
        }
    }
}
