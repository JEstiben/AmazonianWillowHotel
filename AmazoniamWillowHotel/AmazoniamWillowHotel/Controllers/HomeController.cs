using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AmazoniamWillowHotel.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            using (var mo = new Models.Hotel_Amazonian_WillowEntities())
            {
                ViewData["Home"] = mo.Pagina.Where(x => x.nombre == "Home").Include(p => p.Info).Include(p => p.Info.Select(x => x.Imagen1)).ToList();
                
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Informate más de las atracciones que existen cerca del hotel";

            using (var mo = new Models.Hotel_Amazonian_WillowEntities())
            {
                ViewData["SobreNosotros"] = mo.Pagina.Where(x => x.nombre == "Sobre Nosotros").Include(p => p.Info).Include(p => p.Info.Select(x => x.Imagen1)).ToList();
            }

            return View();
        }

        public ActionResult Contact()
        {
            using (var mo = new Models.Hotel_Amazonian_WillowEntities())
            {
                ViewData["Contactenos"] = mo.Pagina.Where(x => x.nombre == "Contáctenos").Include(p => p.Info).Include(p => p.Info.Select(x => x.Imagen1)).ToList();
            }

            return View();
        }

      

        public ActionResult HowToGet()
        {
            using (var mo = new Models.Hotel_Amazonian_WillowEntities())
            {
                ViewData["¿Como Llegar?"] = mo.Pagina.Where(x => x.nombre == "¿Como Llegar?").Include(p => p.Info).ToList();
            }
            return View();
        }

        public ActionResult Rates()
        {
            using (var mo = new Models.Hotel_Amazonian_WillowEntities())
            {
                ViewData["Tarifas"] = mo.Tipo_Habitacion.Include(p => p.Imagen1).Include(p => p.Caracteristica).ToList();
            }
            return View();
        }
    }
}