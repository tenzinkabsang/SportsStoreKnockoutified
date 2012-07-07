using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderProcessor _orderProcessor;

        public CartController(IProductRepository productRepository, IOrderProcessor orderProcessor)
        {
            _productRepository = productRepository;
            _orderProcessor = orderProcessor;
        }

        [HttpPost]
        public ViewResult Checkout(Cart cart, ShippingDetails shippingDetails)
        {
            if (!cart.Lines.Any())
                ModelState.AddModelError("", "Sorry, your cart is empty!");

            if (ModelState.IsValid)
            {
                _orderProcessor.ProcessOrder(cart, shippingDetails);
                cart.Clear();
                return View("Completed");
            }

            return View(shippingDetails);
        }

        public RedirectToRouteResult AddToCart(Cart cart, int productId, string returnUrl)
        {
            Product product = _productRepository.Products.FirstOrDefault(p => p.ProductID == productId);

            if (product != null)
                cart.AddItem(product, 1);

            return RedirectToAction("Index", new { returnUrl });
        }

        public ActionResult AjaxAdd(Cart cart, int productId)
        {
            Product product = _productRepository.Products.FirstOrDefault(p => p.ProductID == productId);

            if (product != null)
                cart.AddItem(product, 1);

            int cartQuantity = cart.Lines.Sum(x => x.Quantity);
            string cartTotal = cart.ComputeTotalValue().ToString("c");

            return Json(new { success = true, cartQuantity, cartTotal });
        }

        public RedirectToRouteResult RemoveFromCart(Cart cart, int productId, string returnUrl)
        {
            Product product = _productRepository.Products.FirstOrDefault(p => p.ProductID == productId);

            if (product != null)
                cart.RemoveLine(product);

            return RedirectToAction("Index", new { returnUrl });
        }

        public ActionResult AjaxRemove(Cart cart, int productId)
        {
            Product product = _productRepository.Products.FirstOrDefault(p => p.ProductID == productId);

            if (product != null)
                cart.RemoveLine(product);

            return Json(new { success = true, cart });
        }

        public ActionResult RemoveItem(Cart cart, Product product)
        {
            //Product product = _productRepository.Products.FirstOrDefault(p => p.ProductID == productId);

            if (product != null)
                cart.RemoveLine(product);

            return Json(new { success = true, total = cart.ComputeTotalValue() });
        }

        [HttpPost]
        public ActionResult UpdateQuantity(Cart cart, CartLine line)
        {
            CartLine lineItem = cart.Lines.First(p => p.Product.ProductID == line.Product.ProductID);

            lineItem.Quantity = line.Quantity;

            CartIndexViewModel model = new CartIndexViewModel
                                           {
                                               Cart = cart,
                                               ReturnUrl = "",
                                               Total = cart.ComputeTotalValue()
                                           };

            //return Json(new { success = true, lineTotal = lineItem.LineTotal, total = cart.ComputeTotalValue() });

            return Json(new {success = true, model});
        }

        public ViewResult Index(Cart cart, string returnUrl)
        {
            return View(new CartIndexViewModel
            {
                Cart = cart,
                ReturnUrl = returnUrl,
                Total = cart.ComputeTotalValue()
            });
        }

        public ViewResult Checkout()
        {
            return View(new ShippingDetails());
        }

        public PartialViewResult Summary(Cart cart)
        {
            return PartialView(cart);
        }

    }
}
