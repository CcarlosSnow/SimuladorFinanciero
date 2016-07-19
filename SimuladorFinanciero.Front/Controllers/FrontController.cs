using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimuladorFinanciero.Core;
using SimuladorFinanciero.Entities;
using SimuladorFinanciero.Helpers;
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
            var Bancos = oProductoBancoBL.SelectByIdProducto(IdProducto);
            ViewBag.ListaBancos = Bancos;
            switch (Tipo)
            {
                case 1:
                    TipoNombre = "Medios de pago";
                    break;
                case 2:
                    TipoNombre = "Financiamiento";
                    break;
                case 3:
                    TipoNombre = "Garantías";
                    break;
                case 4:
                    TipoNombre = "Envío de dinero";
                    break;
            }
            ViewBag.TipoNombre = TipoNombre;
            ViewBag.Producto = oProductoBL.Select(IdProducto);
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