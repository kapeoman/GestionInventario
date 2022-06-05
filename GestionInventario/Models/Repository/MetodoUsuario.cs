using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GestionInventario.Models.View;
using GestionInventario.Models;
using Microsoft.Exchange.WebServices.Data;
using System.Net.Mail;
using System.Net;

namespace GestionInventario.Models.Repository
{
    public class MetodoUsuario
    {
        Usuario oUser = (Usuario)HttpContext.Current.Session["User"];
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

        public Usuario GetUsuario()
        {
            return oUser;
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
                               RutCuerpo = d.RunCuerpo,
                               RutDigito = d.RunDigito,
                               Nombre = d.Nombres,
                               ApellidoPaterno = d.ApellidoPaterno,
                               ApellidoMaterno = d.ApellidoMaterno,
                               Email = d.Email,
                               Sexo = d.Sexo.Codigo,
                               FechaNacimiento = d.FechaNacimiento
                           }).SingleOrDefault();

            usuarioView.Sexos = db.Sexo.ToList();

            return usuarioView;
        }

        public ResponseModel ModificarUser(UsuarioView usuarioView)
        {

            ResponseModel response = new ResponseModel();
            try
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    Persona persona = db.Persona.Where(x => x.Id == usuarioView.Id).SingleOrDefault();

                    persona.RunCuerpo = usuarioView.RutCuerpo;
                    persona.RunDigito = usuarioView.RutDigito;
                    persona.Nombres = usuarioView.Nombre;
                    persona.ApellidoPaterno = usuarioView.ApellidoPaterno;
                    persona.ApellidoMaterno = usuarioView.ApellidoMaterno;
                    persona.FechaNacimiento = usuarioView.FechaNacimiento;
                    persona.Email = usuarioView.Email;
                    persona.SexoCodigo = short.Parse(usuarioView.Sexo.ToString());

                    db.Entry(persona).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    dbContextTransaction.Commit();
                }


                response.Error = false;
                response.Mensaje = "Se a modificado correctamente al usuario";
                return response;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Mensaje = "No se a podido realizar la modificacion " + ex;
                return response;
            }
            
            


            
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

        public ResponseModel ResetearPass(Guid id)
        {
            ResponseModel response = new ResponseModel();
            try
            {

                using (var dbContextTransaction = db.Database.BeginTransaction())
                {

                    //Persona persona = db.Persona.Where(x => x.Id == id).SingleOrDefault();

                    Usuario usuario = db.Usuario.Where(x => x.Id == id).SingleOrDefault();
                    usuario.Password = usuario.Persona.RunCuerpo.ToString();
                    usuario.PassModificada = false;
                    db.Entry(usuario).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                    dbContextTransaction.Commit();

                    response.Error = false;
                    response.Mensaje = "Se Restablecio la contraseña, se ha enviado un correo para el usuario";

                    envioEmailPass(usuario);
                }
                
                return response;
            }
            catch (Exception ex)
            {
                response.Mensaje = "No se pudo eliminar " + ex;
                return response;
            }
        }

        public ResponseModel CambiarPass(string passAntigua, string passNueva)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (oUser.Password == passAntigua)
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {

                        oUser.Password = passNueva;
                        oUser.PassModificada = true;
                        db.Entry(oUser).State = System.Data.Entity.EntityState.Modified;

                        db.SaveChanges();
                        dbContextTransaction.Commit();

                        response.Error = false;
                        response.Mensaje = "Se realizo el cambio de la contraseña con exito";

                    }

                    return response;
                }
                response.Error = true;
                response.Mensaje = "Chanchito Triste";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensaje = "No se pudo cambiar la contraseña " + ex;
                return response;
            }
        }


        public string envioEmailPass(Usuario usuario)
        {
            try
            {
                
                var asunto = "Contraseña Restablecida";
                var desc1 = "Su Contraseña ha sido restablecida";
                var nombreCompleto = usuario.Persona.Nombre;
               

                var body = "<html><body><font face=" + "'Arial'" + "size=" + "'2'>";
                body +=
                    "<div style=" + "'color:#008272;font-size:36px;font-family:SegoeUI;text-align:center;margin:0;padding:0;line-height:48px;letter-spacing:0.5px;font-style: initial;'>" +
                         desc1 + "</div>" + "<br><br>" +
                        "<div style=" + "'margin-left: 0px;'>" +
                        "<strong>Estimad@ " + nombreCompleto + "</ strong>" + "<br>" +
                        "Junto con saludar, le informo que se a restablecido su contraseña <br>" +
                        "<strong>Usuario: </strong>" + usuario.Persona.RunCuerpo + "-" + usuario.Persona.RunDigito + "<br>" +
                        "<strong>Contraseña: </strong>" + usuario.Password + "<br>";

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("gestion.test.2020@gmail.com");
                mail.To.Add(new MailAddress(usuario.Persona.Email));
                mail.Subject = asunto;
                mail.Body = body;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.Normal;

                SmtpClient smtpClient = new SmtpClient();

                smtpClient.Host = "smtp.gmail.com";
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;

                NetworkCredential usercredential = new NetworkCredential("gestion.test.2020@gmail.com", "zejvhtbwtjayfmyo");

                //zejvhtbwtjayfmyo

                smtpClient.Credentials = usercredential;
                smtpClient.Send(mail);

                //ENVIO DE CORREO MEDIANTE WEB SERVICE
                //ExchangeService myservice = new ExchangeService(ExchangeVersion.Exchange2013);
                //myservice.Credentials = new WebCredentials("gestion.test.2020@gmail.com", "12345gestion");
                //myservice.Url = new Uri("https://mail.tencent.com/EWS/Exchange.asmx");
                //EmailMessage emailMessage = new EmailMessage(myservice);
                //emailMessage.Subject = asunto;
                //emailMessage.Body = body;
                //emailMessage.Body.BodyType = BodyType.HTML;
                
                //emailMessage.ToRecipients.Add(usuario.Persona.Email);


                //emailMessage.Send();

                return "CORREO ENVIADO CORRECTAMENTE";
            }
            catch (Exception ex)
            {
                var error = ex.Message.Contains("401") ? "NO SE PUDO ENVIAR EL CORREO: VERIFIQUE LAS CREDENCIALES DEL EMISOR DE CORREOS EN CONFIGURACION DE CUENTA" : ex.Message;
                return error;
            }
        }
    }
}