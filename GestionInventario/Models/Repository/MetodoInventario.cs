using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GestionInventario.Models;
using GestionInventario.Models.View;

namespace GestionInventario.Models.Repository
{
    public class MetodoInventario
    {
        Usuario oUser = (Usuario)HttpContext.Current.Session["User"];
        GestionInventarioEntities db = new GestionInventarioEntities();

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
                response.Mensaje = "Se ha producido un error";
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
                }
                else
                {
                    inventarioHistorico.Fecha = DateTime.Now;
                    inventarioHistorico.StockAnterior = inventarioHistorico.StockNuevo;
                    inventarioHistorico.StockNuevo = inventario.Stock;
                    inventarioHistorico.PrecioUnitario = inventario.Producto.precioUnitario.Value;
                    inventarioHistorico.Ingreso = inventarioHistorico.StockNuevo < inventarioHistorico.StockAnterior ? false : true;
                    inventarioHistorico.UsuarioId = oUser.Id;
                }
                //Inventario inventario = db.Inventario.Where(x => x.Codigo == inventarioNew.Codigo).SingleOrDefault();

                db.InventarioHistorico.Add(inventarioHistorico);
                db.SaveChanges();
                //dbContextTransaction.Commit();


                //}
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
    }
}