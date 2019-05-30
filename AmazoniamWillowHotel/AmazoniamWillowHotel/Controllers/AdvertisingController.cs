using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AmazoniamWillowHotel.Controllers
{
    public class AdvertisingController : Controller
    {

        public PartialViewResult _Advertising()
        {
            using (var mo = new Models.Hotel_Amazonian_WillowEntities())
            {
                ViewData["Advertisng"] = mo.Publicidad.Include(p => p.Imagen1).ToList();

            }
            return PartialView();
        }

    }
}