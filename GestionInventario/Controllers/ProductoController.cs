using GestionInventario.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestionInventario.Models;

namespace GestionInventario.Controllers
{
    public class ProductoController : Controller
    {
        public MetodoProducto metodoProducto = new MetodoProducto();
        // GET: Producto
        public ActionResult Index()
        {

            return View(metodoProducto.ListaProductos());
        }

        public ActionResult Add()
        {
            Producto producto = new Producto();
            return View(producto);
        }
        [HttpPost]
        public ActionResult Add(Producto producto)
        {
            var response = metodoProducto.Add(producto);

            return Json(response);
        }
    }
}