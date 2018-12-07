using shanuMVCUserRoles.DTO;
using shanuMVCUserRoles.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace shanuMVCUserRoles.Controllers
{
    public class NavigationController : Controller
    {
        // GET: Navigation
        public ActionResult Menu()
        {
            IEnumerable<CategoryViewModel> listCategory;
            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                listCategory = db.Categories.ToArray().Select(p => new CategoryViewModel(p)).ToList();
            }
            return PartialView("_CategoriesPartial", listCategory);
        }
    }
}