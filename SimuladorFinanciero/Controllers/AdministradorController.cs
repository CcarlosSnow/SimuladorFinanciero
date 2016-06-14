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

namespace SimuladorFinanciero.Controllers
{
    public class AdministradorController : Controller
    {
        // GET: Administrador
        ArchivoBL oArchivoBL = new ArchivoBL();
        BancoBL oBancoBL = new BancoBL();
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
                            return Json("El archivo debe ser de tipo XLS o XLSX");
                        }
                        string Nombre = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + Extension;
                        string Ruta = Path.Combine(Server.MapPath(Constantes.RutaArchivosExcel), Nombre);
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
                            return Json("El archivo Excel se encuentra vacío");
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

                        var Bancos = data.AsEnumerable().GroupBy(r => r.Field<string>("ID"))
                            .Select(row => new Banco
                        {
                            IdBanco = row.First().Field<string>("ID").Trim(),
                            Nombre = row.First().Field<string>("Banco").Trim(),
                            Web = row.First().Field<string>("Web Banco").Trim()
                        }).Distinct();

                        oBancoBL.BulkInsert(Bancos);

                        Archivo oArchivoEnt = new Archivo();
                        oArchivoEnt.Nombre = Nombre;
                        if (oArchivoBL.UploadExcelFile(oArchivoEnt))
                        {
                            return Json("Archivo cargado correctamente");
                        }
                        else
                        {
                            return Json("Error al cargar archivo");
                        }
                    }
                    else
                    {
                        return Json("Seleccione un archivo a cargar");
                    }
                }
                else
                {
                    return Json("Seleccione un Archivo a Cargar");
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                return Json("Error al Cargar Archivo");
            }
        }
    }
}
