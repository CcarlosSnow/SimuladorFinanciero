﻿using System;
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
using System.Net.Http;
using System.Web.Helpers;
using System.Web;

namespace SimuladorFinanciero.Front.Controllers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AjaxValidateAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest()) // if it is ajax request.
                {
                    this.ValidateRequestHeader(filterContext.HttpContext.Request); // run the validation.
                }
                else
                {
                    AntiForgery.Validate();
                }
            }
            catch (HttpAntiForgeryException e)
            {
                throw new HttpAntiForgeryException("Anti forgery token not found");
            }
        }

        private void ValidateRequestHeader(HttpRequestBase request)
        {
            string cookieToken = string.Empty;
            string formToken = string.Empty;
            string tokenValue = request.Headers["RequestVerificationToken"]; // read the header key and validate the tokens.
            if (!string.IsNullOrEmpty(tokenValue))
            {
                string[] tokens = tokenValue.Split(':');
                if (tokens.Length == 2)
                {
                    cookieToken = tokens[0].Trim();
                    formToken = tokens[1].Trim();
                }
            }

            AntiForgery.Validate(cookieToken, formToken); // this validates the request token.
        }
    }

    public class FrontController : Controller
    {
        ArchivoBL oArchivoBL = new ArchivoBL();
        public ActionResult Index()
        {
            ViewBag.UltimaFechaPublicacion = Formatos.ConvertirFechaFormatPiePagina(oArchivoBL.SelectActive().Fecha);
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
            ViewBag.UltimaFechaPublicacion = Formatos.ConvertirFechaFormatPiePagina(oArchivoBL.SelectActive().Fecha);
            ParametroBL oParametroBL = new ParametroBL();
            var Tipos = oParametroBL.SelectByStart("06");
            ViewBag.Tipos = Tipos;
            return View();
        }

        [AjaxValidateAntiForgeryToken]
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
            ViewBag.UltimaFechaPublicacion = Formatos.ConvertirFechaFormatPiePagina(oArchivoBL.SelectActive().Fecha);
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

            if (Producto.Nombre.StartsWith("1.1") || Producto.Nombre.StartsWith("1.2") || Producto.Nombre.StartsWith("1.3"))
            {
                MostrarPeriodo = false;
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
            ViewBag.UltimaFechaPublicacion = Formatos.ConvertirFechaFormatPiePagina(oArchivoBL.SelectActive().Fecha);
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
            string BancoEmpresaTabla = "";
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
                BancoEmpresaTabla = "Empresa";
                MostrarPeriodo = false;
            }
            else
            {
                BancoEmpresa = "banco";
                MensajeBancoEmpresaJS = "un banco";
                MensajeBancoEmpresaPopUp = "uno o más bancos";
                BancoEmpresaTabla = "Banco";
            }

            if (Producto.Nombre.StartsWith("1.1") || Producto.Nombre.StartsWith("1.2") || Producto.Nombre.StartsWith("1.3"))
            {
                MostrarPeriodo = false;
            }

            ViewBag.IdTipo = IdTipo;
            ViewBag.TipoNombre = TipoNombre;
            ViewBag.IdProducto = IdProducto;
            ViewBag.Producto = Producto;
            ViewBag.ListaBancos = Bancos;
            ViewBag.BancosString = BancosString;
            ViewBag.BancoEmpresa = BancoEmpresa;
            ViewBag.BancoEmpresaTabla = BancoEmpresaTabla;
            ViewBag.BancosArray = BancosArray;
            ViewBag.MensajeBancoEmpresaJS = MensajeBancoEmpresaJS;
            ViewBag.MensajeBancoEmpresaPopUp = MensajeBancoEmpresaPopUp;
            ViewBag.MostrarPeriodo = MostrarPeriodo;
            return View();
        }

        public FileResult ExportExcelRespuesta(string Tipo, int IdProducto, decimal Monto, int Periodo, string Bancos)
        {
            string NombreXLS = "Resultado " + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".xlsx";
            string RutaArchivoXLS = Path.Combine(Server.MapPath(ConstantesHelpers.RutaArchivosExcel), NombreXLS);

            ResultadoService oResultadoService = new ResultadoService();

            string ResponseBody = oResultadoService.GenerarExcelBody(Tipo, IdProducto, Monto, Periodo, Bancos, RutaArchivoXLS);

            byte[] fileBytes = System.IO.File.ReadAllBytes(RutaArchivoXLS);
            return File(fileBytes, "application/ms-excel", NombreXLS);
        }


        public ActionResult EnviarEmail(string Para, string Tipo, int IdProducto, decimal Monto, int Periodo, string Bancos)
        {
            ResultadoService oResultadoService = new ResultadoService();


            string NombreXLS = "Resultado " + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".xlsx";
            string RutaArchivoXLS = Path.Combine(Server.MapPath(ConstantesHelpers.RutaArchivosExcel), NombreXLS);

            string ResponseBody = oResultadoService.GenerarExcelBody(Tipo, IdProducto, Monto, Periodo, Bancos, RutaArchivoXLS);

            MailMessage msg = new MailMessage();

            msg.From = new MailAddress(ConstantesHelpers.EmailEnvioCorreo());
            msg.To.Add(Para);
            msg.Subject = "Simulador Financiero";
            msg.Body = "Se envían los Resultados " + DateTime.Now.ToLongDateString(); ;
            msg.IsBodyHtml = true;
            msg.Attachments.Add(new Attachment(RutaArchivoXLS));

            SmtpClient client = new SmtpClient(ConstantesHelpers.SMTP(), int.Parse(ConstantesHelpers.PuertoSMTP()));
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = ConstantesHelpers.UseDefaultCredentials();
            client.Credentials = new NetworkCredential(ConstantesHelpers.EmailEnvioCorreo(), ConstantesHelpers.PassEnvioCorreo());
            client.EnableSsl = ConstantesHelpers.EnableSSL();

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

        public ActionResult Error404()
        {
            ViewBag.UltimaFechaPublicacion = Formatos.ConvertirFechaFormatPiePagina(oArchivoBL.SelectActive().Fecha);
            return View();
        }

        public ActionResult Error500()
        {
            ViewBag.UltimaFechaPublicacion = Formatos.ConvertirFechaFormatPiePagina(oArchivoBL.SelectActive().Fecha);
            return View();
        }
    }
}