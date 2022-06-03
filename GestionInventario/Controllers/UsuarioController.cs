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
            if (!ModelState.IsValid)
            {
                usuarioView.Sexos = metodoUsuario.ListaSexo();
                return View(usuarioView);
            }
            else
            {
                var estado = metodoUsuario.AddUser(usuarioView);
                if (estado.Error == false)
                {
                    return RedirectToAction("Index", "Usuario");
                }
                else
                {
                    usuarioView.Sexos = metodoUsuario.ListaSexo();
                    return View(usuarioView);
                }
                
            }
            //UsuarioView usuarioView = new UsuarioView();
            
        }
        public ActionResult Delete()
        {
            return View();
        }
        public ActionResult modificar()
        {
            return View();
        }
    }
}