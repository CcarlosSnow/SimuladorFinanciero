using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorFinanciero.Helpers
{
    public static class Formatos
    {
        public const string FechaAnnotation = "{0:dd/MM/yyyy HH:mm:ss}";

        public const string FechaFormat = "dd/MM/yyyy";

        public const string FechaTitleFormat = "dd-MM-yyyy HH-mm-ss";

        public const string NumeroFormat = "{0:0.00}";

        public static string ConvertirNumeroFormat(decimal Numero)
        {
            if (Numero == 0)
            {
                return "-";
            }

            var s = string.Format(NumeroFormat, Numero);
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
                return Math.Truncate((Numero * 1000000) / 10000).ToString();
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
