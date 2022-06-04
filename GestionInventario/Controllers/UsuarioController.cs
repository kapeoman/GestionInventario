using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestionInventario.Models;
using GestionInventario.Models.View;
using GestionInventario.Models.Repository;

namespace GestionInventario.Controllers
{
    public class UsuarioController : Controller
    {
        public MetodoUsuario metodoUsuario = new MetodoUsuario();
        // GET: Usuario
        
        public ActionResult Index()
        {
            using (GestionInventarioEntities db = new GestionInventarioEntities())
            {
                //List<UsuarioView> usuarioViews = metodoUsuario.ListaUsuarios();

                return View(metodoUsuario.ListaUsuarios());
            }
            
        }

        public ActionResult Add()
        {
            UsuarioView usuarioView = new UsuarioView();
            usuarioView.Sexos = metodoUsuario.ListaSexo();
            return View(usuarioView);
        }
        [HttpPost]
        public ActionResult Add(UsuarioView usuarioView)
        {
            ResponseModel response = new ResponseModel();
            
            if (usuarioView.RutCuerpo < 1000000)
            {
                response.Error = true;
                response.Mensaje = "Los parametros no son validos";
                //usuarioView.Sexos = metodoUsuario.ListaSexo();
                return Json(response);
            }
            else
            {
                response = metodoUsuario.AddUser(usuarioView);
                if (response.Error == false)
                {
                    //ViewBag.Mensaje = response.Mensaje;
                    return Json(response);
                }
                else
                {
                    //ViewBag.Mensaje = estado.Mensaje;
                    //usuarioView.Sexos = metodoUsuario.ListaSexo();
                    return Json(response);
                }
                
            }
            //UsuarioView usuarioView = new UsuarioView();
            
        }
        [HttpPost]
        public ActionResult Delete(string Id)
        {
            
            var response = metodoUsuario.DeleteUser(Guid.Parse(Id));
            return Json(response);
        }
        public ActionResult modificar(string Id)
        {
            var usuarioView = metodoUsuario.GetUsuario(Guid.Parse(Id));
            return View(usuarioView);
        }
    }
}