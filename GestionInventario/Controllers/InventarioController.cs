using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestionInventario.Models;
using GestionInventario.Models.Repository;
using GestionInventario.Models.View;

namespace GestionInventario.Controllers
{
    public class InventarioController : Controller
    {
        MetodoInventario metodoInventario = new MetodoInventario();
        // GET: Inventario
        public ActionResult Index()
        {
            InventarioView inventario = metodoInventario.ListaInventario();

            return View(inventario);
        }

        public ActionResult Edit(int Codigo)
        {
            Inventario inventario = metodoInventario.GetInventario(Codigo);
            //var response = 
            return PartialView(inventario);
        }
        [HttpPost]
        public ActionResult Edit(Inventario inventario)
        {
            //Inventario inventario = metodoInventario.GetInventario(Codigo);
            var response = metodoInventario.EditarInventario(inventario);
            return Json(response);
        }
    }
}