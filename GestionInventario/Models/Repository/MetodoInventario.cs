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
                    
                    Inventario inventario = db.Inventario.Where(x=>x.Codigo == inventarioNew.Codigo).SingleOrDefault();

                    inventario.Stock = inventarioNew.Stock;

                    db.SaveChanges();
                    dbContextTransaction.Commit();

                    response.Error = false;
                    response.Mensaje = "Se ha Guardado el stock correctamente";
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Mensaje = "Se ha producido un error " + ex ;
                return response;
            }
        }
    }
}