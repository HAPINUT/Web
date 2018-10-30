﻿using shanuMVCUserRoles.DTO;
using shanuMVCUserRoles.Models.Shop;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            }
            return View(listProduct);
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

            // Return view with model
            return View("ProductDetails", productVM);
        }
    }
}