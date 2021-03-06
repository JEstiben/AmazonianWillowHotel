using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.Entity.Core.Objects;

namespace AmazoniamWillowHotel.Controllers
{
    public class AdminController : Controller
    {

        public ActionResult Login()
        {
            if (isNotLogin())
                return View();
            else return View("Index");
        }

        public ActionResult Index()
        {
            if (isNotLogin())
            {
                if (Request.Form["username"] != null && Request.Form["password"] != null)
                {
                    String username = Request.Form["username"];
                    String password = Request.Form["password"];

                    using (var model = new Models.Hotel_Amazonian_WillowEntities())
                    {
                        if (model.Administrador.Where(x => x.correo == username && x.contrasenna == password).FirstOrDefault() != null)
                        {
                            Models.Administrador administrador = model.Administrador.Where(x => x.correo == username && x.contrasenna == password).FirstOrDefault();
                            Session["username"] = username;
                            Session["nombre"] = administrador.nombre;
                            return View();
                        }
                        else
                        {
                            ViewData["error"] = "Nombre de usuario o contraseña incorrecta";
                            return View("Login");
                        }
                    }
                }
                else
                {
                    ViewData["error"] = "Debe iniciar sesión";
                    return View("Login");
                }
            }
            return View();
        }

        public ActionResult Exit() {
            Session["username"] = null;
            return View("Login");
        }

        public bool isNotLogin() {
            if (Session != null && Session["username"] != null)
                return false;
            return true;
        }

        public ActionResult InsertFacilitie()
        {
            using (var mo = new Models.Hotel_Amazonian_WillowEntities())
            {
                ViewData["Status"] = new SelectList(mo.Estado.ToList(), "Id_Estado", "Nombre");
            }
            //ViewData["Status"] = new SelectList(model.getStatus(), "Id_Estado", "Nombre"); ;
            return View();
        }

        public ActionResult seeAvailableDay()
        {

            return View();
        }

