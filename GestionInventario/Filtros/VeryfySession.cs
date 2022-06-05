using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestionInventario.Controllers;
using GestionInventario.Models;

namespace GestionInventario.Filtros
{
    public class VeryfySession : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            var oUser = (Usuario)HttpContext.Current.Session["User"];
            
            //var permisos = (Perfil)HttpContext.Current.Session["Perfil"];
            if (oUser == null)
            {
                if (filterContext.Controller is AccesoController == false)
                {
                    filterContext.HttpContext.Response.Redirect("~/Acceso/Index");
                }
            }
            else
            {
                
                if (filterContext.Controller is AccesoController == true)
                {
                    filterContext.HttpContext.Response.Redirect("~/Home/Index");
                                                          
                }
                
            }

            base.OnActionExecuting(filterContext);
        }
    }
}