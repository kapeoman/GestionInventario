using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GestionInventario.Models.View;
using GestionInventario.Models;

namespace GestionInventario.Models.Repository
{
    public class MetodoUsuario
    {
        public GestionInventarioEntities db = new GestionInventarioEntities();
        public List<UsuarioView> ListaUsuarios()
        {
            List<UsuarioView> lst = new List<UsuarioView>();

            lst = (from d in db.Usuario
                   select new UsuarioView
                   {
                       Id = d.Persona.Id,
                       Rut = d.Persona.Run,
                       Nombre = d.Persona.Nombre,
                       Email = d.Persona.Email,
                       FechaNacimiento = d.Persona.FechaNacimiento
                   }).ToList();

            return lst;
        }

        public List<Sexo> ListaSexo()
        {
            List<Sexo> lst = db.Sexo.ToList();
            return lst;
        }

        public ResponseModel AddUser(UsuarioView usuarioView)
        {
            ResponseModel responseModel = new ResponseModel();
            responseModel.Error = false;
            try
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    Persona persona = new Persona();
                    persona = db.Persona.Where(x => x.RunCuerpo == usuarioView.RutCuerpo).FirstOrDefault();
                    if (persona == null)
                    {
                        persona = (new Persona
                        {
                            Id = Guid.NewGuid(),
                            RunCuerpo = usuarioView.RutCuerpo,
                            RunDigito = usuarioView.RutDigito,
                            Nombres = usuarioView.Nombre,
                            ApellidoPaterno = usuarioView.ApellidoPaterno,
                            ApellidoMaterno = usuarioView.ApellidoMaterno,
                            Email = usuarioView.Email,
                            SexoCodigo = short.Parse(usuarioView.Sexo.ToString()),
                            FechaNacimiento = usuarioView.FechaNacimiento
                        });

                        Usuario usuario = (new Usuario
                        {
                            Persona = persona,
                            Password = persona.RunCuerpo.ToString(),
                            Activo = false

                        });

                        db.Persona.Add(persona);
                        db.Usuario.Add(usuario);
                        //List<Rol> rol = new List<Rol>();
                        ////usuario = db.Usuario.Where(x => x.Persona.RunCuerpo == 17678911).FirstOrDefault();
                        //rol = db.Rol.ToList();
                        //foreach (var item in rol)
                        //{
                        //    //usuario.Rol.Add(item);
                        //}
                        //db.Usuario.Add(usuario);
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        responseModel.Mensaje = "chanchito feliz =D";
                    }
                    else
                    {
                        responseModel.Error = true;
                        responseModel.Mensaje = "Chanchito Triste T_T";
                    }
                }
                return responseModel;
            }
            catch (Exception ex)
            {
                responseModel.Error = true;
                responseModel.Mensaje = "Chanchito Triste T_T" + ex;
                return responseModel;
            }
            
        }

        public UsuarioView GetUsuario(Guid id)
        {
            Usuario usuario = db.Usuario.Where(x => x.Id == id).First();

            UsuarioView usuarioView = new UsuarioView();

            usuarioView = (from d in db.Persona
                           where d.Id == id
                           select new UsuarioView
                           {
                               Id = d.Id,
                               Rut = d.Run,
                               Nombre = d.Nombres,
                               ApellidoPaterno = d.ApellidoPaterno,
                               ApellidoMaterno = d.ApellidoMaterno,
                               Email = d.Email,
                               Sexo = d.Sexo.Codigo,
                               FechaNacimiento = d.FechaNacimiento
                           }).SingleOrDefault();


            return usuarioView;
        }

        public ResponseModel DeleteUser(Guid id)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {

                    Persona persona = db.Persona.Where(x => x.Id == id).SingleOrDefault();

                    Usuario usuario = db.Usuario.Where(x => x.Id == id).SingleOrDefault();

                    db.Usuario.Remove(usuario);
                    db.Persona.Remove(persona);
                    db.SaveChanges();
                    dbContextTransaction.Commit();

                    response.Error = false;
                    response.Mensaje = "Se elimino correctamente";
                }

                return response;
            }
            catch (Exception ex)
            {
                response.Mensaje = "No se pudo eliminar " + ex;
                return response;
            }
            
        }
    }
}