        public JsonResult getAvailableDay()
        {
            var mo = new Models.Hotel_Amazonian_WillowEntities();

            return Json(mo.getRoomsDay(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckAvailability()
        {
            using (var mo = new Models.Hotel_Amazonian_WillowEntities())
            {
                ViewData["types"] = mo.Tipo_Habitacion.ToList();
            }
            return View();
        }

        public JsonResult consultarDisponibilidad(int TipoHabitacion, String fechaLlegada, String fechaSalida)
        {
            var mo = new Models.Hotel_Amazonian_WillowEntities();

            DateTime llegada = DateTime.Parse(fechaLlegada);
            DateTime salida = DateTime.Parse(fechaSalida);


            return Json(mo.CheckRoomsAvailable(llegada, salida, TipoHabitacion), JsonRequestBehavior.AllowGet);
        }

        public ActionResult insertPromotionView()
        {

            return View();
        }

        [HttpGet]
        public ActionResult updatePromotionView(int? id)
        {

            using (var mo = new Models.Hotel_Amazonian_WillowEntities())
            {
                ViewData["Habitaciones"] = mo.Tipo_Habitacion.ToList();

                ViewData["Promotion"] = mo.Promocion.Where(p => p.id == id).Include(p => p.Tipo_Habitacion).Include(p => p.Tipo_Habitacion.Imagen1).FirstOrDefault();
            }

            return View();
        }

        public ActionResult showPromotion()
        {
            using (var mo = new Models.Hotel_Amazonian_WillowEntities())
            {

                ViewData["Promotions"] = mo.Promocion.Include(p => p.Tipo_Habitacion).Include(p => p.Tipo_Habitacion.Imagen1).ToList();
            }
            return View();
        }

        public JsonResult DeletePromotion(int id)
        {
            var mo = new Models.Hotel_Amazonian_WillowEntities();
            mo.sp_deletePromotion(id);
            return Json("Eliminado", JsonRequestBehavior.AllowGet);
        }

        public JsonResult insertPromotion(string comment, int idescuento, DateTime iFechaInicio, DateTime iFechaFinal, int tipo)
        {
            try
            {

                if (comment != null && idescuento != 0 && tipo != 0
                && iFechaFinal != null && iFechaInicio != null)
                {
                    var mo = new Models.Hotel_Amazonian_WillowEntities();

                    mo.Promocion.Add(new Models.Promocion
                    {
                        descripcion = comment,
                        descuento = idescuento,
                        inicio = iFechaInicio,
                        fin = iFechaFinal,
                        tipoHabitacion = tipo
                    });
                    mo.SaveChanges();
                    return Json("OK", JsonRequestBehavior.AllowGet);
                }//end validation nulls
            }
            catch (Exception e)
            {
                return Json(e.ToString(), JsonRequestBehavior.AllowGet);
            }

            return Json("ERROR", JsonRequestBehavior.AllowGet);
        }//end method

        public JsonResult updatePromotion(int id, string comment, int idescuento, DateTime iFechaInicio, DateTime iFechaFinal, int tipo)
        {

            if (comment != null && idescuento != 0 && tipo != 0
                && iFechaFinal != null && iFechaInicio != null)
            {
                var mo = new Models.Hotel_Amazonian_WillowEntities();
                var promo = mo.Promocion.Where(d => d.id == id).First();
                promo.id = id;
                promo.inicio = iFechaInicio;
                promo.descripcion = comment;
                promo.fin = iFechaFinal;
                promo.tipoHabitacion = tipo;
                promo.descuento = idescuento;

                mo.SaveChanges();
                return Json("OK", JsonRequestBehavior.AllowGet);
            }//end validation nulls

            return Json("ERROR", JsonRequestBehavior.AllowGet);

        }//end method

        public ActionResult ManageRooms()
        {
            if (!isNotLogin())
            {
                using (var mo = new Models.Hotel_Amazonian_WillowEntities())
                {
                    ViewData["AdministrarHabitaciones"] = mo.Tipo_Habitacion.Include(p => p.Habitacion).ToList();
                }
                return View();
            }
            return View("Login");
        }//ManageRooms

        [HttpGet]
        public ActionResult modifyRoomType(int? type) {

            if (!isNotLogin())
            {
                if (type != null)
                {
                    var mo = new Models.Hotel_Amazonian_WillowEntities();
                    ViewData["information"] = mo.Tipo_Habitacion.Where(x => x.id == type).Include(x => x.Imagen1).FirstOrDefault();
                }


                return View();
            }
            return View("Login");
        }

    [HttpGet]
    public ActionResult  updateState(int? type)
    {
            var mo = new Models.Hotel_Amazonian_WillowEntities();


            if (type != null)
            {
                Models.Habitacion habitacionModel = mo.Habitacion.Where(x => x.id == type).FirstOrDefault();
                if (habitacionModel.estado == 1)
                {
                    habitacionModel.estado = 2;
                }
                else if(habitacionModel.estado == 2)
                {
                    habitacionModel.estado = 1;
                }//end else-if

                mo.Entry(habitacionModel).State = EntityState.Modified;

                try
                {
                    mo.SaveChanges();
                }
                catch (Exception ec)
                {
                    Console.WriteLine(ec.Message);
                }

            }//end if
               if (!isNotLogin())
            {
              
                    ViewData["AdministrarHabitaciones"] = mo.Tipo_Habitacion.Include(p => p.Habitacion).ToList();
            }
            return View("ManageRooms");

        }//end updateState

        public ActionResult updateRoomType(int id, String titulo, double rate, String description, int imagenVieja, HttpPostedFileBase img)
        {
            if (isNotLogin())
            {
                Models.Tipo_Habitacion tipo_Habitacion = new Models.Tipo_Habitacion();
                tipo_Habitacion.id = id;
                tipo_Habitacion.titulo = titulo;
                tipo_Habitacion.tarifa = rate;
                tipo_Habitacion.descripcion = description.Replace("\n", "^").Replace("\r", "");

                if (img != null)
                {
                    tipo_Habitacion.imagen = actualizarImagen(img);
                    if (tipo_Habitacion.imagen == -1 || tipo_Habitacion.imagen == 0)
                    {
                        TempData["tituloModal"] = "Oops!!";
                        TempData["error"] = "Error al procesar la imagen.";
                        return View("ManageRooms");
                    }
                }
                else
                {
                    tipo_Habitacion.imagen = imagenVieja;
                }//if-else

                using (var mo = new Models.Hotel_Amazonian_WillowEntities())
                {
                    mo.Entry(tipo_Habitacion).State = EntityState.Modified;
                    mo.SaveChanges();

                    ViewData["AdministrarHabitaciones"] = mo.Tipo_Habitacion.Include(p => p.Habitacion).ToList();
                }

                TempData["tituloModal"] = "Atención";
                TempData["message"] = "El tipo de habitación se ha actualizado exitosamente.";

                return View("ManageRooms");
            }
            return View("Login");
        }//updateRoomType

        public ActionResult reportRoomsView() {

            return View();
        }

        public int actualizarImagen(HttpPostedFileBase img)
        {
            try
            {
                if (img.ContentLength > 0)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(img.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(img.ContentLength);
                    }
                    using (var mo = new Models.Hotel_Amazonian_WillowEntities())
                    {
                        ObjectResult<Models.InsertImage_Result> result = mo.InsertImage(img.FileName, imageData);

                        Models.InsertImage_Result insertImage1 = new Models.InsertImage_Result();
                        foreach (Models.InsertImage_Result insertImage in result)
                            insertImage1 = insertImage;
                        return Convert.ToInt32(insertImage1.id_Imagen);
                    }

                }//if
            }
            catch (Exception ex)
            {
                return -1;
            }//try-catch.
            return 0;
        }//actualizarImagen

        public ActionResult FacilitiesModify()
        {
            using (var mo = new Models.Hotel_Amazonian_WillowEntities())
            {
                ViewData["Facilities"] = mo.Pagina.Where(x => x.nombre == "Facilidades").Include(p => p.Info).Include(p => p.Info.Select(x => x.Imagen1)).ToList();
            }
            return View();
        }


         [HttpPost]
        public ActionResult modifyFacilities(int id, string description) {

            var mo = new Models.Hotel_Amazonian_WillowEntities();
            Models.Info facilitie =  mo.Info.Where(x => x.id == id).FirstOrDefault();
            facilitie.descripcion = description;
            mo.Entry(facilitie).State = EntityState.Modified;

            try
            {
                mo.SaveChanges();
            }
            catch (Exception ec)
            {
                Console.WriteLine(ec.Message);
            }
            TempData["tituloModal"] = "Atención";
            TempData["message"] = "Facilidad actualizada.";
            return View("../Admin/Index");
        }

        public ActionResult ListReservations() {

            using (var mo = new Models.Hotel_Amazonian_WillowEntities())
            {
                var model = mo.Reservacion.Include(x => x.Habitacion1).Include(h => h.Habitacion1.Tipo_Habitacion).ToList();
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult DeleteReservation(int? id) {

            try
            {
                if (id != null)
                {
                    using (var mo = new Models.Hotel_Amazonian_WillowEntities())
                    {
                        var model = mo.Reservacion.Where(x => x.id == id).First();
                        mo.Reservacion.Remove(model);
                        mo.SaveChanges();

                        TempData["tituloModal"] = "Exito";
                        TempData["message"] = "Se eliminó correctamente la reservación.";
                    }
                }
            }
            catch {
                TempData["tituloModal"] = "Error";
                TempData["message"] = "Lo sentimos ocurrió un error.";
            }
            return RedirectToAction("ListReservations");
        }



        [HttpGet]
        public ActionResult ReservationInfo(int? id)
        {

            try
            {
                if (id != null)
                {
                    using (var mo = new Models.Hotel_Amazonian_WillowEntities())
                    {
                        var model = mo.Reservacion.Where(x => x.id == id).Include(x => x.Habitacion1).Include(h => h.Habitacion1.Tipo_Habitacion).First();
                        return View(model);
                    }
                }
                else return RedirectToAction("ListReservations");
            }
            catch
            {
                TempData["tituloModal"] = "Error";
                TempData["message"] = "Lo sentimos ocurrió un error.";
                return RedirectToAction("ListReservations");
            }
            
        }


        public JsonResult getReservationInfo(int id)
        {
            var mo = new Models.Hotel_Amazonian_WillowEntities();

            return Json(mo.getReservationInfo(id), JsonRequestBehavior.AllowGet);
        }

    }//class
}//namespace
