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
    public class AdvertisingController : Controller
    {

        public PartialViewResult _Advertising()
        {
            using (var mo = new Models.Hotel_Amazonian_WillowEntities())
            {
                ViewData["Advertisng"] = mo.Publicidad.Where(p => p.estado == 3).Include(p => p.Imagen1).ToList();

            }
            return PartialView();
        }//_Advertising

        public bool isNotLogin()
        {
            if (Session != null && Session["username"] != null)
                return false;
            return true;
        }//isNotLogin

        public ActionResult showAdvertising()
        {
            if (!isNotLogin())
            {
                using (var mo = new Models.Hotel_Amazonian_WillowEntities())
                {
                    ViewData["Advertisng"] = mo.Publicidad.Where(p => p.estado == 3).Include(p => p.Imagen1).ToList();
                }
                return View();
            }
            return RedirectToAction("Login", "Admin");
        }//showAdvertising

        public ActionResult insertAdvertising()
        {
            if (!isNotLogin())
            {
                return View();
            }
            return RedirectToAction("Login", "Admin");
        }//insertAdvertising

        [HttpGet]
        public ActionResult updateAdvertising(int? id)
        {
            if (!isNotLogin())
            {
                using (var mo = new Models.Hotel_Amazonian_WillowEntities())
                {
                    ViewData["Advertisng"] = mo.Publicidad.Where(p => p.id == id).Include(p => p.Imagen1).FirstOrDefault();
                }

                return View();
            }
            return RedirectToAction("Login", "Admin");
        }//updateAdvertising

        public ActionResult deleteAdvertising(int id, String url, int imagenVieja)
        {
            if (!isNotLogin())
            {
                Models.Publicidad publicidad = new Models.Publicidad();
                publicidad.id = id;
                publicidad.url = url;
                publicidad.estado = 4;
                publicidad.imagen = imagenVieja;

                using (var mo = new Models.Hotel_Amazonian_WillowEntities())
                {
                    mo.Entry(publicidad).State = EntityState.Modified;
                    mo.SaveChanges();

                    ViewData["Advertisng"] = mo.Publicidad.Where(p => p.estado == 3).Include(p => p.Imagen1).ToList();
                }

                TempData["tituloModal"] = "Atención";
                TempData["message"] = "La publicidad se ha eliminado exitosamente.";
                return View("showAdvertising");
            }
            return RedirectToAction("Login", "Admin");
        }//deletePromotion

        public ActionResult insertAdvertisingDB(String url, HttpPostedFileBase img)
        {
            if (!isNotLogin())
            {
                Models.Publicidad publicidad = new Models.Publicidad();
                publicidad.url = url;
                publicidad.id = 0;
                publicidad.estado = 3;

                if (img != null)
                {
                    publicidad.imagen = imagen(img);
                    if (publicidad.imagen == -1 || publicidad.imagen == 0)
                    {
                        TempData["tituloModal"] = "Oops!!";
                        TempData["error"] = "Error al procesar la imagen.";
                        return View("insertAdvertising");
                    }
                }
                else
                {
                    TempData["tituloModal"] = "Oops!!";
                    TempData["error"] = "No se ha seleccionado ninguna imagen.";
                    return View("insertAdvertising");
                }//if-else

                using (var mo = new Models.Hotel_Amazonian_WillowEntities())
                {
                    mo.Entry(publicidad).State = EntityState.Added;
                    mo.SaveChanges();

                    ViewData["Advertisng"] = mo.Publicidad.Where(p => p.estado == 3).Include(p => p.Imagen1).ToList();
                }

                TempData["tituloModal"] = "Confirmación";
                TempData["success"] = "La publicidad se ha insertado exitosamente.";
                return View("showAdvertising");
            }
            return RedirectToAction("Login", "Admin");
        }//insertAdvertisingDB

        public ActionResult updateAdvertisingDB(int id, String url, int imagenVieja, HttpPostedFileBase img)
        {
            if (!isNotLogin())
            {
                Models.Publicidad publicidad = new Models.Publicidad();
                publicidad.id = id;
                publicidad.url = url;
                publicidad.estado = 3;

                if (img != null)
                {
                    publicidad.imagen = imagen(img);
                    if(publicidad.imagen == -1 || publicidad.imagen == 0)
                    {
                        TempData["tituloModal"] = "Oops!!";
                        TempData["error"] = "Error al procesar la imagen.";
                        return View("showAdvertising");
                    }
                }
                else
                {
                    publicidad.imagen = imagenVieja;
                }//if-else

                using (var mo = new Models.Hotel_Amazonian_WillowEntities())
                {
                    mo.Entry(publicidad).State = EntityState.Modified;
                    mo.SaveChanges();

                    ViewData["Advertisng"] = mo.Publicidad.Where(p => p.estado == 3).Include(p => p.Imagen1).ToList();
                }

                TempData["tituloModal"] = "Atención";
                TempData["message"] = "La publicidad se ha actualizado exitosamente.";

                return View("showAdvertising");
            }
            return RedirectToAction("Login", "Admin");
        }//updateAdvertisingDB

        public int imagen(HttpPostedFileBase img)
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
        }//imagen

    }//class
}//namespace