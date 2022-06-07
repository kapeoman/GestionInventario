using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GestionInventario.Models;
using GestionInventario.Models.View;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace GestionInventario.Models.Repository
{
    public class MetodoInventario
    {
        Usuario oUser = (Usuario)HttpContext.Current.Session["User"];
        GestionInventarioEntities db = new GestionInventarioEntities();

        #region Inventario
        
        public Inventario GetInventario(int Codigo)
        {
            Inventario inventario = db.Inventario.Where(x => x.Codigo == Codigo).SingleOrDefault();
            return inventario;
        }

        public InventarioView ListaInventario()
        {
            InventarioView inventarioView = new InventarioView();

            inventarioView.inventarioActivo = db.Inventario.Where(x => !x.Eliminado).ToList();
            inventarioView.inventarioInactivo = db.Inventario.Where(x => x.Eliminado).ToList();

            return inventarioView;
        }

        public ResponseModel EliminarInventario(int Codigo)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    Inventario inventario = db.Inventario.Where(x => x.Codigo == Codigo).SingleOrDefault();
                    inventario.Eliminado = true;
                    db.SaveChanges();
                    dbContextTransaction.Commit();

                    response.Error = false;
                    response.Mensaje = "Se ha Eliminado correctamente";
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Mensaje = "Se ha producido un error " + ex.Message;
                return response;
            }
        }

        public ResponseModel ReactivarInventario(int Codigo)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    Inventario inventario = db.Inventario.Where(x => x.Codigo == Codigo).SingleOrDefault();
                    inventario.Eliminado = false;
                    db.SaveChanges();
                    dbContextTransaction.Commit();

                    response.Error = false;
                    response.Mensaje = "Se ha Reactivado correctamente";
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Mensaje = "Se ha producido un error" + ex.Message;
                return response;
            }
        }

        public ResponseModel EditarInventario(Inventario inventarioNew)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    if (inventarioNew.Stock >= 0)
                    {
                        inventarioNew.Producto = db.Producto.Where(x => x.Codigo == inventarioNew.CodigoProducto).SingleOrDefault();
                        Inventario inventario = db.Inventario.Where(x => x.Codigo == inventarioNew.Codigo).SingleOrDefault();
                        inventarioNew.CodigoProducto = inventario.CodigoProducto;
                        inventarioNew.Producto = db.Producto.Where(x => x.Codigo == inventario.CodigoProducto).SingleOrDefault();
                        if (SaveInventarioHistorico(inventarioNew))
                        {
                            inventario.Stock = inventarioNew.Stock;

                            db.SaveChanges();
                            dbContextTransaction.Commit();

                            response.Error = false;
                            response.Mensaje = "Se ha Guardado el stock correctamente";
                        }
                        else
                        {
                            response.Error = true;
                            response.Mensaje = "Se ha producido un error ";
                            return response;

                        }
                    }
                    
                    //inventario.Stock = inventarioNew.Stock;

                    //db.SaveChanges();
                    //dbContextTransaction.Commit();

                    //response.Error = false;
                    //response.Mensaje = "Se ha Guardado el stock correctamente";
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Mensaje = "Se ha producido un error " + ex;
                return response;
            }
        }

        public bool SaveInventarioHistorico(Inventario inventario)
        {
            try
            {

                //using (var dbContextTransaction = db.Database.BeginTransaction())
                //{                

                InventarioHistorico inventarioHistorico = db.InventarioHistorico.Where(x => x.CodigoInventario == inventario.Codigo).OrderByDescending(x => x.Fecha).SingleOrDefault();
                if (inventarioHistorico == null)
                {
                    inventarioHistorico = new InventarioHistorico();
                    inventarioHistorico.CodigoInventario = inventario.Codigo;
                    inventarioHistorico.Fecha = DateTime.Now;
                    inventarioHistorico.StockAnterior = 0;
                    inventarioHistorico.StockNuevo = inventario.Stock;
                    inventarioHistorico.PrecioUnitario = inventario.Producto.precioUnitario.Value;
                    inventarioHistorico.Ingreso = inventarioHistorico.StockNuevo < inventarioHistorico.StockAnterior ? false : true;
                    inventarioHistorico.UsuarioId = oUser.Id;
                    db.InventarioHistorico.Add(inventarioHistorico);
                }
                else
                {
                    //inventarioHistorico.Fecha = DateTime.Now;
                    //inventarioHistorico.StockAnterior = inventarioHistorico.StockNuevo;
                    //inventarioHistorico.StockNuevo = inventario.Stock;
                    //inventarioHistorico.PrecioUnitario = inventario.Producto.precioUnitario.Value;
                    //inventarioHistorico.Ingreso = inventarioHistorico.StockNuevo < inventarioHistorico.StockAnterior ? false : true;
                    //inventarioHistorico.UsuarioId = oUser.Id;
                    InventarioHistorico inventarioHistoricoNew = (new InventarioHistorico
                    {
                        CodigoInventario = inventarioHistorico.CodigoInventario,
                        Fecha = DateTime.Now,
                        StockAnterior = inventarioHistorico.StockNuevo,
                        StockNuevo = inventario.Stock,
                        PrecioUnitario = inventario.Producto.precioUnitario.Value,
                        Ingreso = inventarioHistorico.StockNuevo < inventarioHistorico.StockAnterior ? false : true,
                        UsuarioId = oUser.Id,
                    });
                    db.InventarioHistorico.Add(inventarioHistoricoNew);
                }
                //Inventario inventario = db.Inventario.Where(x => x.Codigo == inventarioNew.Codigo).SingleOrDefault();

                //db.InventarioHistorico.Add(inventarioHistoricoNew);
                db.SaveChanges();
                //dbContextTransaction.Commit();


                //}
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        #endregion

        #region inventarioHistorico

        public List<InventarioHistorico> ListaInventarioHistoricos()
        {
            List<InventarioHistorico> lst = db.InventarioHistorico.ToList();
            return lst;
        }

        public ExcelPackage ReporteFullHistorico()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var excelPackage = new ExcelPackage();
            //Propiedades Hoja de excel
            var sheet = excelPackage.Workbook.Worksheets.Add("Informe General Semanal");
            sheet.Name = "Informe";
            var columnaFinal = 6;

            //********************************************************************************************************
            //encabezados tabla de datos

            sheet.Cells[1, 1, 1, columnaFinal].Merge = true;
            sheet.Cells[1, 1].Value = "Informe Historico";
            sheet.Cells[1, 1].Style.Font.Size = 14;
            sheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // TITULOS COLUMNAS
            sheet.Cells[2, 1].Value = "Codigo";
            sheet.Cells[2, 2].Value = "Nombre Producto";
            sheet.Cells[2, 3].Value = "Stock Anterior";
            sheet.Cells[2, 4].Value = "Stock Actual";
            sheet.Cells[2, 5].Value = "Precio Unitario";
            sheet.Cells[2, 6].Value = "Fecha";

            // ANCHO Y ALTO DE COLUMNAS Y FILAS
            sheet.Row(1).Height = 50;
            sheet.Column(1).Width = 15;
            sheet.Column(2).Width = 15;
            sheet.Column(3).Width = 15;
            sheet.Column(4).Width = 15;
            sheet.Column(5).Width = 15;
            sheet.Column(6).Width = 25;


            // TAMAÑO LETRA, AJUSTAR, CENTRAR Y ALINEAR TEXTO
            sheet.Cells[2, 1, 2, columnaFinal].Style.WrapText = true;
            sheet.Cells[2, 1, 2, columnaFinal].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[2, 1, 2, columnaFinal].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[2, 1, 2, columnaFinal].Style.Font.Size = 12;

            // NEGRITA A LAS LETRAS
            sheet.Cells[1, 1, 1, columnaFinal].Style.Font.Bold = true;
            sheet.Cells[2, 1, 2, columnaFinal].Style.Font.Bold = true;

            // BORDE A LA TABLA
            var bordesEncabezados = sheet.Cells[1, 1, 2, columnaFinal].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;

           
            var col = 1;
            //Empezamos a escribir sobre ella
            var rowindex = 3;

            //string dateformat = "m/d/yy h:mm";
            foreach (var item in ListaInventarioHistoricos())
            {
                //totalPositivo = totalPositivo + lstCasoCovid.Where(x => x.idCenco == item.idTalana && x.Estado_PCR == 1).Count();
                //totalEnEspera = totalEnEspera + lstCasoCovid.Where(x => x.idCenco == item.idTalana && x.Estado_PCR == 3).Count();
                col = 1;
                sheet.Cells[rowindex, col++].Value = item.Inventario.CodigoProducto;
                sheet.Cells[rowindex, col++].Value = item.Inventario.Producto.nombre;
                sheet.Cells[rowindex, col++].Value = item.StockAnterior;
                sheet.Cells[rowindex, col++].Value = item.StockNuevo;
                sheet.Cells[rowindex, col++].Value = item.PrecioUnitario;
                sheet.Cells[rowindex, col].Style.Numberformat.Format = "mm/dd/yyyy hh:mm:ss AM/PM";
                sheet.Cells[rowindex, col++].Value = item.Fecha;
                
                rowindex++;
            }
            //foreach (var item in lstUnidadOrga)
            //{
            //    totalPositivo = totalPositivo + lstCasoCovid.Where(x => x.idUnidadOrga == item.idOrganizacion && x.Estado_PCR == 1).Count();
            //    totalEnEspera = totalEnEspera + lstCasoCovid.Where(x => x.idUnidadOrga == item.idOrganizacion && x.Estado_PCR == 3).Count();
            //    col = 1;
            //    sheet.Cells[rowindex, col++].Value = item.nombre;
            //    sheet.Cells[rowindex, col++].Value = lstCasoCovid.Where(x => x.idUnidadOrga == item.idOrganizacion && x.Estado_PCR == 1).Count();
            //    sheet.Cells[rowindex, col++].Value = lstCasoCovid.Where(x => x.idUnidadOrga == item.idOrganizacion && x.Estado_PCR == 3).Count();
            //    sheet.Cells[rowindex, col++].Value = "CASA MATRIZ";
            //    rowindex++;
            //}


            //sheet.Cells[rowindex, 1, rowindex, 1].Style.Font.Bold = true;
            //sheet.Cells[rowindex, 1].Value = "Totales";
            //sheet.Cells[rowindex, 2].Value = totalPositivo;
            //sheet.Cells[rowindex, 3].Value = totalEnEspera;
            var bordesTotales = sheet.Cells[rowindex, 1, rowindex, columnaFinal].Style.Border;
            bordesTotales.Top.Style = bordesTotales.Right.Style = bordesTotales.Bottom.Style = bordesTotales.Left.Style = ExcelBorderStyle.Medium;


            return excelPackage;
        }

        public ExcelPackage ReporteFullInventario()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var excelPackage = new ExcelPackage();
            //Propiedades Hoja de excel
            var sheet = excelPackage.Workbook.Worksheets.Add("Informe General Semanal");
            sheet.Name = "Informe";
            var columnaFinal = 4;

            //********************************************************************************************************
            //encabezados tabla de datos

            sheet.Cells[1, 1, 1, columnaFinal].Merge = true;
            sheet.Cells[1, 1].Value = "Informe Historico";
            sheet.Cells[1, 1].Style.Font.Size = 14;
            sheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // TITULOS COLUMNAS
            sheet.Cells[2, 1].Value = "Codigo";
            sheet.Cells[2, 2].Value = "Nombre Producto";
            sheet.Cells[2, 3].Value = "Stock";
            sheet.Cells[2, 4].Value = "Precio Unitario";
            

            // ANCHO Y ALTO DE COLUMNAS Y FILAS
            sheet.Row(1).Height = 50;
            sheet.Column(1).Width = 15;
            sheet.Column(2).Width = 15;
            sheet.Column(3).Width = 15;
            sheet.Column(4).Width = 15;
            sheet.Column(5).Width = 15;
            sheet.Column(6).Width = 25;


            // TAMAÑO LETRA, AJUSTAR, CENTRAR Y ALINEAR TEXTO
            sheet.Cells[2, 1, 2, columnaFinal].Style.WrapText = true;
            sheet.Cells[2, 1, 2, columnaFinal].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[2, 1, 2, columnaFinal].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[2, 1, 2, columnaFinal].Style.Font.Size = 12;

            // NEGRITA A LAS LETRAS
            sheet.Cells[1, 1, 1, columnaFinal].Style.Font.Bold = true;
            sheet.Cells[2, 1, 2, columnaFinal].Style.Font.Bold = true;

            // BORDE A LA TABLA
            var bordesEncabezados = sheet.Cells[1, 1, 2, columnaFinal].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;


            var col = 1;
            //Empezamos a escribir sobre ella
            var rowindex = 3;

            //string dateformat = "m/d/yy h:mm";
            foreach (var item in ListaInventario().inventarioActivo)
            {
                //totalPositivo = totalPositivo + lstCasoCovid.Where(x => x.idCenco == item.idTalana && x.Estado_PCR == 1).Count();
                //totalEnEspera = totalEnEspera + lstCasoCovid.Where(x => x.idCenco == item.idTalana && x.Estado_PCR == 3).Count();
                col = 1;
                sheet.Cells[rowindex, col++].Value = item.CodigoProducto;
                sheet.Cells[rowindex, col++].Value = item.Producto.nombre;
                sheet.Cells[rowindex, col++].Value = item.Stock;
                sheet.Cells[rowindex, col++].Value = item.Producto.precioUnitario;
                //sheet.Cells[rowindex, col++].Value = item.PrecioUnitario;
                //sheet.Cells[rowindex, col].Style.Numberformat.Format = "mm/dd/yyyy hh:mm:ss AM/PM";
                //sheet.Cells[rowindex, col++].Value = item.Fecha;

                rowindex++;
            }
            //foreach (var item in lstUnidadOrga)
            //{
            //    totalPositivo = totalPositivo + lstCasoCovid.Where(x => x.idUnidadOrga == item.idOrganizacion && x.Estado_PCR == 1).Count();
            //    totalEnEspera = totalEnEspera + lstCasoCovid.Where(x => x.idUnidadOrga == item.idOrganizacion && x.Estado_PCR == 3).Count();
            //    col = 1;
            //    sheet.Cells[rowindex, col++].Value = item.nombre;
            //    sheet.Cells[rowindex, col++].Value = lstCasoCovid.Where(x => x.idUnidadOrga == item.idOrganizacion && x.Estado_PCR == 1).Count();
            //    sheet.Cells[rowindex, col++].Value = lstCasoCovid.Where(x => x.idUnidadOrga == item.idOrganizacion && x.Estado_PCR == 3).Count();
            //    sheet.Cells[rowindex, col++].Value = "CASA MATRIZ";
            //    rowindex++;
            //}


            //sheet.Cells[rowindex, 1, rowindex, 1].Style.Font.Bold = true;
            //sheet.Cells[rowindex, 1].Value = "Totales";
            //sheet.Cells[rowindex, 2].Value = totalPositivo;
            //sheet.Cells[rowindex, 3].Value = totalEnEspera;
            var bordesTotales = sheet.Cells[rowindex, 1, rowindex, columnaFinal].Style.Border;
            bordesTotales.Top.Style = bordesTotales.Right.Style = bordesTotales.Bottom.Style = bordesTotales.Left.Style = ExcelBorderStyle.Medium;


            return excelPackage;
        }

        #endregion
    }
}