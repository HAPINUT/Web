using Microsoft.AspNet.Identity.Owin;
using shanuMVCUserRoles.DTO;
using shanuMVCUserRoles.Models.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace shanuMVCUserRoles.Controllers
{
    public class CartController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public CartController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: Cart
        public ActionResult Index()
        {
            // Init the cart list
            var cart = Session["cart"] as List<CartViewModel> ?? new List<CartViewModel>();

            // Check if cart is empty
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your cart is empty.";
                return View();
            }

            // Calculate total and save to ViewBag

            decimal total = 0m;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            // Return view with list
            return View(cart);
        }

        public ActionResult CartPartial()
        {
            // Init CartViewModel
            CartViewModel model = new CartViewModel();

            // Init quantity
            int qty = 0;

            // Init price
            decimal price = 0m;

            // Check for cart session
            if (Session["cart"] != null)
            {
                // Get total qty and price
                var list = (List<CartViewModel>)Session["cart"];

                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }

                model.Quantity = qty;
                model.Price = price;

            }
            else
            {
                // Or set qty and price to 0
                model.Quantity = 0;
                model.Price = 0m;
            }

            // Return partial view with model
            return PartialView(model);
        }

        public ActionResult AddToCartPartial(int id)
        {
            // Init CartViewModel list
            List<CartViewModel> cart = Session["cart"] as List<CartViewModel> ?? new List<CartViewModel>();

            // Init CartViewModel
            CartViewModel model = new CartViewModel();

            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                // Get the product
                Product product = db.Products.Find(id);

                // Check if the product is already in cart
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                // If not, add new
                if (productInCart == null)
                {
                    cart.Add(new CartViewModel()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }
                else
                {
                    // If it is, increment
                    productInCart.Quantity++;
                }
            }

            // Get total qty and price and add to model

            int qty = 0;
            decimal price = 0m;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            // Save cart back to session
            Session["cart"] = cart;

            // Return partial view with model
            return PartialView(model);
        }

        // GET: /Cart/IncrementProduct
        public JsonResult IncrementProduct(int productId)
        {
            // Init cart list
            List<CartViewModel> cart = Session["cart"] as List<CartViewModel>;

            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                // Get CartViewModel from list
                CartViewModel model = cart.FirstOrDefault(x => x.ProductId == productId);

                // Increment qty
                model.Quantity++;

                // Store needed data
                var result = new { qty = model.Quantity, price = model.Price };

                // Return json with data
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        // GET: /Cart/DecrementProduct
        public ActionResult DecrementProduct(int productId)
        {
            // Init cart
            List<CartViewModel> cart = Session["cart"] as List<CartViewModel>;

            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                // Get model from list
                CartViewModel model = cart.FirstOrDefault(x => x.ProductId == productId);

                // Decrement qty
                if (model.Quantity > 1)
                {
                    model.Quantity--;
                }
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }

                // Store needed data
                var result = new { qty = model.Quantity, price = model.Price };

                // Return json
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        // GET: /Cart/RemoveProduct
        public void RemoveProduct(int productId)
        {
            // Init cart list
            List<CartViewModel> cart = Session["cart"] as List<CartViewModel>;

            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                // Get model from list
                CartViewModel model = cart.FirstOrDefault(x => x.ProductId == productId);

                // Remove model from list
                cart.Remove(model);
            }

        }

        public ActionResult PaypalPartial()
        {
            List<CartViewModel> cart = Session["cart"] as List<CartViewModel>;

            return PartialView(cart);
        }

        // POST: /Cart/PlaceOrder
        [HttpPost]
        public void PlaceOrder()
        {
            // Get cart list
            List<CartViewModel> cart = Session["cart"] as List<CartViewModel>;

            // Get username
            string username = User.Identity.Name;

            int orderId = 0;

            using (HAPINUTSHOPEntities db = new HAPINUTSHOPEntities())
            {
                // Init OrderDTO
                Order orderDTO = new Order();

                // Get user id
                var q = db.AspNetUsers.FirstOrDefault(x => x.UserName == username);
                string userId = q.Id;

                // Add to OrderDTO and save
                orderDTO.UserId = userId;
                orderDTO.OrderDate = DateTime.Now;

                db.Orders.Add(orderDTO);

                db.SaveChanges();

                // Get inserted id
                orderId = orderDTO.Id;

                // Init OrderDetailsDTO
                OrderDetail orderDetailsDTO = new OrderDetail();

                // Add to OrderDetailsDTO
                foreach (var item in cart)
                {
                    orderDetailsDTO.OrderId = orderId;
                    orderDetailsDTO.UserId = userId;
                    orderDetailsDTO.ProductId = item.ProductId;
                    orderDetailsDTO.Quantity = item.Quantity;

                    db.OrderDetails.Add(orderDetailsDTO);

                    db.SaveChanges();
                }
            }

            // Email admin
            //var client = new SmtpClient("mailtrap.io", 2525)
            //{
            //    Credentials = new NetworkCredential("21f57cbb94cf88", "e9d7055c69f02d"),
            //    EnableSsl = true
            //};
            //client.Send("admin@example.com", "admin@example.com", "New Order", "You have a new order. Order number " + orderId);

            // Reset session
            Session["cart"] = null;
        }
    }
}