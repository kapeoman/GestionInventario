using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionInventario.Models.View
{
    public class InventarioView
    {       
        public List<Inventario> inventarioActivo { get; set; }
        public List<Inventario> inventarioInactivo { get; set; }


    }
}