using PagedList;
using shanuMVCUserRoles.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace shanuMVCUserRoles.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        // GET: Admin/User
        public ActionResult Index(int? page)
        {
            try
            {
                var pageNumber = page ?? 1;
                using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
                {
                    var listUser = db.AspNetUsers.ToList();

                    // Set pagination
                    var onePageOfProducts = listUser.ToPagedList(pageNumber, 10);
                    ViewBag.OnePageOfProducts = onePageOfProducts;

                    return View(listUser);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}