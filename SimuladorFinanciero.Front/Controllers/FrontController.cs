using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SimuladorFinanciero.Core;
using SimuladorFinanciero.Entities;
using System.Collections.Specialized;
using System.Text;
using SimuladorFinanciero.Helpers;
using System.Net.Mail;
using System.Net;
using System.IO;

namespace SimuladorFinanciero.Front.Controllers
{
    public class FrontController : Controller
    {
        ArchivoBL oArchivoBL = new ArchivoBL();
        public ActionResult Index()
        {
            ViewBag.UltimaFechaPublicacion = oArchivoBL.SelectActive().Fecha.ToLongDateString();
            ProductoBL oProductoBL = new ProductoBL();
            IList<Producto> ListaMediosDePago = oProductoBL.SelectByTipo(1);
            IList<Producto> ListaFinanciamiento = oProductoBL.SelectByTipo(2);
            IList<Producto> ListaGarantias = oProductoBL.SelectByTipo(3);
            IList<Producto> ListaEnvioDinero = oProductoBL.SelectByTipo(4);
            ViewBag.ListaMediosDePago = ListaMediosDePago;
            ViewBag.ListaFinanciamiento = ListaFinanciamiento;
            ViewBag.ListaGarantias = ListaGarantias;
            return View();
        }

        public ActionResult Contacto()
        {
            ViewBag.UltimaFechaPublicacion = oArchivoBL.SelectActive().Fecha.ToLongDateString();
            ParametroBL oParametroBL = new ParametroBL();
            var Tipos = oParametroBL.SelectByStart("06");
            ViewBag.Tipos = Tipos;
            return View();
        }

        public JsonResult GuardarContacto(Sugerencia model)
        {
            SugerenciaBL oSugerenciaBL = new SugerenciaBL();
            model.Fecha = DateTime.Now;
            model.Estado = "0701";
            oSugerenciaBL.Insert(model);
            return Json(new Respuesta { Estado = "OK", Titulo = "Aviso!", Texto = "Los datos fueron registrados correctamente." });
        }

        public ActionResult Paso2(int Tipo, int IdProducto, string Numero)
        {
            ViewBag.UltimaFechaPublicacion = oArchivoBL.SelectActive().Fecha.ToLongDateString();
            ProductoBancoBL oProductoBancoBL = new ProductoBancoBL();
            ProductoBL oProductoBL = new ProductoBL();
            string TipoNombre = "";
            string BancoEmpresa = "";
            string MensajeBancoEmpresaJS = "";
            string MensajeBancoEmpresaPopUp = "";
            Producto Producto = null;
            List<Banco> Bancos = null;
            bool MostrarPeriodo = true;
            switch (Tipo)
            {
                case 1:
                    TipoNombre = "Medios de pago";
                    Producto = oProductoBL.Select(IdProducto);
                    Bancos = oProductoBancoBL.SelectByIdProducto(IdProducto);
                    break;
                case 2:
                    Producto = oProductoBL.Select(IdProducto);
                    Bancos = oProductoBancoBL.SelectByIdProducto(IdProducto);
                    TipoNombre = "Financiamiento";
                    break;
                case 3:
                    Producto = oProductoBL.Select(IdProducto);
                    Bancos = oProductoBancoBL.SelectByIdProducto(IdProducto);
                    TipoNombre = "Garantías";
                    break;
                case 4:
                    Producto = oProductoBL.SelectByTipo(4).First();
                    Bancos = oProductoBancoBL.SelectByIdProducto(Producto.IdProducto);
                    TipoNombre = "Envío de dinero";
                    MostrarPeriodo = false;
                    break;
            }

            if (Producto.Nombre.StartsWith("1.5") || IdProducto == 0)
            {
                BancoEmpresa = "Empresa";
                MensajeBancoEmpresaJS = "una Empresa";
                MensajeBancoEmpresaPopUp = "una o más Empresas";
                MostrarPeriodo = false;
            }
            else
            {
                BancoEmpresa = "Banco";
                MensajeBancoEmpresaJS = "un Banco";
                MensajeBancoEmpresaPopUp = "uno o más bancos";
            }

            ViewBag.IdTipo = Tipo;
            ViewBag.TipoNombre = TipoNombre;
            ViewBag.Producto = Producto;
            ViewBag.ListaBancos = Bancos;
            ViewBag.BancoEmpresa = BancoEmpresa;
            ViewBag.MensajeBancoEmpresaJS = MensajeBancoEmpresaJS;
            ViewBag.MensajeBancoEmpresaPopUp = MensajeBancoEmpresaPopUp;
            ViewBag.MostrarPeriodo = MostrarPeriodo;
            return View();
        }

