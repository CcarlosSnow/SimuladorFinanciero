using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace SimuladorFinanciero
{
    public static class Constantes
    {
        //private static string cRutaArchivosExcel = "";
        public static string RutaArchivosExcel
        {
            get
            {
                return ConfigurationManager.AppSettings["RutaArchivosExcel"];
            }
        }
    }
}