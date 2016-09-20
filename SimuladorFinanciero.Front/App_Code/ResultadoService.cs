using System;
using System.Collections.Generic;
using System.Linq;
using SimuladorFinanciero.Core;
using SimuladorFinanciero.Entities;
using SimuladorFinanciero.Helpers;
using ClosedXML.Excel;
using System.IO;
using System.Web;

namespace SimuladorFinanciero
{
    public class ResultadoService
    {
        public string GenerarExcelBody(string Tipo, int IdProducto, decimal Monto, int Periodo, string Bancos, string RutaGuardarExcel)
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
            foreach (string i in BancosArray)
            {
                var ProductoBanco = oProductoBancoBL.SelectByIdProductoAndIdBanco(IdProducto, i);
                ListaProductosBancos.Add(ProductoBanco);

                var ConceptosProductosBancosUsuales = oConceptoProductoBancoBL.SelectByProductoAndBancoAndTipoComision(IdProducto, i, "0401", Periodo).Concat(ListaConceptosProductosBancosUsuales);
                ListaConceptosProductosBancosUsuales = ConceptosProductosBancosUsuales.ToList();

                var ConceptosProductosBancosEventuales = oConceptoProductoBancoBL.SelectByProductoAndBancoAndTipoComision(IdProducto, i, "0402", Periodo).Concat(ListaConceptosProductosBancosEventuales);
                ListaConceptosProductosBancosEventuales = ConceptosProductosBancosEventuales.ToList();
            }

            string NombreExcel = "Resultado " + DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");

