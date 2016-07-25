using System;

namespace SimuladorFinanciero.Helpers
{
    public static class Formatos
    {
        public const string FechaAnnotation = "{0:dd/MM/yyyy HH:mm:ss}";

        public const string FechaFormat = "dd/MM/yyyy";

        public const string FechaTitleFormat = "dd-MM-yyyy HH-mm-ss";

        public const string NumeroFormat = "0,0.00";

        public const string PorcentajeFormat = "0.0000";    

        public static string ConvertirNumeroFormat(decimal Numero)
        {
            if (Numero == 0)
            {
                return "-";
            }

            var s = Numero.ToString(NumeroFormat);
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
                return ((Numero * 1000000) / 10000).ToString(PorcentajeFormat);
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
