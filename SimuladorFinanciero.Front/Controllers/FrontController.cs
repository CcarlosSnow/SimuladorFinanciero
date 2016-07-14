using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimuladorFinanciero.Core;
using SimuladorFinanciero.Entities;
using SimuladorFinanciero.Helpers;

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
            return View();
        }

        public JsonResult GuardarContacto(Sugerencia model)
        {
            SugerenciaBL oSugerenciaBL = new SugerenciaBL();
            //Sugerencia oSugerencia = new Sugerencia();
            //oSugerencia.Descripcion = mensaje;
            //oSugerencia.Nombre = nombres;
            //oSugerencia.Correo = correo;
            model.Fecha = DateTime.Now;
            model.Tipo = "0601";
            model.Estado = "0701";
            oSugerenciaBL.Insert(model);
            return Json(new Respuesta { Estado = "OK", Titulo = "Aviso!", Texto = "Los datos fueron registrados correctamente." });
        }
    }
}