            var WorkBook = new XLWorkbook();
            var WorkSheet = WorkBook.Worksheets.Add(NombreExcel);
            WorkSheet.Style.Font.SetFontName("Helvetica");
            WorkSheet.Style.Font.SetFontSize(10);
            WorkSheet.Style.Font.SetFontColor(XLColor.Black);
            WorkSheet.Style.Fill.SetBackgroundColor(XLColor.White);
            WorkSheet.Column(1).Width = 15.57;
            WorkSheet.Column(2).Width = 45;
            WorkSheet.Column(3).Width = 45;
            WorkSheet.Column(4).Width = 8.86;
            WorkSheet.Column(5).Width = 10.71;
            WorkSheet.Column(6).Width = 11.43;
            WorkSheet.Column(7).Width = 10.71;
            WorkSheet.Row(1).Height = 28.5;
            WorkSheet.Row(2).Height = 21;
            WorkSheet.Row(3).Height = 21;
            WorkSheet.Row(4).Height = 21;
            WorkSheet.Row(5).Height = 21;
            int FilaActual = 1;
            WorkSheet.Cell("A" + FilaActual.ToString()).Value = "Simulador Financiero";
            FilaActual = FilaActual + 1;
            WorkSheet.Cell("A" + FilaActual.ToString()).Value = "Tarifario actualizado a " + UltimaFechaPublicacion;
            FilaActual = FilaActual + 1;
            WorkSheet.Cell("A" + FilaActual.ToString()).Value = "Producto: " + Tipo + " - " + oProducto.Nombre.Substring(4);
            FilaActual = FilaActual + 1;
            WorkSheet.Cell("A" + FilaActual.ToString()).Value = "Monto: $" + Formatos.ConvertirNumeroFormat(Monto);
            if (Periodo != 0)
            {
                FilaActual = FilaActual + 1;
                WorkSheet.Cell("A" + FilaActual.ToString()).Value = "Periodo: " + Periodo.ToString() + " días";
                var RangeCabecera = WorkSheet.Range("A1:G" + FilaActual.ToString());
                RangeCabecera.Row(1).Merge().Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.RedPigment).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Font.SetFontSize(18).Font.SetFontColor(XLColor.White).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                RangeCabecera.Row(2).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                RangeCabecera.Row(3).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                RangeCabecera.Row(4).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                RangeCabecera.Row(5).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
            }
            else
            {
                var RangeCabecera = WorkSheet.Range("A1:G" + FilaActual.ToString());
                RangeCabecera.Row(1).Merge().Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.RedPigment).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Font.SetFontSize(18).Font.SetFontColor(XLColor.White).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                RangeCabecera.Row(2).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                RangeCabecera.Row(3).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                RangeCabecera.Row(4).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
            }
            FilaActual = FilaActual + 1;
            WorkSheet.Cell("A" + FilaActual.ToString()).Value = "";
            WorkSheet.Cell("A" + FilaActual.ToString()).Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
            WorkSheet.Cell("B" + FilaActual.ToString()).Value = "Banco";
            var RangeBancoCabecera = WorkSheet.Range("B" + FilaActual.ToString() + ":C" + FilaActual.ToString());
            RangeBancoCabecera.Merge().Style.Font.SetBold().Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
            WorkSheet.Cell("D" + FilaActual.ToString()).Value = "Costo Financiero";
            var RangeGastoFinancieroCabecera = WorkSheet.Range("D" + FilaActual.ToString() + ":G" + FilaActual.ToString());
            RangeGastoFinancieroCabecera.Merge().Style.Font.SetBold().Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);

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
                GastoTotalUsual = 0;
                GastoTotalEventual = 0;
                foreach (var cpbu in ListaConceptosProductosBancosUsuales)
                {
                    if (i.Banco.IdBanco == cpbu.IdBanco)
                    {
                        RowSpanUsuales = RowSpanUsuales + 1;
                        Formatos.CalcularGasto(cpbu.Tasa30, Monto, cpbu.Minimo, cpbu.Maximo, ref GastoTotalUsual);
                    }
                }
                FilaActual = FilaActual + 1;
                WorkSheet.Row(FilaActual).Height = 21;
                WorkSheet.Cell("A" + FilaActual.ToString()).Value = "";
                WorkSheet.Cell("A" + FilaActual.ToString()).Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                WorkSheet.Cell("B" + FilaActual.ToString()).Value = i.Banco.Nombre;

                var RangeBanco = WorkSheet.Range("B" + FilaActual.ToString() + ":C" + FilaActual.ToString());
                RangeBanco.Merge().Style.Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.AirForceBlue).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                WorkSheet.Cell("D" + FilaActual.ToString()).Value = "$ " + Formatos.ConvertirNumeroFormat(GastoTotalUsual);
                var RangeGastoFinanciero = WorkSheet.Range("D" + FilaActual.ToString() + ":G" + FilaActual.ToString());
                RangeGastoFinanciero.Merge().Style.Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.AirForceBlue).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);

                FilaActual = FilaActual + 1;
                WorkSheet.Row(FilaActual).Height = 21;
                WorkSheet.Cell("A" + FilaActual.ToString()).Value = "";
                WorkSheet.Cell("A" + FilaActual.ToString()).Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                WorkSheet.Cell("B" + FilaActual.ToString()).Value = "Concepto";
                WorkSheet.Cell("B" + FilaActual.ToString()).Style.Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.SteelBlue).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                WorkSheet.Cell("C" + FilaActual.ToString()).Value = "Observaciones";
                WorkSheet.Cell("C" + FilaActual.ToString()).Style.Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.SteelBlue).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                WorkSheet.Cell("D" + FilaActual.ToString()).Value = "Tasa %";
                WorkSheet.Cell("D" + FilaActual.ToString()).Style.Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.SteelBlue).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                WorkSheet.Cell("E" + FilaActual.ToString()).Value = "Mínimo $";
                WorkSheet.Cell("E" + FilaActual.ToString()).Style.Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.SteelBlue).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                WorkSheet.Cell("F" + FilaActual.ToString()).Value = "Máximo $";
                WorkSheet.Cell("F" + FilaActual.ToString()).Style.Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.SteelBlue).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                WorkSheet.Cell("G" + FilaActual.ToString()).Value = "Gastos $";
                WorkSheet.Cell("G" + FilaActual.ToString()).Style.Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.SteelBlue).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);

                foreach (var cpbu in ListaConceptosProductosBancosUsuales)
                {
                    if (i.Banco.IdBanco == cpbu.IdBanco)
                    {
                        if (ConteoRowSpanUsuales == 0)
                        {
                            GastoTotalUsual = 0;
                            FilaActual = FilaActual + 1;
                            WorkSheet.Cell("A" + FilaActual.ToString()).Value = "Costos Usuales";
                            var RangeCostosUsuales = WorkSheet.Range("A" + FilaActual.ToString() + ":A" + (FilaActual + RowSpanUsuales - 1).ToString());
                            RangeCostosUsuales.Merge().Style.Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.SteelBlue).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                            WorkSheet.Cell("B" + FilaActual.ToString()).Value = cpbu.Concepto.Nombre;
                            WorkSheet.Cell("B" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black).Alignment.SetWrapText();
                            WorkSheet.Cell("C" + FilaActual.ToString()).Value = cpbu.Observaciones;
                            WorkSheet.Cell("C" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black).Alignment.SetWrapText();
                            WorkSheet.Cell("D" + FilaActual.ToString()).Value = Formatos.ConvertirNumeroFormatTasa(cpbu.Tasa30);
                            WorkSheet.Cell("D" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                            WorkSheet.Cell("E" + FilaActual.ToString()).Value = Formatos.ConvertirNumeroFormat(cpbu.Minimo);
                            WorkSheet.Cell("E" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                            WorkSheet.Cell("F" + FilaActual.ToString()).Value = Formatos.ConvertirNumeroFormat(cpbu.Maximo);
                            WorkSheet.Cell("F" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                            WorkSheet.Cell("G" + FilaActual.ToString()).Value = Formatos.CalcularGasto(cpbu.Tasa30, Monto, cpbu.Minimo, cpbu.Maximo, ref GastoTotalUsual);
                            WorkSheet.Cell("G" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                        }
                        else
                        {
                            GastoTotalUsual = 0;
                            FilaActual = FilaActual + 1;
                            WorkSheet.Cell("B" + FilaActual.ToString()).Value = cpbu.Concepto.Nombre;
                            WorkSheet.Cell("B" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black).Alignment.SetWrapText();
                            WorkSheet.Cell("C" + FilaActual.ToString()).Value = cpbu.Observaciones;
                            WorkSheet.Cell("C" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black).Alignment.SetWrapText();
                            WorkSheet.Cell("D" + FilaActual.ToString()).Value = Formatos.ConvertirNumeroFormatTasa(cpbu.Tasa30);
                            WorkSheet.Cell("D" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                            WorkSheet.Cell("E" + FilaActual.ToString()).Value = Formatos.ConvertirNumeroFormat(cpbu.Minimo);
                            WorkSheet.Cell("E" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                            WorkSheet.Cell("F" + FilaActual.ToString()).Value = Formatos.ConvertirNumeroFormat(cpbu.Maximo);
                            WorkSheet.Cell("F" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                            WorkSheet.Cell("G" + FilaActual.ToString()).Value = Formatos.CalcularGasto(cpbu.Tasa30, Monto, cpbu.Minimo, cpbu.Maximo, ref GastoTotalUsual);
                            WorkSheet.Cell("G" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
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
                            GastoTotalEventual = 0;
                            FilaActual = FilaActual + 1;
                            WorkSheet.Cell("A" + FilaActual.ToString()).Value = "Costos Eventuales";
                            var RangeCostosEventuales = WorkSheet.Range("A" + FilaActual.ToString() + ":A" + (FilaActual + RowSpanEventuales - 1).ToString());
                            RangeCostosEventuales.Merge().Style.Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.SteelBlue).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                            WorkSheet.Cell("B" + FilaActual.ToString()).Value = cpbe.Concepto.Nombre;
                            WorkSheet.Cell("B" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black).Alignment.SetWrapText();
                            WorkSheet.Cell("C" + FilaActual.ToString()).Value = cpbe.Observaciones;
                            WorkSheet.Cell("C" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black).Alignment.SetWrapText();
                            WorkSheet.Cell("D" + FilaActual.ToString()).Value = Formatos.ConvertirNumeroFormatTasa(cpbe.Tasa30);
                            WorkSheet.Cell("D" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                            WorkSheet.Cell("E" + FilaActual.ToString()).Value = Formatos.ConvertirNumeroFormat(cpbe.Minimo);
                            WorkSheet.Cell("E" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                            WorkSheet.Cell("F" + FilaActual.ToString()).Value = Formatos.ConvertirNumeroFormat(cpbe.Maximo);
                            WorkSheet.Cell("F" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                            WorkSheet.Cell("G" + FilaActual.ToString()).Value = Formatos.CalcularGasto(cpbe.Tasa30, Monto, cpbe.Minimo, cpbe.Maximo, ref GastoTotalEventual);
                            WorkSheet.Cell("G" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                            
                        }
                        else
                        {
                            GastoTotalEventual = 0;
                            FilaActual = FilaActual + 1;
                            WorkSheet.Cell("B" + FilaActual.ToString()).Value = cpbe.Concepto.Nombre;
                            WorkSheet.Cell("B" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black).Alignment.SetWrapText();
                            WorkSheet.Cell("C" + FilaActual.ToString()).Value = cpbe.Observaciones;
                            WorkSheet.Cell("C" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black).Alignment.SetWrapText();
                            WorkSheet.Cell("D" + FilaActual.ToString()).Value = Formatos.ConvertirNumeroFormatTasa(cpbe.Tasa30);
                            WorkSheet.Cell("D" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                            WorkSheet.Cell("E" + FilaActual.ToString()).Value = Formatos.ConvertirNumeroFormat(cpbe.Minimo);
                            WorkSheet.Cell("E" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                            WorkSheet.Cell("F" + FilaActual.ToString()).Value = Formatos.ConvertirNumeroFormat(cpbe.Maximo);
                            WorkSheet.Cell("F" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                            WorkSheet.Cell("G" + FilaActual.ToString()).Value = Formatos.CalcularGasto(cpbe.Tasa30, Monto, cpbe.Minimo, cpbe.Maximo, ref GastoTotalEventual);
                            WorkSheet.Cell("G" + FilaActual.ToString()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                            
                        }
                        ConteoRowSpanEventuales++;
                    }
                }
                FilaActual = FilaActual + 1;
                WorkSheet.Row(FilaActual).Height = 44.25;
                WorkSheet.Cell("A" + FilaActual.ToString()).Value = "";
                WorkSheet.Cell("A" + FilaActual.ToString()).Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black);
                WorkSheet.Cell("B" + FilaActual.ToString()).Value = "Datos de contacto: (*) Consulte la fuente de información aqui o tome contacto con " + i.Banco.Nombre + " a través de " + i.Contacto;
                var RangeFooterBanco = WorkSheet.Range("B" + FilaActual.ToString() + ":G" + FilaActual.ToString());
                RangeFooterBanco.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Border.SetOutsideBorder(XLBorderStyleValues.Medium).Border.SetOutsideBorderColor(XLColor.Black).Alignment.SetWrapText();
            }
            WorkBook.SaveAs(RutaGuardarExcel);
            return "OK";
        }
    }
}