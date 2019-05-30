using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AmazoniamWillowHotel.Controllers
{
    public class PromotionController : Controller
    {

        public PartialViewResult _Promotions()
        {
            using (var mo = new Models.Hotel_Amazonian_WillowEntities())
            {
                DateTime fechaActual = DateTime.Today;
                ViewData["Promotions"] = mo.Promocion.Where(p => p.inicio <= fechaActual && p.fin >= fechaActual).Include(p => p.Tipo_Habitacion).Include(p => p.Tipo_Habitacion.Imagen1).ToList();
            }
            return PartialView();
        }

        public ActionResult ShowPromotion(int id)
        {
            using (var mo = new Models.Hotel_Amazonian_WillowEntities())
            {
                ViewData["Promotion"] = mo.Promocion.Where(p => p.id == id).Include(p => p.Tipo_Habitacion).Include(p => p.Tipo_Habitacion.Imagen1).ToList();
            }
            return View();
        }


        public ActionResult insertPromotionView()
        {
           
            return View();
        }


        public JsonResult getTypes()
        {
            var mo = new Models.Hotel_Amazonian_WillowEntities();

            return Json(mo.sp_getTypes(), JsonRequestBehavior.AllowGet);
        }

    }
}