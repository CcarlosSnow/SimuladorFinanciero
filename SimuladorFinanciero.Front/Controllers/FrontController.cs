using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SimuladorFinanciero.Core;
using SimuladorFinanciero.Entities;
using System.Collections.Specialized;

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

        public ActionResult Paso2(int Tipo, int IdProducto)
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
            ViewBag.ListaMediosDePago = ListaMediosDePago;
            ViewBag.ListaFinanciamiento = ListaFinanciamiento;
            ViewBag.ListaGarantias = ListaGarantias;

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
                    Bancos = oProductoBancoBL.SelectByIdProducto(IdProducto);
                    break;
                case 2:
                    Producto = oProductoBL.Select(IdProducto);
                    Bancos = oProductoBancoBL.SelectByIdProducto(IdProducto);
                    break;
                case 3:
                    Producto = oProductoBL.Select(IdProducto);
                    Bancos = oProductoBancoBL.SelectByIdProducto(IdProducto);
                    break;
                case 4:
                    Producto = oProductoBL.SelectByTipo(4).First();
                    Bancos = oProductoBancoBL.SelectByIdProducto(Producto.IdProducto);
                    MostrarPeriodo = false;
                    break;
            }

            if (Producto.Nombre.StartsWith("1.5") || IdTipo == 4)
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


        public ActionResult ExportExcelRespuesta()
        {
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Contactos " + DateTime.Now.ToString(Formatos.FechaTitleFormat) + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.ContentEncoding = Encoding.Unicode;
            Response.BinaryWrite(Encoding.Unicode.GetPreamble());
            //Response.Charset = ;
            Response.Write("<!DOCTYPE html>");
            Response.Write("<table cellspacing='0' border='0'>" +
                            "<colgroup width='39'></colgroup>" +
                            "<colgroup width='146'></colgroup>" +
                            "<colgroup width='124'></colgroup>" +
                            "<colgroup width='158'></colgroup>" +
                            "<colgroup width='81'></colgroup>" +
                            "<colgroup width='246'></colgroup>" +
                            "<tr><td style='border-bottom: 1px solid #ffffff' colspan=7 height='24' align='left' valign=top bgcolor='#FFFFFF'><b><br></b></td></tr>" +
                            "<tr><td style='background-color: #CF232B;color: white;border-top: 1px solid #ffffff; border-bottom: 1px solid #ffffff' colspan=7 rowspan=2 height='58' align='center' valign=middle bgcolor='#FFFFFF'>" +
                            "<b><font face='Tahoma' style='font-size: 17px'>Simulador financiero " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "</font></b></td></tr>" +
                            "<tr><td style='border-top: 1px solid #ffffff; border-bottom: 1px solid #ffffff' colspan=7 rowspan=2 height='47' align='left' valign=top><br></td></tr>" +
                            "<tr><td colspan='6' style='height: 21px;'></td></tr>" +
                            "<tr>" +
            "<td style='background-color: #7F7F7F;color: white; border-bottom: 1px solid #E2E0E0;border-left: 1px solid #E2E0E0; height='40' align='center' valign=middle bgcolor='#FFFFFF'>" +
                "<b><font face='Tahoma' style='font-size: 15px'>#</font></b>" +
            "</td>" +
            "<td style='background-color: #7F7F7F;color: white; border-bottom: 1px solid #E2E0E0; border-left: 1px solid #E2E0E0;' align='center' valign=middle bgcolor='#FFFFFF'>" +
                "<b><font face='Tahoma' style='font-size: 15px'>Fecha</font></b>" +
            "</td>" +
            "<td style='background-color: #7F7F7F;color: white;border-bottom: 1px solid #E2E0E0;border-left: 1px solid #E2E0E0;' align='center' valign=middle bgcolor='#FFFFFF'>" +
                "<b><font face='Tahoma' style='font-size: 15px'>Nombre</font></b>" +
            "</td>" +
            "<td style='background-color: #7F7F7F;color: white; border-bottom: 1px solid #E2E0E0;border-left: 1px solid #E2E0E0;' align='center' valign=middle bgcolor='#FFFFFF'>" +
                "<b><font face='Tahoma' style='font-size: 15px'>Correo electr&oacute;nico</font></b>" +
            "</td>" +
            "<td style='background-color: #7F7F7F;color: white; border-bottom: 1px solid #E2E0E0; border-left: 1px solid #E2E0E0;' align='center' valign=middle bgcolor='#FFFFFF'>" +
                "<b><font face='Tahoma' style='font-size: 15px'>Asunto</font></b>" +
            "</td>" +
            "<td style='background-color: #7F7F7F;color: white;border-bottom: 1px solid #E2E0E0; border-right: 1px solid #E2E0E0; border-left: 1px solid #E2E0E0;' align='center' valign=middle bgcolor='#FFFFFF'>" +
                "<b><font face='Tahoma' style='font-size: 15px'>Mensaje</font></b>" +
            "</td>" +
            "<td style='background-color: #7F7F7F;color: white;border-bottom: 1px solid #E2E0E0; border-right: 1px solid #E2E0E0; border-left: 1px solid #E2E0E0;' align='center' valign=middle bgcolor='#FFFFFF'>" +
                "<b><font face='Tahoma' style='font-size: 15px'>Estado</font></b>" +
            "</td>" +
            "</tr>");
            int x = 0;
            foreach (var i in Sugerencias)
            {
                x++;
                Response.Write("<tr><td style='border-bottom: 1px solid #E2E0E0; border-left: 1px solid #E2E0E0;'height='89' align='center' valign=middle><font face='Tahoma' style='font-size: 14px'>" + x.ToString() + "</font></td>");
                Response.Write("<td style='border-bottom: 1px solid #E2E0E0; border-left: 1px solid #E2E0E0;'align='left' valign=middle bgcolor='#FFFFFF'><font face='Tahoma' style='font-size: 14px'>" + i.Fecha.ToString(Formatos.FechaFormat) + "</font></td>");
                Response.Write("<td style='border-bottom: 1px solid #E2E0E0; border-left: 1px solid #E2E0E0;'align='left' valign=middle><font face='Tahoma' style='font-size: 14px'>" + i.Nombre + "</font></td>");
                Response.Write("<td style='border-bottom: 1px solid #E2E0E0; border-left: 1px solid #E2E0E0;'align='left' valign=middle><font face='Tahoma' style='font-size: 14px'>" + i.Correo + "</font></td>");
                Response.Write("<td style='border-bottom: 1px solid #E2E0E0; border-left: 1px solid #E2E0E0;'align='left' valign=middle><font face='Tahoma' style='font-size: 14px'>" + i.Parametro.Nombre + "</font></td>");
                Response.Write("<td style='border-bottom: 1px solid #E2E0E0; border-left: 1px solid #E2E0E0;'align='left' valign=middle> <font face='Tahoma' style='font-size: 14px'>" + i.Descripcion + "</font></td>");
                Response.Write("<td style='border-bottom: 1px solid #E2E0E0; border-left: 1px solid #E2E0E0; border-right: 1px solid #E2E0E0;' align='left' valign=middle><font face='Tahoma' style='font-size: 14px'>" + i.Parametro1.Nombre + "</font></td></tr>");
            }
            Response.Write("</table>");
            //StringWriter sw = new StringWriter();
            //HtmlTextWriter htw = new HtmlTextWriter(sw);
            //gv.RenderControl(htw);
            //Response.Output.Write(sw.ToString());

            Response.Flush();
            Response.End();

            return RedirectToAction("Contactos");
        }
    }
}