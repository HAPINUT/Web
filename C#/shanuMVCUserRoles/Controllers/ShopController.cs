using shanuMVCUserRoles.DTO;
using shanuMVCUserRoles.Models.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace shanuMVCUserRoles.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            IEnumerable<ProductViewModel> listProduct;
            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                listProduct = db.Products.ToArray().Select(p => new ProductViewModel(p)).ToList();

                ViewBag.CategoryName = string.Empty;
                ViewBag.IsFeaturedProductsDisplay = false;
            }
            return View(listProduct);
        }

        public ActionResult Categories(int id)
        {
            IEnumerable<ProductViewModel> listProduct;
            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                listProduct = db.Products.Where(x => x.CategoryId == id).ToArray().Select(p => new ProductViewModel(p)).ToList();

                ViewBag.listProductOther = db.Products.Where(x => x.CategoryId != id).ToArray().Select(p => new ProductViewModel(p)).ToList(); ;
                ViewBag.CategoryName = listProduct.FirstOrDefault().CategoryName;
                ViewBag.IsFeaturedProductsDisplay = true;
            }

            return View("Index", listProduct);
        }

        public ActionResult ProductDetails(int id)
        {
            Product dto;
            ProductViewModel productVM;

            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                if(!db.Products.Any(x=>x.Id.Equals(id)))
                {
                    return RedirectToAction("Index", "Shop");
                }

                // Init product
                dto = db.Products.Where(x => x.Id == id).FirstOrDefault();

                productVM = new ProductViewModel(dto);
            }

            // Get gallery images
            productVM.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Products/" + id + "/Gallery"))
                                                .Select(fn => Path.GetFileName(fn));

            #region Get danh sách sản phẩm liên quan
            IEnumerable<ProductViewModel> listProduct;
            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                listProduct = db.Products.Where(x => x.CategoryId == productVM.CategoryId && x.Id != productVM.Id).ToArray().Select(p => new ProductViewModel(p)).ToList();
            }
            ViewBag.ListProduct = listProduct;
            #endregion

            // Return view with model
            return View("ProductDetails", productVM);
        }

        // GET: Search
        public ActionResult Search(string keyword)
        {
            IEnumerable<ProductViewModel> listProduct;
            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                listProduct = db.Products.Where(x => x.Name.ToLower().Contains(keyword.ToLower())).ToArray().Select(p => new ProductViewModel(p)).ToList();
                ViewBag.IsFeaturedProductsDisplay = false;

                if(listProduct.Count() == 0)
                {
                    ViewBag.NoRecord = string.Format("Không có sản phẩm nào phù hợp với kết quả tìm kiếm '{0}'", keyword);
                }
            }
            return View("Index", listProduct);
        }

        // POST: Page
        [HttpPost]
        public JsonResult GetProduct(Product p)
        {
            var product = new Product();
            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                product = db.Products.Where(x => x.Id == p.Id).SingleOrDefault();
            }

            return Json(product.Decription, JsonRequestBehavior.AllowGet);
        }
    }
}