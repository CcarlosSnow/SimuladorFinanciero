using System;
using System.Configuration;

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

        public static string EmailEnvioCorreo()
        {
            return ConfigurationManager.AppSettings["EmailEnvioCorreo"];
        }

        public static string PassEnvioCorreo()
        {
            return ConfigurationManager.AppSettings["PassEnvioCorreo"];
        }
    }
}
