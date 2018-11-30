using shanuMVCUserRoles.DTO;
using shanuMVCUserRoles.Models;
using shanuMVCUserRoles.Models.Shop;
using PagedList;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
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

                if(db.Products.Select(x => x.CategoryId == dto.Id).Any())
                {
                    TempData["SM"] = "Loại sản phẩm đang được sử dụng cho sản phẩm hiện tại vui lòng kiểm tra trước khi xóa!";
                    return RedirectToAction("Categories");
                }
                else
                {
                    // Remove the category
                    db.Categories.Remove(dto);

                    // Save
                    db.SaveChanges();
                }
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

        // GET: Admin/Shop/AddProduct
        [HttpGet]
        public ActionResult AddProduct()
        {
            // Init model
            ProductViewModel model = new ProductViewModel();

            // Add select list of categories to model
            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            // Return view with model
            return View(model);
        }

        // POST: Admin/Shop/AddProduct
        [HttpPost]
        public ActionResult AddProduct(ProductViewModel model, HttpPostedFileBase file)
        {
            // Check model state
            if (!ModelState.IsValid)
            {
                using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }

            // Make sure product name is unique
            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "Tên sản phẩm đã tồn tại!");
                    return View(model);
                }
            }

            // Declare product id
            int id;

            // Init and save productDTO
            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                Product product = new Product();

                product.Name = model.Name;
                product.Decription = model.Decription;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;

                Category catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                product.CategoryName = catDTO.Name;

                db.Products.Add(product);
                db.SaveChanges();

                // Get the id
                id = product.Id;
            }

            // Set TempData message
            TempData["SM"] = "Thêm sản phẩm thành công!";

            #region Upload Image

            // Create necessary directories
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images", Server.MapPath(@"\")));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);

            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);

            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);

            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);

            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);

            // Check if a file was uploaded
            if (file != null && file.ContentLength > 0)
            {
                // Get file extension
                string ext = file.ContentType.ToLower();

                // Verify extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "Không thể upload hình ảnh, vui lòng kiểm tra lại định dạng!");
                        return View(model);
                    }
                }

                // Init image name
                string imageName = file.FileName;

                // Save image name to DTO
                using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
                {
                    Product dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                // Set original and thumb image paths
                var path = string.Format("{0}\\{1}", pathString2, imageName);
                var path2 = string.Format("{0}\\{1}", pathString3, imageName);

                // Save original
                file.SaveAs(path);

                // Create and save thumb
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }

            #endregion

            // Redirect
            return RedirectToAction("Products");
        }

        // GET: Admin/Shop/Products
        public ActionResult Products(int? page, int? catId)
        {
            // Declare a list of ProductVM
            List<ProductViewModel> listOfProductVM;

            // Set page number
            var pageNumber = page ?? 1;

            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                // Init the list
                listOfProductVM = db.Products.ToArray()
                                  .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                                  .Select(x => new ProductViewModel(x))
                                  .ToList();

                // Populate categories select list
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // Set selected category
                ViewBag.SelectedCat = catId.ToString();
            }

            // Set pagination
            var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 10);
            ViewBag.OnePageOfProducts = onePageOfProducts;

            // Return view with list
            return View(listOfProductVM);
        }

        // GET: Admin/Shop/EditProduct/id
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            // Declare productVM
            ProductViewModel model;

            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                // Get the product
                Product dto = db.Products.Find(id);

                // Make sure product exists
                if (dto == null)
                {
                    return Content("Sản phẩm không tồn tại!");
                }

                // init model
                model = new ProductViewModel(dto);

                // Make a select list
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // Get all gallery images
                model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Products/" + id + "/Gallery/Thumbs"))
                                                .Select(fn => Path.GetFileName(fn));
            }

            // Return view with model
            return View(model);
        }

        // POST: Admin/Shop/EditProduct/id
        [HttpPost]
        public ActionResult EditProduct(ProductViewModel model, HttpPostedFileBase file)
        {
            // Get product id
            int id = model.Id;

            // Populate categories select list and gallery images
            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Products/" + id + "/Gallery/Thumbs"))
                                                .Select(fn => Path.GetFileName(fn));

            // Check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Make sure product name is unique
            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                if (db.Products.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "Tên sản phẩm đã tồn tại!");
                    return View(model);
                }
            }

            // Update product
            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                Product dto = db.Products.Find(id);

                dto.Name = model.Name;
                dto.Decription = model.Decription;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;
                dto.ImageName = model.Image;

                Category catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = catDTO.Name;

                db.SaveChanges();
            }

            // Set TempData message
            TempData["SM"] = "Thay đổi thành công!";

            #region Image Upload

            // Check for file upload
            if (file != null && file.ContentLength > 0)
            {

                // Get extension
                string ext = file.ContentType.ToLower();

                // Verify extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
                    {
                        ModelState.AddModelError("", "Không thể upload hình ảnh, vui lòng kiểm tra lại định dạng!");
                        return View(model);
                    }
                }

                // Set uplpad directory paths
                var originalDirectory = new DirectoryInfo(string.Format("{0}Images", Server.MapPath(@"\")));

                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");

                // Delete files from directories

                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                foreach (FileInfo file2 in di1.GetFiles())
                    file2.Delete();

                foreach (FileInfo file3 in di2.GetFiles())
                    file3.Delete();

                // Save image name

                string imageName = file.FileName;

                using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
                {
                    Product dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                // Save original and thumb images

                var path = string.Format("{0}\\{1}", pathString1, imageName);
                var path2 = string.Format("{0}\\{1}", pathString2, imageName);

                file.SaveAs(path);

                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }

            #endregion

            // Redirect
            return RedirectToAction("Products");
        }

        // GET: Admin/Shop/DeleteProduct/id
        public ActionResult DeleteProduct(int id)
        {
            // Delete product from DB
            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                Product dto = db.Products.Find(id);
                db.Products.Remove(dto);

                db.SaveChanges();
            }

            // Delete product folder
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images", Server.MapPath(@"\")));
            string pathString = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());

            if (Directory.Exists(pathString))
                Directory.Delete(pathString, true);

            // Redirect
            return RedirectToAction("Products");
        }

        // POST: Admin/Shop/SaveGalleryImages
        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            // Loop through files
            foreach (string fileName in Request.Files)
            {
                // Init the file
                HttpPostedFileBase file = Request.Files[fileName];

                // Check it's not null
                if (file != null && file.ContentLength > 0)
                {
                    // Set directory paths
                    var originalDirectory = new DirectoryInfo(string.Format("{0}Images", Server.MapPath(@"\")));

                    string pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
                    string pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

                    // Set image paths
                    var path = string.Format("{0}\\{1}", pathString1, file.FileName);
                    var path2 = string.Format("{0}\\{1}", pathString2, file.FileName);

                    // Save original and thumb

                    file.SaveAs(path);
                    WebImage img = new WebImage(file.InputStream);
                    img.Resize(200, 200);
                    img.Save(path2);
                }

            }

        }

        // POST: Admin/Shop/DeleteImage
        [HttpPost]
        public void DeleteImage(int id, string imageName)
        {
            string fullPath1 = Request.MapPath("~/Images/Products/" + id.ToString() + "/Gallery/" + imageName);
            string fullPath2 = Request.MapPath("~/Images/Products/" + id.ToString() + "/Gallery/Thumbs/" + imageName);

            if (System.IO.File.Exists(fullPath1))
                System.IO.File.Delete(fullPath1);

            if (System.IO.File.Exists(fullPath2))
                System.IO.File.Delete(fullPath2);
        }

    }
}