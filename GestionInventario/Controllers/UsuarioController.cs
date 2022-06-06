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

                return View(metodoUsuario.ListaUsuarios(true));
            }
            
        }

        public ActionResult UsuariosInactivos()
        {
            using (GestionInventarioEntities db = new GestionInventarioEntities())
            {
                //List<UsuarioView> usuarioViews = metodoUsuario.ListaUsuarios();

                return View(metodoUsuario.ListaUsuarios(false));
            }
        }

        public ActionResult Add()
        {
            UsuarioView usuarioView = new UsuarioView();
            usuarioView.Sexos = metodoUsuario.ListaSexo();
            usuarioView.rols = metodoUsuario.ListaRoles();
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
                
                for (int i = 0; i < metodoUsuario.ListaRoles().Count; i++)
                {
                    var rol = int.Parse(Request.Form["RolList[" + i + "]"].ToString());
                    usuarioView.Rol.Add(rol);
                }

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
        public ActionResult Delete(string Id, string Activo)
        {
            
            var response = metodoUsuario.DeleteUser(Guid.Parse(Id), int.Parse(Activo));
            return Json(response);
        }
        public ActionResult modificar(string Id)
        {
            var usuarioView = metodoUsuario.GetUsuario(Guid.Parse(Id));
            return View(usuarioView);
        }
        [HttpPost]
        public ActionResult modificar(UsuarioView usuarioView)
        {
            var response = metodoUsuario.ModificarUser(usuarioView);
            return Json(response);
        }

        public ActionResult recetearPass(string Id)
        {
            var response = metodoUsuario.ResetearPass(Guid.Parse(Id));
            return Json(response);
        }

        public ActionResult cambiarPass()
        {
            
            return View();
        }
        [HttpPost]
        public ActionResult cambiarPass(string passActual, string passNueva)
        {
            var response = metodoUsuario.CambiarPass(passActual, passNueva);


            return Json(response);
        }

        public ActionResult Rol()
        {
            List<Rol> roles = metodoUsuario.ListaRoles();
            return View(roles);
        }

        public ActionResult AddRol()
        {
            Rol rol = new Rol();
            return PartialView("_AddRol", rol);
        }
        [HttpPost]
        public ActionResult AddRol(Rol rol)
        {
            ResponseModel response = metodoUsuario.AddRol(rol);
            return Json(response);
        }

        public ActionResult ModificarRol(int Codigo)
        {
            Rol rol = metodoUsuario.GetRol(Codigo);
            return PartialView("_ModificarRol", rol);
        }
        [HttpPost]
        public ActionResult ModificarRol(Rol rol)
        {
            ResponseModel response = metodoUsuario.ModificarRol(rol);
            return Json(response);
        }

        public ActionResult DeleteRol(int Codigo)
        {
            ResponseModel response = metodoUsuario.DeleteRol(Codigo);
            return Json(response);
        }
    }
}