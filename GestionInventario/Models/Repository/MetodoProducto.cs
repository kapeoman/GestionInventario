using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GestionInventario.Models.View;

namespace GestionInventario.Models.Repository
{
    public class MetodoProducto
    {
        public GestionInventarioEntities db = new GestionInventarioEntities();

        public Producto GetProducto(int codigo)
        {
            Producto producto = db.Producto.Where(x => x.Codigo == codigo).SingleOrDefault();
            return producto;
        }
        public List<Producto> ListaProductos(bool eliminado)
        {
            List<Producto> lst = new List<Producto>();

            //lst = (from d in db.Usuario
            //       select new UsuarioView
            //       {
            //           Id = d.Persona.Id,
            //           Rut = d.Persona.Run,
            //           Nombre = d.Persona.Nombre,
            //           Email = d.Persona.Email,
            //           FechaNacimiento = d.Persona.FechaNacimiento
            //       }).ToList();
            lst = db.Producto.Where(x=>x.Eliminado.Value == eliminado).ToList();

            return lst;
        }

        public ResponseModel Add(Producto producto)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    producto.Eliminado = false;
                    db.Producto.Add(producto);
                    Inventario inventario = (new Inventario
                    {
                        CodigoProducto = producto.Codigo,
                        Producto = producto,
                        Stock = 0,
                        Eliminado = false,

                    });
                    db.Inventario.Add(inventario);
                    db.SaveChanges();
                    dbContextTransaction.Commit();
                }
                response.Error = false;
                response.Mensaje = "Producto se a guardado correctamente";
                return response;
            }
            catch (Exception ex)
            {
                response.Error = false;
                response.Mensaje = "Se a producido un error al guardar el producto " + ex;
                return response;
            }

        }

        public ResponseModel Edit(Producto productoNew)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    db.Entry(productoNew).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    dbContextTransaction.Commit();
                }
                response.Error = false;
                response.Mensaje = "Se ha modificado correctamente el producto";
                return response;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Mensaje = "Ocurrio un error " + ex.Message;
                return response;
            }
        }

        public ResponseModel Delete(int Codigo, int Activo)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var dbContextTransection = db.Database.BeginTransaction())
                {
                    Producto producto = db.Producto.Where(x => x.Codigo == Codigo).SingleOrDefault();
                    Inventario inventario = db.Inventario.Where(x => x.CodigoProducto == producto.Codigo).SingleOrDefault();
                    if (Activo == 1)
                    {
                        inventario.Eliminado = false;
                        producto.Eliminado = false;
                    }
                    else
                    {
                        inventario.Eliminado = true;
                        producto.Eliminado = true;
                    }
                    
                    db.Entry(inventario).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(producto).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    dbContextTransection.Commit();
                }
                response.Error = false;
                response.Mensaje = "Se a eliminado correctamente el producto";
                return response;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Mensaje = "No se ha podido eliminar el producto " + ex.Message;
                return response;
            }
        }
    }
}