using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimuladorFinanciero.Core;
using System.Data.OleDb;
using System.Data;
using SimuladorFinanciero.Entities;
using System.IO;
using SimuladorFinanciero.Helpers;

namespace SimuladorFinanciero
{
    public class SubirArchivoService
    {
        public Enumerators.RespuestaCargaExcel CargarExcelDataBase(string NombreXLS, string NombreTXT, string Extension, string RutaXLS, string RutaTXT, string Accion, out int FilaError, int IdArchivo = 0)
        {
            FilaError = 0;
            if (File.Exists(RutaXLS))
            {
                string excelConnectionString = string.Empty;
                if (Extension == ".xls")
                {
                    excelConnectionString = ConstantesHelpers.ConnectionExcelXLS(RutaXLS);
                }
                else if (Extension == ".xlsx")
                {
                    excelConnectionString = ConstantesHelpers.ConnectionExcelXLSX(RutaXLS);
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
                int Linea = 0;
                string LineaError = "";


                try
                {
                    //oConceptoProductoBancoBL.DeleteAll();
                    //oProductoBancoBL.DeleteAll();
                    //oConceptoBL.DeleteAll();
                    //oProductoBL.DeleteAll();
                    //oBancoBL.DeleteAll();

                    var Bancos = data.AsEnumerable().GroupBy(r => r.Field<string>("ID").Trim())
                        .Select(row => new Banco
                        {
                            IdBanco = row.First().Field<string>("ID").Trim(),
                            Nombre = row.First().Field<string>("Banco").Trim(),
                            Web = row.First().Field<string>("Web Banco").Trim()
                        });
                    //oBancoBL.BulkInsert(Bancos);

                    var Productos = data.AsEnumerable().GroupBy(r => r.Field<string>("Producto").Trim())
                        .Select(row => new Producto
                        {
                            Nombre = row.First().Field<string>("Producto").Trim(),
                            Tipo = int.Parse(row.First().Field<string>("Producto").Trim().Substring(0, 1))
                        });

                    //oProductoBL.BulkInsert(Productos);

                    var Conceptos = data.AsEnumerable().GroupBy(r => r.Field<string>("Concepto").Trim())
                        .Select(row => new Concepto
                        {
                            Nombre = row.First().Field<string>("Concepto").Trim()
                        });

                    //oConceptoBL.BulkInsert(Conceptos);

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

                    //oProductoBancoBL.BulkInsert(ProductosBancos);

                    StreamReader oStreamReader = new StreamReader(RutaTXT, Encoding.Default, false);
                    string[] Datos = oStreamReader.ReadLine().Split(Convert.ToChar(9));
                    List<ConceptoProductoBanco> ConceptosProductosBancos = new List<ConceptoProductoBanco>();
                    ConceptoProductoBanco oConceptoProductoBanco = null;

                    try
                    {
                        while (oStreamReader.Peek() >= 0)
                        {

                            Linea++;
                            LineaError = oStreamReader.ReadLine();
                            Datos = LineaError.Split(Convert.ToChar(9));

                            oConceptoProductoBanco = new ConceptoProductoBanco();
                            oConceptoProductoBanco.Concepto = new Concepto
                            {
                                Nombre = Datos[4]
                            };
                            oConceptoProductoBanco.ProductoBanco = new ProductoBanco
                            {
                                Producto = new Producto
                                {
                                    Nombre = Datos[3]
                                }
                            };
                            oConceptoProductoBanco.IdBanco = Datos[1];
                            oConceptoProductoBanco.TipoComision = Datos[5];

                            if (Datos[6].Trim().Length > 0)
                            {
                                oConceptoProductoBanco.Tasa30 = decimal.Parse(Datos[6]);
                            }
                            else
                            {
                                oConceptoProductoBanco.Tasa30 = 0;
                            }

                            if (Datos[7].Trim().Length > 0)
                            {
                                oConceptoProductoBanco.Tasa60 = decimal.Parse(Datos[7]);
                            }
                            else
                            {
                                oConceptoProductoBanco.Tasa60 = 0;
                            }

                            if (Datos[8].Trim().Length > 0)
                            {
                                oConceptoProductoBanco.Tasa90 = decimal.Parse(Datos[8]);
                            }
                            else
                            {
                                oConceptoProductoBanco.Tasa90 = 0;
                            }

                            if (Datos[9].Trim().Length > 0)
                            {
                                oConceptoProductoBanco.Minimo = decimal.Parse(Datos[9]);
                            }
                            else
                            {
                                oConceptoProductoBanco.Minimo = 0;
                            }

                            if (Datos[10].Trim().Length > 0)
                            {
                                oConceptoProductoBanco.Maximo = decimal.Parse(Datos[10]);
                            }
                            else
                            {
                                oConceptoProductoBanco.Maximo = 0;
                            }

                            if (Datos[11].Trim().Length > 0)
                            {
                                oConceptoProductoBanco.METasaMax = decimal.Parse(Datos[11]);
                            }
                            else
                            {
                                oConceptoProductoBanco.METasaMax = 0;
                            }

                            if (Datos[12].Trim().Length > 0)
                            {
                                oConceptoProductoBanco.METasaMin = decimal.Parse(Datos[12]);
                            }
                            else
                            {
                                oConceptoProductoBanco.METasaMin = 0;
                            }

                            if (Datos[13].Trim().Length > 0)
                            {
                                oConceptoProductoBanco.MEMin = decimal.Parse(Datos[13]);
                            }
                            else
                            {
                                oConceptoProductoBanco.MEMin = 0;
                            }

                            if (Datos[14].Trim().Length > 0)
                            {
                                oConceptoProductoBanco.MEMax = decimal.Parse(Datos[14]);
                            }
                            else
                            {
                                oConceptoProductoBanco.MEMax = 0;
                            }

                            oConceptoProductoBanco.Observaciones = Datos[18];

                            ConceptosProductosBancos.Add(oConceptoProductoBanco);
                        }

                        oStreamReader.Close();
                        oStreamReader.Dispose();
                    }
                    catch (Exception ex)
                    {
                        FilaError = Linea;
                        return Enumerators.RespuestaCargaExcel.ErrorFila;
                    }

                    oArchivoBL.InsertExcel(Bancos, Productos, Conceptos, ProductosBancos, ConceptosProductosBancos);
                    //oConceptoProductoBancoBL.BulkInsert(ConceptosProductosBancos);
                }
                catch (Exception)
                {
                    return Enumerators.RespuestaCargaExcel.Error;
                }

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
