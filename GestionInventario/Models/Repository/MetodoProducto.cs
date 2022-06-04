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
        public List<Producto> ListaProductos()
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
            lst = db.Producto.ToList();

            return lst;
        }

        public ResponseModel Add(Producto producto)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
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
    }
}