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

            return View(metodoProducto.ListaProductos(false));
        }

        public ActionResult IndexProductoEliminado()
        {

            return View(metodoProducto.ListaProductos(true));
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

        public ActionResult Edit(int codigo)
        {
            Producto producto = metodoProducto.GetProducto(codigo);
            return PartialView("_Edit",producto);
        }
        [HttpPost]
        public ActionResult Edit(Producto producto)
        {
            var response = metodoProducto.Edit(producto);
            return Json(response);
        }
        [HttpPost]
        public ActionResult Delete(int codigo, int Activo)
        {
            var response = metodoProducto.Delete(codigo, Activo);
            return Json(response);
        }
    }
}