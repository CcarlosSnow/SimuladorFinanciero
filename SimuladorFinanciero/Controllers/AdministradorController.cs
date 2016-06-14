using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimuladorFinanciero.Core;

namespace SimuladorFinanciero.Controllers
{
    public class AdministradorController : Controller
    {
        // GET: Administrador
        ArchivoBL oArchivoBL = new ArchivoBL();
        public ActionResult ListaArchivos()
        {
            return View(oArchivoBL.SelectAll());
        }

        public ActionResult SubirArchivo()
        {
            return View();
        }

        [HttpPost]
        public void Subir(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return;
            }
            string Nombre = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".xlsx";

            file.SaveAs(Server.MapPath("~/ArchivosExcel/" + Nombre));
        }
    }
}
