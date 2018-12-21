using shanuMVCUserRoles.DTO;
using shanuMVCUserRoles.Models.Pages;
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
            //using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            //{
            //    var page = db.Pages.FirstOrDefault(x => x.TopicId == 2);
            //    PageViewModel model = new PageViewModel(page);
            //    return View(model);
            //}
            return View();
        }

        [HttpPost]
        public JsonResult GetPage(Page p)
        {
            var page = new Page();
            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                page = db.Pages.Where(x => x.Id == p.Id).SingleOrDefault();
            }

            if (page == null || page.Body == null)
            {
                return Json("Không có dữ liệu", JsonRequestBehavior.AllowGet);
            }

            return Json(page.Body, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
    }
}