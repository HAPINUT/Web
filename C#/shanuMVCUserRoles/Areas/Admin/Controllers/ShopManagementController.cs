using shanuMVCUserRoles.DTO;
using shanuMVCUserRoles.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace shanuMVCUserRoles.Areas.Admin.Controllers
{
    public class ShopManagementController : Controller
    {
        // GET: Admin/Shop/Categories
        public ActionResult Categories()
        {
            // Declare a list of models
            List<CategoryViewModel> categoryVMList;

            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                // Init the list
                categoryVMList = db.Categories
                                .ToArray()
                                .OrderBy(x => x.Name)
                                .Select(x => new CategoryViewModel(x))
                                .ToList();
            }

            // Return view with list
            return View(categoryVMList);
        }

        // POST: Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            // Declare id
            string id;

            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                // Check that the category name is unique
                if (db.Categories.Any(x => x.Name == catName))
                    return "titletaken";

                // Init DTO
                Category dto = new Category();

                // Add to DTO
                dto.Name = catName;

                // Save DTO
                db.Categories.Add(dto);
                db.SaveChanges();

                // Get the id
                id = dto.Id.ToString();
            }

            // Return id
            return id;
        }

        // POST: Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                // Set initial count
                int count = 1;

                // Declare CategoryDTO
                Category dto;

                // Set sorting for each category
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);

                    db.SaveChanges();

                    count++;
                }
            }

        }

        // GET: Admin/Shop/DeleteCategory/id
        public ActionResult DeleteCategory(int id)
        {
            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                // Get the category
                Category dto = db.Categories.Find(id);

                // Remove the category
                db.Categories.Remove(dto);

                // Save
                db.SaveChanges();
            }

            // Redirect
            return RedirectToAction("Categories");
        }

        // POST: Admin/Shop/RenameCategory
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                // Check category name is unique
                if (db.Categories.Any(x => x.Name == newCatName))
                    return "titletaken";

                // Get DTO
                Category dto = db.Categories.Find(id);

                // Edit DTO
                dto.Name = newCatName;

                // Save
                db.SaveChanges();
            }

            // Return
            return "ok";
        }
    }
}