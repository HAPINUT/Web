using shanuMVCUserRoles.DTO;
using shanuMVCUserRoles.Models.Pages;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace shanuMVCUserRoles.Controllers
{
    public class PageController : Controller
    {
        // GET: Page
        public ActionResult Index()
        {
            IEnumerable<PageViewModel> listPage;
            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                listPage = db.Pages.ToArray().Select(p => new PageViewModel(p)).ToList();
            }
            return View(listPage);
        }

        // POST: Page
        [HttpPost]
        public JsonResult GetPage(Page p)
        {
            var page = new Page();
            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                page = db.Pages.Where(x => x.Id == p.Id).SingleOrDefault();
            }

            return Json(page.Body, JsonRequestBehavior.AllowGet);
        }
    }
}