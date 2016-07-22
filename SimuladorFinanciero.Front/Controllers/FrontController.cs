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
        // GET: Front
        public ActionResult Index()
        {
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
            NameValueCollection Form = Request.Form;
            double Monto = double.Parse(Form["txtMonto"]);
            int IdProducto = int.Parse(Form["hdIdProducto"]);
            string TipoNombre = Form["hdTipoNombre"];
            string Bancos = Form["hdBancos"];
            string[] BancosArray = Bancos.Split(',');

            ProductoBancoBL oProductoBancoBL = new ProductoBancoBL();
            List<ProductoBanco> ListaProductosBancos = new List<ProductoBanco>();
            List<ConceptoProductoBanco> ListaConceptosProductosBancosUsuales = new List<ConceptoProductoBanco>();
            List<ConceptoProductoBanco> ListaConceptosProductosBancosEventuales = new List<ConceptoProductoBanco>();
            ConceptoProductoBancoBL oConceptoProductoBancoBL = new ConceptoProductoBancoBL();
            foreach (string i in BancosArray)
            {
                var ProductoBanco = oProductoBancoBL.SelectByIdProductoAndIdBanco(IdProducto, i);
                ListaProductosBancos.Add(ProductoBanco);

                var ConceptosProductosBancosUsuales = oConceptoProductoBancoBL.SelectByProductoAndBancoAndTipoComision(IdProducto, i, "0401").Concat(ListaConceptosProductosBancosUsuales);
                ListaConceptosProductosBancosUsuales = ConceptosProductosBancosUsuales.ToList();

                var ConceptosProductosBancosEventuales = oConceptoProductoBancoBL.SelectByProductoAndBancoAndTipoComision(IdProducto, i, "0402").Concat(ListaConceptosProductosBancosEventuales);
                ListaConceptosProductosBancosEventuales = ConceptosProductosBancosEventuales.ToList();
            }

            ProductoBL oProductoBL = new ProductoBL();
            var Producto = oProductoBL.Select(IdProducto);
            ViewBag.TipoNombre = TipoNombre;
            ViewBag.Producto = Producto;
            ViewBag.Monto = Monto;
            ViewBag.ListaProductosBancos = ListaProductosBancos;
            ViewBag.ListaConceptosProductosBancosUsuales = ListaConceptosProductosBancosUsuales;
            ViewBag.ListaConceptosProductosBancosEventuales = ListaConceptosProductosBancosEventuales;
            return View();
        }
    }
}