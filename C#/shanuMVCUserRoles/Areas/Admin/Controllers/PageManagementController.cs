using shanuMVCUserRoles.DTO;
using shanuMVCUserRoles.Models.Pages;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;

namespace shanuMVCUserRoles.Areas.Admin.Controllers
{
    //[Authorize]
    public class PageManagementController : Controller
    {
        // GET: Admin/Page
        public ActionResult Index(int? page)
        {
            // Declare list of PageVM
            List<PageViewModel> pagesList;

            // Set page number
            var pageNumber = page ?? 1;

            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                // Init the list
                pagesList = db.Pages.ToArray().OrderBy(x => x.Title).Select(x => new PageViewModel(x)).ToList();
            }

            // Set pagination
            var onePageOfPages = pagesList.ToPagedList(pageNumber, 10);
            ViewBag.OnePageOfPages = onePageOfPages;

            // Return view with list
            return View(pagesList);
        }

        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        { 
            PageViewModel model = new PageViewModel();
            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                model.topics = new SelectList(db.Topics.ToList(), "Id", "Name");
            }
            return View(model);
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

        [HttpGet]
        public ActionResult EditPage (int id)
        {
            PageViewModel model;
            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                Page dto = db.Pages.Find(id);

                if (dto == null)
                {
                    return Content("Bài viết không tồn tại!");
                }

                model = new PageViewModel(dto);
            }
            return View(model);
        }

        // POST: Admin/Pages/EditPage/id
        [HttpPost]
        public ActionResult EditPage(PageViewModel model)
        {
            // Check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                // Get page id
                int id = model.Id;

                // Get the page
                Page dto = db.Pages.Find(id);

                // DTO the title
                dto.Title = model.Title;

                // Make sure title are unique
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "Tiêu đề đã tồn tại!");
                    return View(model);
                }

                // DTO the rest
                dto.Body = model.Body;

                // Save the DTO
                db.SaveChanges();
            }

            // Set TempData message
            TempData["SM"] = "Chỉnh sửa thành công!";

            // Redirect
            return RedirectToAction("Index");
        }

        // GET: Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            // Declare PageVM
            PageViewModel model;

            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                // Get the page
                Page dto = db.Pages.Find(id);

                // Confirm page exists
                if (dto == null)
                {
                    return Content("The page does not exist.");
                }

                // Init PageVM
                model = new PageViewModel(dto);
            }

            // Return view with model
            return View(model);
        }

        // GET: Admin/Pages/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                // Get the page
                Page dto = db.Pages.Find(id);

                // Remove the page
                db.Pages.Remove(dto);

                // Save
                db.SaveChanges();
            }

            // Redirect
            return RedirectToAction("Index");
        }
    }
}