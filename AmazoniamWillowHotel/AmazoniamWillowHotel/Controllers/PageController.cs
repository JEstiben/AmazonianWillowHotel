using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AmazoniamWillowHotel.Controllers
{
    public class PageController : Controller
    {

        public ActionResult ModifyPages()
        {
            return View();
        }
        
        public ActionResult ModifyHomePage()
        {
            using (var mo = new Models.Hotel_Amazonian_WillowEntities())
            {
                ViewData["Home"] = mo.Pagina.Where(x => x.nombre == "Home").Include(p => p.Info).Include(p => p.Info.Select(x => x.Imagen1)).ToList();

            }
            return View();
        }

        [HttpPost]
        public ActionResult ModifyHomePage(string description) {

            var mo = new Models.Hotel_Amazonian_WillowEntities();
            var home = mo.Pagina.Where(x => x.nombre == "Home").Include(p => p.Info).Include(p => p.Info.Select(x => x.Imagen1)).First();
            home.Info.First().descripcion = description.Replace("\n", "^");

            mo.SaveChanges();

            TempData["tituloModal"] = "Atención";
            TempData["message"] = "Página Home actualizada.";
            return View("../Admin/Index");
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

    }
}