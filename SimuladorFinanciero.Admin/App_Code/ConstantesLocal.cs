using System.Configuration;

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
