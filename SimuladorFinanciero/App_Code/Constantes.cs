using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SimuladorFinanciero
{
    public static class Constantes
    {
        public static string RutaArchivosExcel
        {
            get
            {
                return ConfigurationManager.AppSettings["RutaArchivosExcel"];
            }
        }
    }
}
