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

namespace SimuladorFinanciero.Controllers
{
    public class AdministradorController : Controller
    {
        // GET: Administrador
        ArchivoBL oArchivoBL = new ArchivoBL();
        BancoBL oBancoBL = new BancoBL();
        ProductoBL oProductoBL = new ProductoBL();
        ConceptoBL oConceptoBL = new ConceptoBL();
        ProductoBancoBL oProductoBancoBL = new ProductoBancoBL();
        ConceptoProductoBancoBL oConceptoProductoBancoBL = new ConceptoProductoBancoBL();
        SugerenciaBL oSugerenciaBL = new SugerenciaBL();
        ParametroBL oParametroBL = new ParametroBL();
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
                        string Nombre = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + Extension;
                        string Ruta = Path.Combine(Server.MapPath(ConstantesLocal.RutaArchivosExcel), Nombre);
                        file.SaveAs(Ruta);

                        string excelConnectionString = string.Empty;
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                        Ruta + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";

                        if (Extension == ".xls")
                        {
                            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                            Ruta + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        }
                        else if (Extension == ".xlsx")
                        {
                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            Ruta + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        }

                        OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                        excelConnection.Open();
                        DataTable dt = new DataTable();

                        dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        if (dt == null)
                        {
                            return Json(new Respuesta { Estado = "Error", Titulo = "Aviso!", Texto = "El archivo se encuentra vacío" });
                        }

                        string[] excelSheets = new string[dt.Rows.Count];
                        int t = 0;
                        //excel data saves in temp file here.
                        foreach (DataRow row in dt.Rows)
                        {
                            excelSheets[t] = row["TABLE_NAME"].ToString();
                            t++;
                        }
                        OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);

                        string query = string.Format("Select * from [{0}]", excelSheets[0]);
                        DataTable data = new DataTable();
                        using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                        {
                            dataAdapter.Fill(data);
                        }

                        oConceptoProductoBancoBL.DeleteAll();
                        oProductoBancoBL.DeleteAll();
                        oConceptoBL.DeleteAll();
                        oProductoBL.DeleteAll();
                        oBancoBL.DeleteAll();

                        var Bancos = data.AsEnumerable().GroupBy(r => r.Field<string>("ID").Trim())
                            .Select(row => new Banco
                            {
                                IdBanco = row.First().Field<string>("ID").Trim(),
                                Nombre = row.First().Field<string>("Banco").Trim(),
                                Web = row.First().Field<string>("Web Banco").Trim()
                            });
                        oBancoBL.BulkInsert(Bancos);

                        var Productos = data.AsEnumerable().GroupBy(r => r.Field<string>("Producto").Trim())
                            .Select(row => new Producto
                            {
                                Nombre = row.First().Field<string>("Producto").Trim(),
                                Tipo = int.Parse(Nombre.Substring(1, Nombre.IndexOf('0', 1, 1)))
                            });

                        oProductoBL.BulkInsert(Productos);

                        var Conceptos = data.AsEnumerable().GroupBy(r => r.Field<string>("Concepto").Trim())
                            .Select(row => new Concepto
                            {
                                Nombre = row.First().Field<string>("Concepto").Trim()
                            });

                        oConceptoBL.BulkInsert(Conceptos);

                        var ProductosBancos = data.AsEnumerable().GroupBy(r => new
                        {
                            Banco = r.Field<string>("ID").Trim(),
                            Producto = r.Field<string>("Producto").Trim()
                        }).
                        Select(row => new ProductoBanco
                        {
                            IdBanco = row.First().Field<string>("ID").Trim(),
                            Producto = new Producto
                            {
                                Nombre = row.First().Field<string>("Producto").Trim(),
                            },
                            WebTarifario = row.First().Field<string>("Web Tarifario").Trim(),
                            Contacto = row.First().Field<string>("Contacto").Trim()
                        });

                        oProductoBancoBL.BulkInsert(ProductosBancos);

