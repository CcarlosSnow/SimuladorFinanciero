using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimuladorFinanciero.Core;
using System.Data.OleDb;
using System.Data;
using SimuladorFinanciero.Entities;
using System.IO;

namespace SimuladorFinanciero
{
    public class SubirArchivoService
    {
        public Enumerators.RespuestaCargaExcel CargarExcelDataBase(string NombreXLS, string NombreTXT, string Extension, string RutaXLS, string RutaTXT, string Accion, int IdArchivo = 0)
        {
            if (File.Exists(RutaXLS))
            {
                string excelConnectionString = string.Empty;
                if (Extension == ".xls")
                {
                    excelConnectionString = ConstantesLocal.ConnectionExcelXLS(RutaXLS);
                }
                else if (Extension == ".xlsx")
                {
                    excelConnectionString = ConstantesLocal.ConnectionExcelXLSX(RutaXLS);
                }

                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                excelConnection.Open();
                DataTable dt = new DataTable();

                dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dt == null)
                {
                    return Enumerators.RespuestaCargaExcel.ExcelVacio;
                    //return Json(new Respuesta { Estado = "Error", Titulo = "Aviso!", Texto = "El archivo se encuentra vacío" });
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

                ArchivoBL oArchivoBL = new ArchivoBL();
                BancoBL oBancoBL = new BancoBL();
                ProductoBL oProductoBL = new ProductoBL();
                ConceptoBL oConceptoBL = new ConceptoBL();
                ProductoBancoBL oProductoBancoBL = new ProductoBancoBL();
                ConceptoProductoBancoBL oConceptoProductoBancoBL = new ConceptoProductoBancoBL();


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
                        Tipo = int.Parse(row.First().Field<string>("Producto").Trim().Substring(2, 1))
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

                //StreamReader oStreamReader = new StreamReader(RutaTXT, Encoding.UTF7);
                //string[] Datos = oStreamReader.ReadLine().Split(Convert.ToChar(9));
                //int Linea = 0;
                //string LineaError = "";
                //List<ConceptoProductoBanco> ConceptosProductosBancos = new List<ConceptoProductoBanco>();
                //ConceptoProductoBanco oConceptoProductoBanco = null;
                //while (oStreamReader.Peek() >= 0)
                //{
                //    try
                //    {
                //        Linea++;
                //        LineaError = oStreamReader.ReadLine();
                //        Datos = LineaError.Split(Convert.ToChar(9));
                //        oConceptoProductoBanco = new ConceptoProductoBanco();
                //        oConceptoProductoBanco.Concepto = new Concepto
                //        {
                //            Nombre = Datos[4]
                //        };
                //        oConceptoProductoBanco.ProductoBanco = new ProductoBanco
                //        {
                //            Producto = new Producto
                //            {
                //                Nombre = Datos[3]
                //            }
                //        };
                //        oConceptoProductoBanco.IdBanco = Datos[1];
                //        oConceptoProductoBanco.TipoComision = Datos[5];
                //        decimal Tasa = 0;
                //        oConceptoProductoBanco.Tasa = decimal.TryParse(Datos[6], out Tasa) ? Tasa : 0;

                //        decimal Min = 0;
                //        oConceptoProductoBanco.Minimo = decimal.TryParse(Datos[7], out Min) ? Min : 0;

                //        decimal Max = 0;
                //        oConceptoProductoBanco.Maximo = decimal.TryParse(Datos[8], out Max) ? Max : 0;

                //        decimal METasaMax = 0;
                //        oConceptoProductoBanco.METasaMax = decimal.TryParse(Datos[9], out METasaMax) ? METasaMax : 0;

                //        decimal METasaMin = 0;
                //        oConceptoProductoBanco.METasaMin = decimal.TryParse(Datos[10], out METasaMin) ? METasaMin : 0;

                //        decimal MEMin = 0;
                //        oConceptoProductoBanco.MEMin = decimal.TryParse(Datos[11], out MEMin) ? MEMin : 0;

                //        decimal MEMax = 0;
                //        oConceptoProductoBanco.MEMax = decimal.TryParse(Datos[12], out MEMax) ? MEMax : 0;

                //        oConceptoProductoBanco.Observaciones = Datos[13];

                //        ConceptosProductosBancos.Add(oConceptoProductoBanco);
                //    }
                //    catch (Exception)
                //    {

                //        throw;
                //    }
                //}
                //oStreamReader.Close();
                //oStreamReader.Dispose();
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


                excelConnection.Dispose();
                excelConnection1.Dispose();
                if (Accion == "NUEVO")
                {
                    Archivo oArchivoEnt = new Archivo();
                    oArchivoEnt.NombreXLS = NombreXLS;
                    oArchivoEnt.NombreTXT = NombreTXT;
                    if (oArchivoBL.UploadExcelFile(oArchivoEnt))
                    {
                        return Enumerators.RespuestaCargaExcel.Correcto;
                    }
                    else
                    {
                        return Enumerators.RespuestaCargaExcel.Error;
                    }
                }
                else
                {
                    if (oArchivoBL.ActiveExcelFile(IdArchivo))
                    {
                        return Enumerators.RespuestaCargaExcel.Correcto;
                    }
                    else
                    {
                        return Enumerators.RespuestaCargaExcel.Error;
                    }
                }
            }
            else
            {
                return Enumerators.RespuestaCargaExcel.ArchivoNoExiste;
            }
        }
    }
}
