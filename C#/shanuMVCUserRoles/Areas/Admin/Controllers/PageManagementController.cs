using shanuMVCUserRoles.DTO;
using shanuMVCUserRoles.Models.Pages;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace shanuMVCUserRoles.Areas.Admin.Controllers
{
    [Authorize]
    public class PageManagementController : Controller
    {
        // GET: Admin/Page
        public ActionResult Index()
        {
            // Declare list of PageVM
            List<PageViewModel> pagesList;

            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                // Init the list
                pagesList = db.Pages.ToArray().OrderBy(x => x.Title).Select(x => new PageViewModel(x)).ToList();
            }

            // Return view with list
            return View(pagesList);
        }

        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageViewModel model)
        {
            // Check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                // Declare slug
                //string slug;

                // Init pageDTO
                Page dto = new Page();

                // DTO title
                dto.Title = model.Title;

                // Make sure title and slug are unique
                if (db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title or slug already exists.");
                    return View(model);
                }

                // DTO the rest
                dto.Body = model.Body;
                dto.TopicId = model.TopicId;

                // Save DTO
                db.Pages.Add(dto);
                db.SaveChanges();
            }

            // Set TempData message
            TempData["SM"] = "You have added a new page!";

            // Redirect
            return RedirectToAction("Index");
        }
    }
}