        public ActionResult Resultado()
        {
            ViewBag.UltimaFechaPublicacion = oArchivoBL.SelectActive().Fecha.ToLongDateString();
            NameValueCollection Form = Request.Form;
            double Monto = double.Parse(Form["txtMonto"]);
            int IdProducto = int.Parse(Form["hdIdProducto"]);
            int IdTipo = int.Parse(Form["hdIdTipo"]);
            string TipoNombre = Form["hdTipoNombre"];
            string BancosString = Form["hdBancos"];
            int Periodo = int.Parse(Form["hdPeriodo"]);
            string[] BancosArray = BancosString.Split(',');

            BancoBL oBancoBL = new BancoBL();
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

            ProductoBL oProductoBL = new ProductoBL();
            ViewBag.TipoNombre = TipoNombre;
            ViewBag.Monto = Monto;
            ViewBag.Periodo = Periodo;
            ViewBag.ListaProductosBancos = ListaProductosBancos;
            ViewBag.ListaConceptosProductosBancosUsuales = ListaConceptosProductosBancosUsuales;
            ViewBag.ListaConceptosProductosBancosEventuales = ListaConceptosProductosBancosEventuales;

            IList<Producto> ListaMediosDePago = oProductoBL.SelectByTipo(1);
            IList<Producto> ListaFinanciamiento = oProductoBL.SelectByTipo(2);
            IList<Producto> ListaGarantias = oProductoBL.SelectByTipo(3);
            IList<Producto> ListaEnvioDinero = oProductoBL.SelectByTipo(4);
            ViewBag.ListaMediosDePago = ListaMediosDePago;
            ViewBag.ListaFinanciamiento = ListaFinanciamiento;
            ViewBag.ListaGarantias = ListaGarantias;
            ViewBag.ListaEnvioDinero = ListaEnvioDinero;

            string BancoEmpresa = "";
            string MensajeBancoEmpresaJS = "";
            string MensajeBancoEmpresaPopUp = "";
            Producto Producto = null;
            List<Banco> Bancos = null;
            bool MostrarPeriodo = true;
            switch (IdTipo)
            {
                case 1:
                    Producto = oProductoBL.Select(IdProducto);
                    //Bancos = oProductoBancoBL.SelectByIdProducto(IdProducto);
                    break;
                case 2:
                    Producto = oProductoBL.Select(IdProducto);
                    //Bancos = oProductoBancoBL.SelectByIdProducto(IdProducto);
                    break;
                case 3:
                    Producto = oProductoBL.Select(IdProducto);
                    //Bancos = oProductoBancoBL.SelectByIdProducto(IdProducto);
                    break;
                case 4:
                    Producto = oProductoBL.SelectByTipo(4).First();
                    //Bancos = oProductoBancoBL.SelectByIdProducto(Producto.IdProducto);
                    MostrarPeriodo = false;
                    break;
            }

            Bancos = oBancoBL.SelectAll().ToList();

            if (Producto.Nombre.StartsWith("1.5") || IdTipo == 4)
            {
                BancoEmpresa = "empresa";
                MensajeBancoEmpresaJS = "una empresa";
                MensajeBancoEmpresaPopUp = "una o más empresas";
                MostrarPeriodo = false;
            }
            else
            {
                BancoEmpresa = "banco";
                MensajeBancoEmpresaJS = "un banco";
                MensajeBancoEmpresaPopUp = "uno o más bancos";
            }

            ViewBag.IdTipo = IdTipo;
            ViewBag.TipoNombre = TipoNombre;
            ViewBag.IdProducto = IdProducto;
            ViewBag.Producto = Producto;
            ViewBag.ListaBancos = Bancos;
            ViewBag.BancosString = BancosString;
            ViewBag.BancoEmpresa = BancoEmpresa;
            ViewBag.BancosArray = BancosArray;
            ViewBag.MensajeBancoEmpresaJS = MensajeBancoEmpresaJS;
            ViewBag.MensajeBancoEmpresaPopUp = MensajeBancoEmpresaPopUp;
            ViewBag.MostrarPeriodo = MostrarPeriodo;
            return View();
        }

