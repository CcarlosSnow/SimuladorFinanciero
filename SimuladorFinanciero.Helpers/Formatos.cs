using System;
using System.Globalization;
using System.Web.Configuration;
using System.Configuration;

namespace SimuladorFinanciero.Helpers
{
    public static class Formatos
    {
        public const string FechaAnnotation = "{0:dd/MM/yyyy HH:mm:ss}";

        public const string FechaFormat = "dd/MM/yyyy";

        public const string FechaTitleFormat = "dd-MM-yyyy HH-mm-ss";

        public const string NumeroFormat = "0,0.00";

        public const string PorcentajeFormat = "0.0000";    

        public static string ConvertirFechaFormatPiePagina(DateTime Fecha)
        {
            GlobalizationSection oGlobalizationSection = (GlobalizationSection)ConfigurationManager.GetSection("system.web/globalization");
            DateTimeFormatInfo oDateTimeFormatInfo = new CultureInfo(oGlobalizationSection.Culture).DateTimeFormat;
            string Mes = oDateTimeFormatInfo.GetMonthName(Fecha.Month);
            return Mes + " de " + Fecha.Year;
        }

        public static string ConvertirNumeroFormat(decimal Numero)
        {
            if (Numero == 0)
            {
                return "-";
            }

            var s = Numero.ToString("N");
            return s;
        }

        public static string ConvertirNumeroFormatTasa(decimal Numero)
        {
            if (Numero == 0)
            {
                return "-";
            }
            else
            {
                return ((Numero * 1000000) / 10000).ToString().TrimEnd('0').TrimEnd('.');
            }
        }

        public static string CalcularGasto(decimal Tasa, decimal Monto, decimal Minimo, decimal Maximo, ref decimal GastoTotal)
        {
            decimal Gasto = Tasa * Monto;
            if (Minimo > 0 && Maximo == 0)
            {
                if (Gasto < Minimo)
                {
                    Gasto = Minimo;
                }
            }
            else if (Minimo == 0 && Maximo > 0)
            {
                if (Tasa > 0)
                {
                    if (Gasto > Maximo)
                    {
                        Gasto = Maximo;
                    }
                }
                else
                {
                    Gasto = Maximo;
                }
            }

            else if (Minimo > 0 && Maximo > 0)
            {
                if (Minimo > Gasto)
                {

                    Gasto = Minimo;
                }
                else if (Maximo < Gasto)
                {
                    Gasto = Maximo;
                }
            }

            GastoTotal = GastoTotal + Gasto;

            return ConvertirNumeroFormat(Gasto);
        }
    }
}
