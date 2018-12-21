using shanuMVCUserRoles.DTO;
using shanuMVCUserRoles.Models.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace shanuMVCUserRoles.Controllers
{
	public class HomeController : Controller
	{
        private const int limitProduct = 4; 

		public ActionResult Index()
		{
            IEnumerable<ProductViewModel> listProduct;
            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                listProduct = db.Products.ToArray().OrderByDescending(x => x.Id).Select(p => new ProductViewModel(p)).Take(limitProduct).ToList();
            }
            return View(listProduct);
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
    }
}