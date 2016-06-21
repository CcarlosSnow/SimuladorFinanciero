using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using SimuladorFinanciero.Helpers;

namespace SimuladorFinanciero
{
    public static class ConstantesLocal
    {
        public static string RutaArchivosExcel
        {
            get
            {
                return ConfigurationManager.AppSettings["RutaArchivosExcel"];
            }
        }
        public static string ConnectionExcelXLS(string Ruta)
        {
            return ConfigurationManager.AppSettings["ConnectionExcelXLS1"] + Ruta + ConfigurationManager.AppSettings["ConnectionExcelXLS2"];
        }

        public static string ConnectionExcelXLSX(string Ruta)
        {
            return ConfigurationManager.AppSettings["ConnectionExcelXLSX1"] + Ruta + ConfigurationManager.AppSettings["ConnectionExcelXLSX2"];
        }
    }
}
