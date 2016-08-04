using System;
using System.Collections.Generic;
using System.Linq;
using SimuladorFinanciero.Core;
using SimuladorFinanciero.Entities;
using SimuladorFinanciero.Helpers;

namespace SimuladorFinanciero
{
    public class ResultadoService
    {
        public string GenerarExcelBody(string Tipo, int IdProducto, decimal Monto, int Periodo, string Bancos, int Fuente)
        {
            ArchivoBL oArchivoBL = new ArchivoBL();
            string UltimaFechaPublicacion = Formatos.ConvertirFechaFormatPiePagina(oArchivoBL.SelectActive().Fecha);

            string[] BancosArray = Bancos.Split(',');

            ProductoBL oProductoBL = new ProductoBL();
            Producto oProducto = oProductoBL.Select(IdProducto);
            ProductoBancoBL oProductoBancoBL = new ProductoBancoBL();
            List<ProductoBanco> ListaProductosBancos = new List<ProductoBanco>();
            List<ConceptoProductoBancoDTO> ListaConceptosProductosBancosUsuales = new List<ConceptoProductoBancoDTO>();
            List<ConceptoProductoBancoDTO> ListaConceptosProductosBancosEventuales = new List<ConceptoProductoBancoDTO>();
            ConceptoProductoBancoBL oConceptoProductoBancoBL = new ConceptoProductoBancoBL();
            string FilaPeriodo = "";
            foreach (string i in BancosArray)
            {
                var ProductoBanco = oProductoBancoBL.SelectByIdProductoAndIdBanco(IdProducto, i);
                ListaProductosBancos.Add(ProductoBanco);

                var ConceptosProductosBancosUsuales = oConceptoProductoBancoBL.SelectByProductoAndBancoAndTipoComision(IdProducto, i, "0401", Periodo).Concat(ListaConceptosProductosBancosUsuales);
                ListaConceptosProductosBancosUsuales = ConceptosProductosBancosUsuales.ToList();

                var ConceptosProductosBancosEventuales = oConceptoProductoBancoBL.SelectByProductoAndBancoAndTipoComision(IdProducto, i, "0402", Periodo).Concat(ListaConceptosProductosBancosEventuales);
                ListaConceptosProductosBancosEventuales = ConceptosProductosBancosEventuales.ToList();
            }

            if (Periodo == 0)
            {
                FilaPeriodo = "";
            }
            else
            {
                FilaPeriodo = "<tr>" +
                            "<td style='border: 1px SOLID #000000;' colspan=7 height='27' align='left' valign=top bgcolor='#F1F1F1'><font face='Helvetica' size=2>Periodo: " + Periodo.ToString() + " días </font></td>" +
                            "</tr>";
            }
            string ResponseBody = "";

            if (Fuente == 0)
            {
                ResponseBody = "<!DOCTYPE html>";
            }

            ResponseBody = ResponseBody + "<html><head><meta http-equiv='content - type' content='text / html; charset = utf - 8'/></head><body>" +
                                  "<table cellspacing='0' border='0'>" +
                                  "<tr>" +
                                  "<td style='border: 1px SOLID #000000;' colspan=7 height='38' align='center' valign='top' bgcolor='#CC0000'>" +
                                  "<b style='color: white; '>" +
                                  "<font face='Helvetica' size=5>Simulador Financiero</font>" +
                                  "</b>" +
                                  "</td>" +
                                  "</tr>" +
                                  "<td style='border: 1px SOLID #000000;' colspan=7 height='27' align='center' valign=top bgcolor='#FFFFFF'><font face='Helvetica' size=2>Tarifario actualizado a " + UltimaFechaPublicacion + "</font></td>" +
                                  "</tr>" +
                                  "<tr>" +
                                  "<td style='border: 1px SOLID #000000;' colspan=7 height='27' align='left' valign=top bgcolor='#F1F1F1'><font face='Helvetica' size=2>Producto: " + Tipo + "-" + oProducto.Nombre.Substring(4) + "</font></td>" +
                                  "</tr>" +
                                  "<tr>" +
                                  "<td style='border: 1px SOLID #000000;' colspan=7 height='27' align='left' valign=top bgcolor='#DEDEDE'><font face='Helvetica' size=2>Monto: $" + Formatos.ConvertirNumeroFormat(Monto) + "</font></td>" +
                                  "</tr>" +
                                  FilaPeriodo +
                                  "<tr>" +
                                  "<td style='border: 1px SOLID #000000;' height='28' align='left' valign=top><font face='Helvetica' size=2><br></font></td>" +
                                  "<td style='border: 1px SOLID #000000; border-bottom: 1px SOLID #ffffff;' colspan=2 align='center' valign='top'><b><font face='Helvetica' size=2>Banco</font></b></td>" +
                                  "<td style='border: 1px SOLID #000000; border-bottom: 1px SOLID #ffffff;' colspan=4 align='center' valign='top'><b><font face='Helvetica' size=2>Gasto Financiero</font></b></td>" +
                                  "</tr>";
            int ConteoRowSpanUsuales = 0;
            decimal GastoTotalUsual = 0;
            int RowSpanUsuales = 0;

            int ConteoRowSpanEventuales = 0;
            decimal GastoTotalEventual = 0;
            int RowSpanEventuales = 0;

            foreach (var i in ListaProductosBancos)
            {
                ConteoRowSpanUsuales = 0;
                ConteoRowSpanEventuales = 0;
                RowSpanUsuales = 0;
                RowSpanEventuales = 0;
                foreach (var cpbu in ListaConceptosProductosBancosUsuales)
                {
                    if (i.Banco.IdBanco == cpbu.IdBanco)
                    {
                        RowSpanUsuales = RowSpanUsuales + 1;
                        Formatos.CalcularGasto(cpbu.Tasa30, Monto, cpbu.Minimo, cpbu.Maximo, ref GastoTotalUsual);
                    }
                }

                ResponseBody = ResponseBody + "<tr>" +
                               "<td style='border: 1px SOLID #000000; border-right: 1px SOLID #ffffff;' height='28' align='left' valign=top><font face='Helvetica' size=2><br></font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' colspan=2 align='center' valign='top' bgcolor='#165778'><font face='Helvetica' size=2 color='#FFFFFF'>" + i.Banco.Nombre + "</font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' colspan=4 align='center' valign='top' bgcolor='#165778'><font face='Helvetica' size=2 color='#FFFFFF'>$ " + Formatos.ConvertirNumeroFormat(GastoTotalUsual) + "</font></td>" +
                               "</tr>" +
                               "<tr>" +
                               "<td style='border: 1px SOLID #000000; border-right: 1px SOLID #ffffff;' height='28' align='left' valign=top><font face='Helvetica' size=2><br></font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' align='center' valign='top' bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Concepto</font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' align='center' valign='top' bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Observaciones</font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' align='center' valign='top' bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Tasa %</font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' align='center' valign='top' bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Mínimo $</font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' align='center' valign='top' bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Máximo $</font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' align='center' valign='top' bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Gastos $</font></td>" +
                               "</tr>";
                //RowSpanUsuales = 0;

                foreach (var cpbu in ListaConceptosProductosBancosUsuales)
                {

                    if (i.Banco.IdBanco == cpbu.IdBanco)
                    {
                        if (ConteoRowSpanUsuales == 0)
                        {
                            ResponseBody = ResponseBody + "<tr>" +
                                       "<td style='border: 1px SOLID #ffffff;' rowspan=" + RowSpanUsuales.ToString() + " align='center' valign='top' bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Costos usuales</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign='top' bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbu.Concepto.Nombre + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign='top' bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbu.Observaciones + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign='top' bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbu.Tasa30) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign='top' bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormat(cpbu.Minimo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign='top' bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormat(cpbu.Maximo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign='top' bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.CalcularGasto(cpbu.Tasa30, Monto, cpbu.Minimo, cpbu.Maximo, ref GastoTotalUsual) + "</font></td>" +
                                       "</tr>";
                        }
                        else
                        {
                            ResponseBody = ResponseBody + "<tr>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign='top' bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbu.Concepto.Nombre + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign='top' bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbu.Observaciones + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign='top' bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbu.Tasa30) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign='top' bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormat(cpbu.Minimo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign='top' bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormat(cpbu.Maximo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign='top' bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.CalcularGasto(cpbu.Tasa30, Monto, cpbu.Minimo, cpbu.Maximo, ref GastoTotalUsual) + "</font></td>" +
                                       "</tr>";
                        }
                        ConteoRowSpanUsuales++;
                    }
                }
                RowSpanEventuales = 0;
                foreach (var cpbe in ListaConceptosProductosBancosEventuales)
                {
                    if (i.Banco.IdBanco == cpbe.IdBanco)
                    {
                        RowSpanEventuales = RowSpanEventuales + 1;
                    }
                }

                foreach (var cpbe in ListaConceptosProductosBancosEventuales)
                {
                    if (i.Banco.IdBanco == cpbe.IdBanco)
                    {
                        if (ConteoRowSpanEventuales == 0)
                        {
                            ResponseBody = ResponseBody + "<tr>" +
                                       "<td style='border: 1px SOLID #ffffff;' rowspan=" + RowSpanEventuales.ToString() + " align='center' valign='top' bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Costos eventuales</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign='top' bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbe.Concepto.Nombre + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign='top' bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbe.Observaciones + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign='top' bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbe.Tasa30) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign='top' bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormat(cpbe.Minimo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign='top' bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormat(cpbe.Maximo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign='top' bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.CalcularGasto(cpbe.Tasa30, Monto, cpbe.Minimo, cpbe.Maximo, ref GastoTotalEventual) + "</font></td>" +
                                       "</tr>";
                        }
                        else
                        {
                            ResponseBody = ResponseBody + "<tr>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign='top' bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbe.Concepto.Nombre + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign='top' bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbe.Observaciones + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign='top' bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbe.Tasa30) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign='top' bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormat(cpbe.Minimo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign='top' bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormat(cpbe.Maximo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign='top' bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.CalcularGasto(cpbe.Tasa30, Monto, cpbe.Minimo, cpbe.Maximo, ref GastoTotalEventual) + "</font></td>" +
                                       "</tr>";
                        }
                        ConteoRowSpanEventuales++;
                    }
                }

                ResponseBody = ResponseBody + "<tr>" +
                               "<td style='border: 1px SOLID #000000;' height='59' align='left' valign=top><font face='Helvetica' size=2><br></font></td>" +
                               "<td style='border: 1px SOLID #000000;' colspan=6 align='center' valign=top bgcolor='#F1F1F1'><font face='Helvetica' size=2 color='#000000'>Datos de contacto: (*) Consulte la fuente de información aqui o tome contacto con " + i.Banco.Nombre + " a través de " + i.Contacto + "</font></td>" +
                               "</tr>";
            }
            ResponseBody = ResponseBody + "</table>" +
                               "</body>" +
                               "</html>";

            return ResponseBody;
        }
    }
}