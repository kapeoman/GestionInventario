using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GestionInventario.Models.View
{
    public class UsuarioView
    {
        public Guid Id { get; set; }
        //[Required]
        //[Range(1000000, 99999999)]
        //[RegularExpression("(^[0-9]+$)", ErrorMessage = "Solo se permiten números")]
        public int RutCuerpo { get; set; }
        //[Required]
        //[RegularExpression("(^[0-9Kk]+$)", ErrorMessage = "Solo se permiten números y letra k")]
        public string RutDigito { get; set; } = "0";
        public string Rut { get; set; }
        //[Required]
        public string Nombre { get; set; }
        //[Required]
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        //[Required]
        //[EmailAddress]
        public string Email { get; set; }
        //[Required]
        public DateTime? FechaNacimiento { get; set; }
        //[Required]
        public int Sexo { get; set; }
        public List<int> Rol { get; set; }
        public List<Sexo> Sexos { get; set; } 
        public List<Rol> rols { get; set; }

        
        public UsuarioView()
        {
            Rol = new List<int>();
        }
    }
    

}