                        //var ConceptosProductosBancos = from i in data.AsEnumerable()
                        //                               select new ConceptoProductoBanco
                        //                               {
                        //                                   Concepto = new Concepto
                        //                                   {
                        //                                       Nombre = i.Field<string>("Concepto")
                        //                                   },
                        //                                   ProductoBanco = new ProductoBanco
                        //                                   {
                        //                                       Producto = new Producto
                        //                                       {
                        //                                           Nombre = i.Field<string>("Producto").Trim()
                        //                                       },
                        //                                   },
                        //                                   IdBanco = i.Field<string>("ID").Trim(),
                        //                                   TipoComision = i.Field<string>("Tipo de comisión (usual (U) o eventual E)").Trim(),
                        //                                   Tasa = i.Field<decimal>("Tasa")
                        //                               };
                        //GroupBy(r => new
                        //{
                        //    Concepto = r.Field<string>("Concepto").Trim(),
                        //    Producto = r.Field<string>("Producto").Trim(),
                        //    Banco = r.Field<string>("ID").Trim(),
                        //    TipoComision = r.Field<string>("Tipo de comisión (usual (U) o eventual E)").Trim()
                        //}).
                        //Select(row => new ConceptoProductoBanco
                        //{
                        //    Concepto = new Concepto
                        //    {
                        //        Nombre = row.Field<string>("Concepto").Trim()
                        //    },
                        //    ProductoBanco = new ProductoBanco
                        //    {
                        //        Producto = new Producto
                        //        {
                        //            Nombre = row.Field<string>("Producto").Trim()
                        //        },
                        //    },
                        //    IdBanco = row.Field<string>("ID").Trim(),
                        //    TipoComision = row.Field<string>("Tipo de comisión (usual (U) o eventual E)").Trim(),
                        //    Tasa = row.Field<Nullable>("Tasa")
                        //Minimo = row.Field<decimal>("Min"),
                        //Maximo = row.Field<decimal>("Max"),
                        //METasaMax = row.Field<decimal>("ME-Tasa Max."),
                        //METasaMin = row.Field<decimal>("ME-Tasa Min."),
                        //MEMin = row.Field<decimal>("ME-Min"),
                        //MEMax = row.Field<decimal>("ME-Max"),
                        //Observaciones = row.Field<string>("Observaciones").Trim()

                        //});

                        //oConceptoProductoBancoBL.BulkInsert(ConceptosProductosBancos);

                        Archivo oArchivoEnt = new Archivo();
                        oArchivoEnt.Nombre = Nombre;
                        if (oArchivoBL.UploadExcelFile(oArchivoEnt))
                        {
                            return Json(new Respuesta { Estado = "OK", Titulo = "Aviso!", Texto = "Archivo Cargado Correctamente" });
                        }
                        else
                        {
                            return Json("Error al cargar archivo");
                        }
                    }
                    else
                    {
                        return Json(new Respuesta { Estado = "Error", Titulo = "Aviso!", Texto = "Seleccione un archivo a cargar" });
                        //Respuesta oRespuesta = new Respuesta();
                        //oRespuesta.Titulo = "Aviso!";
                        //oRespuesta.Texto = "Seleccione un Archivo a Cargar";
                        //return Json(Newtonsoft.Json.JsonConvert.SerializeObject(oRespuesta, Newtonsoft.Json.Formatting.Indented));
                        //return Json("Seleccione un archivo a cargar");
                    }
                }
                else
                {
                    return Json(new Respuesta { Estado = "Error", Titulo = "Aviso!", Texto = "Seleccione un archivo a cargar" });
                    //Respuesta oRespuesta = new Respuesta();
                    //oRespuesta.Titulo = "Aviso!";
                    //oRespuesta.Texto = "Seleccione un Archivo a Cargar";
                    //return Json(Newtonsoft.Json.JsonConvert.SerializeObject(oRespuesta, Newtonsoft.Json.Formatting.Indented));
                    //return Json("Seleccione un Archivo a Cargar");
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                return Json("Error al Cargar Archivo");
            }
        }

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

        //[HttpPost]
        //public ActionResult Contactos()
        //{
        //    var TipoContacto = oParametroBL.SelectByStart("06");
        //    ViewBag.TipoContactoList = new SelectList(TipoContacto, "IdParametro", "Nombre");
        //    return View(oSugerenciaBL.SelectByFechaAndTipo(ConstantesHelpers.FechaDesde, ConstantesHelpers.FechaHasta));
        //}

        public ActionResult DownloadExcel(string Nombre)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(Server.MapPath(ConstantesLocal.RutaArchivosExcel), Nombre));
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, Nombre);
        }
    }
}
