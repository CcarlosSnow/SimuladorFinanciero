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
using System.Web.UI.WebControls;
using System.Web.UI;
using Bytescout.Spreadsheet;
using System.Text;
using Newtonsoft.Json;

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
            //try
            //{
            //Response.Write("<script>alert('1');</script>");
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


                    //FileStream FileStr = new FileStream(RutaXLS, FileMode.Open);
                    
                    Spreadsheet oSpreadsheet = new Spreadsheet();
                    oSpreadsheet.LoadFromFile(RutaXLS);
                    oSpreadsheet.Workbook.Worksheets[0].SaveAsTXT(Path.Combine(Server.MapPath(ConstantesLocal.RutaArchivosExcel), NombreTXT));
                    
                    Enumerators.RespuestaCargaExcel RespuestaCargaExcel;
                    int FilaError = 0;
                    RespuestaCargaExcel = oSubirArchivoService.CargarExcelDataBase(NombreXLS, NombreTXT, Extension, RutaXLS, RutaTXT, "NUEVO", out FilaError);

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

                        case Enumerators.RespuestaCargaExcel.ErrorFila:
                            return Json(new Respuesta { Estado = "Error", Titulo = "Aviso!", Texto = "Error en la fila " + FilaError.ToString() });

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
            //}
            //catch (Exception ex)
            //{
            //    //Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            //    return Json(ex.Message);
            //}
        }

        ParametroBL oParametroBL = new ParametroBL();
        SugerenciaBL oSugerenciaBL = new SugerenciaBL();
        public ActionResult Contactos()
        {
            DateTime Desde = ConstantesHelpers.FechaDesde;
            DateTime Hasta = ConstantesHelpers.FechaHasta;
            string Tipo = "";
            string Estado = "";

            if (Request.Form.Count > 0)
            {
                Desde = DateTime.Parse(Request.Form["start"].ToString());
                Hasta = DateTime.Parse(Request.Form["end"].ToString());
                Tipo = Request.Form["Tipo"].ToString();
                Estado = Request.Form["Estado"].ToString();
            }

            var TipoCombo = oParametroBL.SelectByStart("06");
            var EstadoCombo = oParametroBL.SelectByStart("07");
            ViewBag.FechaDesde = Desde;
            ViewBag.FechaHasta = Hasta;
            ViewBag.Tipo = Tipo;
            ViewBag.Estado = Estado;
            ViewBag.TipoCombo = new SelectList(TipoCombo, "IdParametro", "Nombre", Tipo);
            ViewBag.EstadoCombo = new SelectList(EstadoCombo, "IdParametro", "Nombre", Estado);
            return View(oSugerenciaBL.SelectByFechaAndTipo(Desde, Hasta, Tipo, Estado));
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

        [HttpGet]
        public bool UpdateEstadoSugerencia(int IdSugerencia, string Estado)
        {
            return oSugerenciaBL.UpdateEstado(IdSugerencia, Estado);
        }

        [HttpGet]
        public string MostrarDetalleSugerencia(int IdSugerencia)
        {
            return oSugerenciaBL.Select(IdSugerencia).Descripcion;
        }

        public ActionResult ActivarExcel(int ArchivoId)
        {
            Enumerators.RespuestaCargaExcel RespuestaCargaExcel;
            Archivo oArchivo = oArchivoBL.Select(ArchivoId);
            string RutaXLS = Path.Combine(Server.MapPath(ConstantesLocal.RutaArchivosExcel), oArchivo.NombreXLS);
            string RutaTXT = Path.Combine(Server.MapPath(ConstantesLocal.RutaArchivosExcel), oArchivo.NombreTXT);
            string Extension = System.IO.Path.GetExtension(oArchivo.NombreXLS);
            int FilaError = 0;
            RespuestaCargaExcel = oSubirArchivoService.CargarExcelDataBase(oArchivo.NombreXLS, oArchivo.NombreTXT, Extension, RutaXLS, RutaTXT, "CARGAR", out FilaError, ArchivoId);

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

                case Enumerators.RespuestaCargaExcel.ErrorFila:
                    return Json(new Respuesta { Estado = "Error", Titulo = "Aviso!", Texto = "Error en la fila " + FilaError.ToString() });

                default:
                    return Json(new Respuesta { Estado = "OK", Titulo = "Aviso!", Texto = "Respuesta por defecto" });
            }
        }

        public ActionResult ExportExcelContactos(DateTime Desde, DateTime Hasta, string Tipo = "")
        {
            //GridView gv = new GridView();
            //gv.DataSource = oSugerenciaBL.SelectByFechaAndTipo(Desde, Hasta, Tipo);
            //gv.DataBind();
            var Sugerencias = oSugerenciaBL.SelectByFechaAndTipo(Desde, Hasta, Tipo);
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
