using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestionInventario.Models;
using GestionInventario.Models.Repository;
using GestionInventario.Models.View;
using OfficeOpenXml;

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

        public ActionResult ListaInventarioHistorico()
        {
            List<InventarioHistorico> inventarioHistorico = metodoInventario.ListaInventarioHistoricos();
            return View(inventarioHistorico);
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
        [HttpPost]
        public ActionResult Delete(int Codigo)
        {
            var response = metodoInventario.EliminarInventario(Codigo);
            return Json(response);
        }
        [HttpPost]
        public ActionResult Activar(int Codigo)
        {
            var response = metodoInventario.ReactivarInventario(Codigo);
            return Json(response);
        }

        public ActionResult repoInventarioHistoricoFull()
        {

            string user = User.Identity.Name;
            using (ExcelPackage repoExcel = metodoInventario.ReporteFullHistorico())
            {
                var stream = new MemoryStream();
                repoExcel.SaveAs(stream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = "Reporte General Historico " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + ".xlsx";
                stream.Position = 0;
                return File(stream, contentType, fileName);
            }
        }
        public ActionResult repoInventarioFull()
        {

            string user = User.Identity.Name;
            using (ExcelPackage repoExcel = metodoInventario.ReporteFullInventario())
            {
                var stream = new MemoryStream();
                repoExcel.SaveAs(stream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = "Reporte General Inventario " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + ".xlsx";
                stream.Position = 0;
                return File(stream, contentType, fileName);
            }
        }
    }
}