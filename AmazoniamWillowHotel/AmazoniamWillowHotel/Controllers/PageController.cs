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
    public class PageController : Controller
    {

        public ActionResult ModifyPages()
        {
            return View();
        }

        public bool isNotLogin()
        {
            if (Session != null && Session["username"] != null)
                return false;
            return true;
        }//isNotLogin

        public ActionResult ModifyHomePage()
        {
            if (!isNotLogin())
            {
                using (var mo = new Models.Hotel_Amazonian_WillowEntities())
                {
                    ViewData["Home"] = mo.Pagina.Where(x => x.nombre == "Home").Include(p => p.Info).Include(p => p.Info.Select(x => x.Imagen1)).ToList();

                }
                return View();
            }
            return RedirectToAction("Login", "Admin");
        }

        [HttpPost]
        public ActionResult ModifyHomePage(string description, int imagenVieja, HttpPostedFileBase img) {
            if (!isNotLogin())
            {
                var mo = new Models.Hotel_Amazonian_WillowEntities();
                var home = mo.Pagina.Where(x => x.nombre == "Home").Include(p => p.Info).Include(p => p.Info.Select(x => x.Imagen1)).First();
                home.Info.First().descripcion = description.Replace("\n", "^");

                if (img != null)
                {
                    home.Info.First().imagen = actualizarImagen(img);
                    if (home.Info.First().imagen == -1 || home.Info.First().imagen == 0)
                    {
                        TempData["tituloModal"] = "Oops!!";
                        TempData["error"] = "Error al procesar la imagen.";
                        return View("showAdvertising");
                    }
                }
                else
                {
                    home.Info.First().imagen = imagenVieja;
                }//if-else

                mo.SaveChanges();

                TempData["tituloModal"] = "Atención";
                TempData["message"] = "Página Home actualizada.";
                return View("../Admin/Index");
            }
            return RedirectToAction("Login", "Admin");
        }

        public ActionResult ModifyAboutPage()
        {
            using (var mo = new Models.Hotel_Amazonian_WillowEntities())
            {
                ViewData["About"] = mo.Pagina.Where(x => x.nombre == "Sobre Nosotros").Include(p => p.Info).Include(p => p.Info.Select(x => x.Imagen1)).ToList();

            }
            return View();
        }

        [HttpPost]
        public ActionResult ModifyAboutPage(string description)
        {

            var mo = new Models.Hotel_Amazonian_WillowEntities();
            var home = mo.Pagina.Where(x => x.nombre == "Sobre Nosotros").Include(p => p.Info).Include(p => p.Info.Select(x => x.Imagen1)).First();
            home.Info.First().descripcion = description.Replace("\n", "^");

            mo.SaveChanges();

            TempData["tituloModal"] = "Atención";
            TempData["message"] = "Página Sobre Nosotros actualizada.";
            return View("../Admin/Index");
        }

        public ActionResult ModifyHowToGetPage()
        {
            if (!isNotLogin())
            {
                using (var mo = new Models.Hotel_Amazonian_WillowEntities())
                {
                    ViewData["HowToGet"] = mo.Pagina.Where(x => x.nombre == "¿Como Llegar?").Include(p => p.Info).Include(p => p.Info.Select(x => x.Imagen1)).ToList();

                }
                return View();
            }
            return RedirectToAction("Login", "Admin");
        }

        [HttpPost]
        public ActionResult ModifyHowToGetPage(string description)
        {

            var mo = new Models.Hotel_Amazonian_WillowEntities();
            var home = mo.Pagina.Where(x => x.nombre == "¿Como Llegar?").Include(p => p.Info).Include(p => p.Info.Select(x => x.Imagen1)).First();
            home.Info.First().descripcion = description.Replace("\n", "^");

            mo.SaveChanges();

            TempData["tituloModal"] = "Atención";
            TempData["message"] = "Página ¿Como llegar? actualizada.";
            return View("../Admin/Index");
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

    }
}