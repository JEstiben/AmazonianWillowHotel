﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Data.Entity.Core.Objects;
using System.Net.Mail;
using System.Net;

namespace AmazoniamWillowHotel.Controllers
{
    public class ReservationController : Controller
    {

        public ActionResult OnlineBook()
        {
            using (var mo = new Models.Hotel_Amazonian_WillowEntities())
            {
                ViewData["types"] = mo.Tipo_Habitacion.ToList();
            }
            return View();
        }//OnlineBook

        public ActionResult ReservationClient(int numero, String titulo, String descripcion, double tarifa, int imagen, String fechaLlegada, String fechaSalida)
        {
            Models.CheckAvailability_Result checkAvailability = new Models.CheckAvailability_Result();

            checkAvailability.numero = numero;
            checkAvailability.titulo = titulo;
            checkAvailability.descripcion = descripcion;
            checkAvailability.tarifa = tarifa;
            checkAvailability.imagen = imagen;
            ViewData["fechaLlegada"] = fechaLlegada;
            ViewData["fechaSalida"] = fechaSalida;
            ViewData["Monto"] = tarifa * calcularDias(fechaLlegada, fechaSalida);

            using (var mo = new Models.Hotel_Amazonian_WillowEntities())
            {
                ViewData["imagen"] = mo.Imagen.Where(x => x.id_Imagen == imagen).ToList();
            }

            return View(checkAvailability);
        }//ReservationClient

        public ActionResult ReservationInformation(String nombreCompleto, String correo_, int numeroReserva, float montoR)
        {
            Models.MakeReservation_Result makeReservation = new Models.MakeReservation_Result();

            makeReservation.nombre = nombreCompleto;
            makeReservation.correo = correo_;
            makeReservation.numeroReserva = numeroReserva;
            makeReservation.monto = montoR;

            correo(nombreCompleto, correo_, numeroReserva, montoR);

            TempData["tituloModal"] = "Confirmación";
            TempData["success"] = "La reservación se ha procesado con éxito.";

            return View(makeReservation);
        }//ReservationInformation

        public ActionResult RoomNotAvailable()
        {
            return View();
        }//RoomNotAvailable

        public JsonResult checkAvailability(int TipoHabitacion, DateTime fechaLlegada, DateTime fechaSalida)
        {
            using (var mo = new Models.Hotel_Amazonian_WillowEntities())
            {
                Models.CheckAvailability_Result checkAvailability1 = new Models.CheckAvailability_Result();

                ObjectResult<Models.CheckAvailability_Result> result = mo.CheckAvailability(TipoHabitacion, fechaLlegada, fechaSalida);
                
                foreach (Models.CheckAvailability_Result checkAvailability in result)
                    checkAvailability1 = checkAvailability;

                if(checkAvailability1.descripcion == "No hay Habitaciones")
                {
                    result = mo.CheckAvailability(0, fechaLlegada, fechaSalida);

                    foreach (Models.CheckAvailability_Result checkAvailability in result)
                        checkAvailability1 = checkAvailability;

                    if (checkAvailability1.descripcion != "No hay Habitaciones")
                    {
                        TempData["tituloModal"] = "Atención";
                        TempData["message"] = "No hay habitaciones del tipo especificado, pero tenemos la siguiente sugerencia";
                    }
                    else
                    {
                        TempData["tituloModal"] = "Oops!!";
                        TempData["error"] = "Lo sentimos en este momento no tenemos habitaciones disponibles para las fechas ingresadas.";
                    }
                }
                else
                {
                    TempData["tituloModal"] = "Confirmación";
                    TempData["success"] = "Se ha encontrado la siguiente habitación disponible.";
                }

                Thread.Sleep(3000);
                return Json(checkAvailability1, JsonRequestBehavior.AllowGet);
            }
        }//checkAvailability

        public JsonResult makeReservation(String identificacion, String nombre, String apellidos, String correo, String tarjeta, String fechaV, String codigoS, int numero, String fechaLlegada, String fechaSalida, float monto)
        {
            using (var mo = new Models.Hotel_Amazonian_WillowEntities())
            {
                DateTime fechaLlegada1 = DateTime.Parse(fechaLlegada);
                DateTime fechaSalida1 = DateTime.Parse(fechaSalida);
                ObjectResult<Models.MakeReservation_Result> result = mo.MakeReservation(identificacion, nombre, apellidos, correo, tarjeta, fechaV, codigoS,numero, fechaLlegada1, fechaSalida1, monto);

                Models.MakeReservation_Result makeReservation1 = new Models.MakeReservation_Result();
                foreach (Models.MakeReservation_Result makeReservation in result)
                    makeReservation1 = makeReservation;

                Thread.Sleep(3000);
                return Json(makeReservation1, JsonRequestBehavior.AllowGet);
            }
        }//makeReservation

        public JsonResult freeRoom(int numero)
        {
            using (var mo = new Models.Hotel_Amazonian_WillowEntities())
            {
                mo.FreeRoom(numero);

                TempData["tituloModal"] = "Atención";
                TempData["message"] = "La habitación que teniamos pre-reservada para usted se ha liberado.";

                return Json("Cancelado", JsonRequestBehavior.AllowGet);
            }
        }//freeRoom

        public JsonResult loadData(String Identificacion)
        {
            using (var mo = new Models.Hotel_Amazonian_WillowEntities())
            {
                Models.Reservacion reservation = new Models.Reservacion();
                reservation = mo.Reservacion.Where(x => x.identificacion == Identificacion).FirstOrDefault();

                if (reservation != null)
                {
                    return Json(reservation.nombre + "^" + reservation.apellidos + "^" + reservation.correo, JsonRequestBehavior.AllowGet);
                    //return Json(reservation, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
        }//loadData

        public void correo(String nombreCompleto, String correo_, int numeroReserva, float montoR)
        {
            MailMessage email = new MailMessage();
            email.To.Add(new MailAddress(correo_));
            email.From = new MailAddress("hotelamazonianwillow@gmail.com");
            email.Subject = "Reservación Comprobante ( " + DateTime.Now.ToString("MM/dd/yyy hh:mm:ss") + " ) ";
            email.Body = "<p>Reservación realizada!</p><br/><p>Gracias "+nombreCompleto+"!! Su reservación fue realizada exitosamente.</p><br/><p>Número de reservación: "+numeroReserva+ "</p><br/><p>Monto de la reservación: "+montoR+"</p><br/><p>Gracias por preferirnos!</p>";
            email.IsBodyHtml = true;
            email.Priority = MailPriority.Normal;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 25;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("hotelamazonianwillow@gmail.com", "HAW_2019");

            try
            {
                smtp.Send(email);
            }
            catch (Exception ex)
            {
                email.Dispose();
            }
        }//correo

        public int calcularDias(String fechaLlegada, String fechaSalida)
        {
            DateTime fechaLlegada1 = DateTime.Parse(fechaLlegada);
            DateTime fechaSalida1 = DateTime.Parse(fechaSalida);

            TimeSpan timeSpan = fechaSalida1 - fechaLlegada1;

            return timeSpan.Days;

        }//calcularDias

    }//class
}//namespace
