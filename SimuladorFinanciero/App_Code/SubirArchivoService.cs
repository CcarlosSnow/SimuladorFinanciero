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
        public Enumerators.RespuestaCargaExcel CargarExcelDataBase(string Nombre, string Extension, string Ruta, string Accion, int IdArchivo = 0)
        {
            if (File.Exists(Ruta))
            {
                string excelConnectionString = string.Empty;
                if (Extension == ".xls")
                {
                    excelConnectionString = ConstantesLocal.ConnectionExcelXLS(Ruta);
                }
                else if (Extension == ".xlsx")
                {
                    excelConnectionString = ConstantesLocal.ConnectionExcelXLSX(Ruta);
                }

                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                excelConnection.Open();
                DataTable dt = new DataTable();

                dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dt == null)
                {
                    return Enumerators.RespuestaCargaExcel.ExcelVacio;
                }

                string[] excelSheets = new string[dt.Rows.Count];
                int t = 0;
                
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
                    oArchivoEnt.Nombre = Nombre;
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