        public ActionResult ExportExcelRespuesta(string Tipo, int IdProducto, decimal Monto, int Periodo, string Bancos)
        {
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

            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Resultados " + DateTime.Now.ToString(Formatos.FechaTitleFormat) + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.ContentEncoding = Encoding.Unicode;
            Response.BinaryWrite(Encoding.Unicode.GetPreamble());
            //Response.Charset = ;
            string ResponseBody = "<html><head><meta http-equiv='content - type' content='text / html; charset = utf - 8'/></head><body>" +
                                  "<table cellspacing='0' border='0'>" +
                                  "<tr>" +
                                  "<td style='border: 1px SOLID #000000;' colspan=7 height='38' align='center' valign='top' bgcolor='#CC0000'>" +
                                  "<b style='color: white; '>" +
                                  "<font face='Helvetica' size=5>Simulador Financiero</font>" +
                                  "</b>" +
                                  "</td>" +
                                  "</tr>" +
                                  "<td style='border: 1px SOLID #000000;' colspan=7 height='27' align='center' valign=top bgcolor='#FFFFFF'><font face='Helvetica' size=2>Resultados - " + DateTime.Now.ToLongDateString() + "</font></td>" +
                                  "</tr>" +
                                  "<tr>" +
                                  "<td style='border: 1px SOLID #000000;' colspan=7 height='27' align='left' valign=top bgcolor='#F1F1F1'><font face='Helvetica' size=2>Producto: " + Tipo + "-" + oProducto.Nombre.Substring(4) + "</font></td>" +
                                  "</tr>" +
                                  "<tr>" +
                                  "<td style='border: 1px SOLID #000000;' colspan=7 height='27' align='left' valign=top bgcolor='#DEDEDE'><font face='Helvetica' size=2>Monto: $" + Monto.ToString() + "</font></td>" +
                                  "</tr>" +
                                  "<tr>" +
                                  "<td style='border: 1px SOLID #000000;' colspan=7 height='27' align='left' valign=top bgcolor='#F1F1F1'><font face='Helvetica' size=2>Periodo: " + Periodo.ToString() + " días </font></td>" +
                                  "</tr>" +
                                  "<tr>" +
                                  "<td style='border: 1px SOLID #000000;' height='28' align='left' valign=top><font face='Helvetica' size=2><br></font></td>" +
                                  "<td style='border: 1px SOLID #000000; border-bottom: 1px SOLID #ffffff;' colspan=2 align='center' valign=middle><b><font face='Helvetica' size=2>Banco</font></b></td>" +
                                  "<td style='border: 1px SOLID #000000; border-bottom: 1px SOLID #ffffff;' colspan=4 align='center' valign=middle><b><font face='Helvetica' size=2>Gasto Financiero</font></b></td>" +
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
                ResponseBody = ResponseBody + "<tr>" +
                               "<td style='border: 1px SOLID #000000; border-right: 1px SOLID #ffffff;' height='28' align='left' valign=top><font face='Helvetica' size=2><br></font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' colspan=2 align='center' valign=middle bgcolor='#165778'><font face='Helvetica' size=2 color='#FFFFFF'>" + i.Banco.Nombre + "</font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' colspan=4 align='center' valign=middle bgcolor='#165778'><font face='Helvetica' size=2 color='#FFFFFF'>$ " + Formatos.ConvertirNumeroFormat(GastoTotalUsual) + "</font></td>" +
                               "</tr>" +
                               "<tr>" +
                               "<td style='border: 1px SOLID #000000; border-right: 1px SOLID #ffffff;' height='28' align='left' valign=top><font face='Helvetica' size=2><br></font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' align='center' valign=middle bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Concepto</font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' align='center' valign=middle bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Observaciones</font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' align='center' valign=middle bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Tasa %</font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' align='center' valign=middle bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Mínimo $</font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' align='center' valign=middle bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Máximo $</font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' align='center' valign=middle bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Gastos $</font></td>" +
                               "</tr>";
                RowSpanUsuales = 0;
                foreach (var cpbu in ListaConceptosProductosBancosUsuales)
                {
                    if (i.Banco.IdBanco == cpbu.IdBanco)
                    {
                        RowSpanUsuales = RowSpanUsuales + 1;
                    }
                }

                foreach (var cpbu in ListaConceptosProductosBancosUsuales)
                {
                    if (i.Banco.IdBanco == cpbu.IdBanco)
                    {
                        if (ConteoRowSpanUsuales == 0)
                        {
                            ResponseBody = ResponseBody + "<tr>" +
                                       "<td style='border: 1px SOLID #ffffff;' rowspan=" + RowSpanUsuales.ToString() + " height='226' align='center' valign=middle bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Costos usuales</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbu.Concepto.Nombre + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbu.Observaciones + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbu.Tasa30) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbu.Minimo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbu.Maximo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.CalcularGasto(cpbu.Tasa30, Monto, cpbu.Minimo, cpbu.Maximo, ref GastoTotalEventual) + "</font></td>" +
                                       "</tr>";

                        }
                        else
                        {
                            ResponseBody = ResponseBody + "<tr>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbu.Concepto.Nombre + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbu.Observaciones + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbu.Tasa30) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbu.Minimo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbu.Maximo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.CalcularGasto(cpbu.Tasa30, Monto, cpbu.Minimo, cpbu.Maximo, ref GastoTotalEventual) + "</font></td>" +
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
                                       "<td style='border: 1px SOLID #ffffff;' rowspan=" + RowSpanEventuales.ToString() + " height='226' align='center' valign=middle bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Costos eventuales</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbe.Concepto.Nombre + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbe.Observaciones + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbe.Tasa30) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbe.Minimo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbe.Maximo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.CalcularGasto(cpbe.Tasa30, Monto, cpbe.Minimo, cpbe.Maximo, ref GastoTotalEventual) + "</font></td>" +
                                       "</tr>";
                        }
                        else
                        {
                            ResponseBody = ResponseBody + "<tr>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbe.Concepto.Nombre + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbe.Observaciones + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbe.Tasa30) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbe.Minimo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbe.Maximo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.CalcularGasto(cpbe.Tasa30, Monto, cpbe.Minimo, cpbe.Maximo, ref GastoTotalEventual) + "</font></td>" +
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

            //StringWriter sw = new StringWriter();
            //HtmlTextWriter htw = new HtmlTextWriter(sw);
            //gv.RenderControl(htw);
            //Response.Output.Write(sw.ToString());
            Response.Write(ResponseBody);
            Response.Flush();
            Response.End();

            return RedirectToAction("Resultado");
        }


        public ActionResult EnviarEmail(string Para, string Tipo, int IdProducto, decimal Monto, int Periodo, string Bancos)
        {
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

            //Response.Clear();
            //Response.ClearContent();
            //Response.ClearHeaders();
            //Response.Buffer = true;
            //Response.AddHeader("content-disposition", "attachment; filename=Resultados " + DateTime.Now.ToString(Formatos.FechaTitleFormat) + ".xls");
            //Response.ContentType = "application/ms-excel";
            //Response.ContentEncoding = Encoding.Unicode;
            //Response.BinaryWrite(Encoding.Unicode.GetPreamble());
            //Response.Charset = ;
            string ResponseBody = "<!DOCTYPE html><html><head><meta http-equiv='content - type' content='text / html; charset = utf - 8'/></head><body>" +
                                  "<table cellspacing='0' border='0'>" +
                                  "<tr>" +
                                  "<td style='border: 1px SOLID #000000;' colspan=7 height='38' align='center' valign='top' bgcolor='#CC0000'>" +
                                  "<b style='color: white; '>" +
                                  "<font face='Helvetica' size=5>Simulador Financiero</font>" +
                                  "</b>" +
                                  "</td>" +
                                  "</tr>" +
                                  "<td style='border: 1px SOLID #000000;' colspan=7 height='27' align='center' valign=top bgcolor='#FFFFFF'><font face='Helvetica' size=2>Resultados - " + DateTime.Now.ToLongDateString() + "</font></td>" +
                                  "</tr>" +
                                  "<tr>" +
                                  "<td style='border: 1px SOLID #000000;' colspan=7 height='27' align='left' valign=top bgcolor='#F1F1F1'><font face='Helvetica' size=2>Producto: " + Tipo + "-" + oProducto.Nombre.Substring(4) + "</font></td>" +
                                  "</tr>" +
                                  "<tr>" +
                                  "<td style='border: 1px SOLID #000000;' colspan=7 height='27' align='left' valign=top bgcolor='#DEDEDE'><font face='Helvetica' size=2>Monto: $" + Monto.ToString() + "</font></td>" +
                                  "</tr>" +
                                  "<tr>" +
                                  "<td style='border: 1px SOLID #000000;' colspan=7 height='27' align='left' valign=top bgcolor='#F1F1F1'><font face='Helvetica' size=2>Periodo: " + Periodo.ToString() + " días </font></td>" +
                                  "</tr>" +
                                  "<tr>" +
                                  "<td style='border: 1px SOLID #000000;' height='28' align='left' valign=top><font face='Helvetica' size=2><br></font></td>" +
                                  "<td style='border: 1px SOLID #000000; border-bottom: 1px SOLID #ffffff;' colspan=2 align='center' valign=middle><b><font face='Helvetica' size=2>Banco</font></b></td>" +
                                  "<td style='border: 1px SOLID #000000; border-bottom: 1px SOLID #ffffff;' colspan=4 align='center' valign=middle><b><font face='Helvetica' size=2>Gasto Financiero</font></b></td>" +
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
                ResponseBody = ResponseBody + "<tr>" +
                               "<td style='border: 1px SOLID #000000; border-right: 1px SOLID #ffffff;' height='28' align='left' valign=top><font face='Helvetica' size=2><br></font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' colspan=2 align='center' valign=middle bgcolor='#165778'><font face='Helvetica' size=2 color='#FFFFFF'>" + i.Banco.Nombre + "</font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' colspan=4 align='center' valign=middle bgcolor='#165778'><font face='Helvetica' size=2 color='#FFFFFF'>$ " + Formatos.ConvertirNumeroFormat(GastoTotalUsual) + "</font></td>" +
                               "</tr>" +
                               "<tr>" +
                               "<td style='border: 1px SOLID #000000; border-right: 1px SOLID #ffffff;' height='28' align='left' valign=top><font face='Helvetica' size=2><br></font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' align='center' valign=middle bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Concepto</font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' align='center' valign=middle bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Observaciones</font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' align='center' valign=middle bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Tasa %</font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' align='center' valign=middle bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Mínimo $</font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' align='center' valign=middle bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Máximo $</font></td>" +
                               "<td style='border: 1px SOLID #ffffff;' align='center' valign=middle bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Gastos $</font></td>" +
                               "</tr>";
                RowSpanUsuales = 0;
                foreach (var cpbu in ListaConceptosProductosBancosUsuales)
                {
                    if (i.Banco.IdBanco == cpbu.IdBanco)
                    {
                        RowSpanUsuales = RowSpanUsuales + 1;
                    }
                }

                foreach (var cpbu in ListaConceptosProductosBancosUsuales)
                {
                    if (i.Banco.IdBanco == cpbu.IdBanco)
                    {
                        if (ConteoRowSpanUsuales == 0)
                        {
                            ResponseBody = ResponseBody + "<tr>" +
                                       "<td style='border: 1px SOLID #ffffff;' rowspan=" + RowSpanUsuales.ToString() + " height='226' align='center' valign=middle bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Costos usuales</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbu.Concepto.Nombre + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbu.Observaciones + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbu.Tasa30) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbu.Minimo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbu.Maximo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.CalcularGasto(cpbu.Tasa30, Monto, cpbu.Minimo, cpbu.Maximo, ref GastoTotalEventual) + "</font></td>" +
                                       "</tr>";

                        }
                        else
                        {
                            ResponseBody = ResponseBody + "<tr>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbu.Concepto.Nombre + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbu.Observaciones + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbu.Tasa30) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbu.Minimo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbu.Maximo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.CalcularGasto(cpbu.Tasa30, Monto, cpbu.Minimo, cpbu.Maximo, ref GastoTotalEventual) + "</font></td>" +
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
                                       "<td style='border: 1px SOLID #ffffff;' rowspan=" + RowSpanEventuales.ToString() + " height='226' align='center' valign=middle bgcolor='#217BAA'><font face='Helvetica' size=2 color='#FFFFFF'>Costos eventuales</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbe.Concepto.Nombre + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbe.Observaciones + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbe.Tasa30) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbe.Minimo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbe.Maximo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.CalcularGasto(cpbe.Tasa30, Monto, cpbe.Minimo, cpbe.Maximo, ref GastoTotalEventual) + "</font></td>" +
                                       "</tr>";
                        }
                        else
                        {
                            ResponseBody = ResponseBody + "<tr>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbe.Concepto.Nombre + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='left' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + cpbe.Observaciones + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbe.Tasa30) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbe.Minimo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.ConvertirNumeroFormatTasa(cpbe.Maximo) + "</font></td>" +
                                       "<td style='border: 1px SOLID #000000;' align='center' valign=middle bgcolor='#F1F1F1' sdval='30' sdnum='1033;'><font face='Helvetica' size=2>" + Formatos.CalcularGasto(cpbe.Tasa30, Monto, cpbe.Minimo, cpbe.Maximo, ref GastoTotalEventual) + "</font></td>" +
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


            string NombreXLS = "Resultado " + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".xls";
            string RutaArchivoXLS = Path.Combine(Server.MapPath(ConstantesHelpers.RutaArchivosExcel), NombreXLS);
            System.IO.File.WriteAllText(RutaArchivoXLS, ResponseBody);

            MailMessage msg = new MailMessage();

            msg.From = new MailAddress(ConstantesHelpers.EmailEnvioCorreo());
            msg.To.Add(Para);
            msg.Subject = "Simulador Financiero";
            msg.Body = "Se envían los Resultados " + DateTime.Now.ToLongDateString(); ;
            msg.IsBodyHtml = true;
            msg.Attachments.Add(new Attachment(RutaArchivoXLS));

            //System.Net.Mail.Attachment attachment;
            //attachment = new System.Net.Mail.Attachment(???);
            //msg.Attachments.Add(attachment);

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(ConstantesHelpers.EmailEnvioCorreo(), ConstantesHelpers.PassEnvioCorreo());
            client.EnableSsl = true;

            try
            {
                client.Send(msg);
                return Json(new Respuesta { Estado = "OK", Titulo = "Aviso!", Texto = "Email enviado correctamente" });
            }
            catch (Exception)
            {
                return Json(new Respuesta { Estado = "Error", Titulo = "Aviso!", Texto = "Ocurrió un error al enviar el email, por favor inténtelo más tarde" });
            }
            //return Json(new Respuesta { Estado = "OK", Titulo = "Aviso!", Texto = "Email enviado correctamente" });
        }
    }
}