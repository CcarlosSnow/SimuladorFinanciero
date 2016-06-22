using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimuladorFinanciero.Entities;
using SimuladorFinanciero.Core;
using System.IO;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using SimuladorFinanciero.Helpers;
using Bytescout.Spreadsheet;

namespace SimuladorFinanciero.Controllers
{
    public class AdministradorController : Controller
    {
        // GET: Administrador
        ArchivoBL oArchivoBL = new ArchivoBL();
        SubirArchivoService oSubirArchivoService = new SubirArchivoService();
        public ActionResult ListaArchivos()
        {
            return View(oArchivoBL.SelectAll());
        }

        public ActionResult SubirArchivo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Subir()
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        string Extension = System.IO.Path.GetExtension(file.FileName);
                        if (Extension != ".xls" && Extension != ".xlsx")
                        {
                            return Json(new Respuesta { Estado = "Error", Titulo = "Aviso!", Texto = "El archivo debe ser de tipo XLS o XLSX" });
                        }
                        string NombreXLS = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + Extension;
                        string NombreTXT = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".txt";
                        string RutaXLS = Path.Combine(Server.MapPath(ConstantesLocal.RutaArchivosExcel), NombreXLS);
                        string RutaTXT = Path.Combine(Server.MapPath(ConstantesLocal.RutaArchivosExcel), NombreTXT);
                        file.SaveAs(RutaXLS);

                        FileStream FileStr = new FileStream(RutaXLS, FileMode.Open);

                        Spreadsheet oSpreadsheet = new Spreadsheet();
                        oSpreadsheet.LoadFromStream(FileStr);
                        oSpreadsheet.Workbook.Worksheets[0].SaveAsTXT(Path.Combine(Server.MapPath(ConstantesLocal.RutaArchivosExcel), NombreTXT));

                        Enumerators.RespuestaCargaExcel RespuestaCargaExcel;

                        RespuestaCargaExcel = oSubirArchivoService.CargarExcelDataBase(NombreXLS, NombreTXT, Extension, RutaXLS, RutaTXT, "NUEVO");

                        switch (RespuestaCargaExcel)
                        {
                            case Enumerators.RespuestaCargaExcel.Correcto:
                                return Json(new Respuesta { Estado = "OK", Titulo = "Aviso!", Texto = "Archivo cargado orrectamente" });

                            case Enumerators.RespuestaCargaExcel.ArchivoNoExiste:
                                return Json(new Respuesta { Estado = "Error", Titulo = "Aviso!", Texto = "Archivo no existe" });

                            case Enumerators.RespuestaCargaExcel.ExcelVacio:
                                return Json(new Respuesta { Estado = "Error", Titulo = "Aviso!", Texto = "El archivo excel se encuentra vacío" });

                            case Enumerators.RespuestaCargaExcel.Error:
                                return Json(new Respuesta { Estado = "Error", Titulo = "Aviso!", Texto = "Error al cargar el archivo" });

                            default:
                                return Json(new Respuesta { Estado = "OK", Titulo = "Aviso!", Texto = "Respuesta por defecto" });
                        }
                    }
                    else
                    {
                        return Json(new Respuesta { Estado = "Error", Titulo = "Aviso!", Texto = "Seleccione un archivo a cargar" });
                    }
                }
                else
                {
                    return Json(new Respuesta { Estado = "Error", Titulo = "Aviso!", Texto = "Seleccione un archivo a cargar" });
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                return Json("Error al Cargar Archivo");
            }
        }

        ParametroBL oParametroBL = new ParametroBL();
        SugerenciaBL oSugerenciaBL = new SugerenciaBL();
        public ActionResult Contactos()
        {
            DateTime Desde = ConstantesHelpers.FechaDesde;
            DateTime Hasta = ConstantesHelpers.FechaHasta;
            string Tipo = "";

            if (Request.Form.Count > 0)
            {
                Desde = DateTime.Parse(Request.Form["start"].ToString());
                Hasta = DateTime.Parse(Request.Form["end"].ToString());
                Tipo = Request.Form["Tipo"].ToString();
            }

            var TipoContacto = oParametroBL.SelectByStart("06");
            ViewBag.FechaDesde = Desde;
            ViewBag.FechaHasta = Hasta;
            ViewBag.TipoContactoList = new SelectList(TipoContacto, "IdParametro", "Nombre", Tipo);
            return View(oSugerenciaBL.SelectByFechaAndTipo(Desde, Hasta, Tipo));
        }

        public ActionResult DownloadExcel(string Nombre)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(Server.MapPath(ConstantesLocal.RutaArchivosExcel), Nombre));
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, Nombre);
        }

        public ActionResult EliminarExcel(int Id)
        {
            oArchivoBL.Delete(Id);
            return RedirectToAction("ListaArchivos");
        }

        public ActionResult ActivarExcel(int ArchivoId)
        {
            Enumerators.RespuestaCargaExcel RespuestaCargaExcel;
            Archivo oArchivo = oArchivoBL.Select(ArchivoId);
            string RutaXLS = Path.Combine(Server.MapPath(ConstantesLocal.RutaArchivosExcel), oArchivo.NombreXLS);
            string RutaTXT = Path.Combine(Server.MapPath(ConstantesLocal.RutaArchivosExcel), oArchivo.NombreTXT);
            string Extension = System.IO.Path.GetExtension(oArchivo.NombreXLS);

            RespuestaCargaExcel = oSubirArchivoService.CargarExcelDataBase(oArchivo.NombreXLS, oArchivo.NombreTXT, Extension, RutaXLS, RutaTXT, "CARGAR", ArchivoId);

            switch (RespuestaCargaExcel)
            {
                case Enumerators.RespuestaCargaExcel.Correcto:
                    return RedirectToAction("ListaArchivos");

                case Enumerators.RespuestaCargaExcel.ArchivoNoExiste:
                    return Json(new Respuesta { Estado = "Error", Titulo = "Aviso!", Texto = "Archivo no existe" });

                case Enumerators.RespuestaCargaExcel.ExcelVacio:
                    return Json(new Respuesta { Estado = "Error", Titulo = "Aviso!", Texto = "El archivo excel se encuentra vacío" });

                case Enumerators.RespuestaCargaExcel.Error:
                    return Json(new Respuesta { Estado = "Error", Titulo = "Aviso!", Texto = "Error al cargar el archivo" });

                default:
                    return Json(new Respuesta { Estado = "OK", Titulo = "Aviso!", Texto = "Respuesta por defecto" });
            }
        }
    }
}
