using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Controllers
{
    public class AdminController : Controller
    {
        private readonly IProductRepository _productRepository;

        public AdminController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public ViewResult Index()
        {
            return View(_productRepository.Products);
        }

        public ActionResult AjaxDelete(Product product)
        {
            bool result = _productRepository.RemoveProduct(product.ProductID);

            return Json(new {success = result});
        }

        [HttpPost]
        public ActionResult AjaxEdit(Product product)
        {
            if(ModelState.IsValid)
            {
                _productRepository.InsertOrUpdate(product);

                TempData["message"] = string.Format("{0} has been saved", product.Name);

                return Json(new {success = true, _productRepository.Products}); // return all products -- ko.mapping.fromJS(data)
            }

            return Json(new {success = false});
        }

        [HttpPost]
        public ActionResult AjaxCreate(Product product)
        {
            if (ModelState.IsValid)
            {
                _productRepository.InsertOrUpdate(product);
                
                return Json(new { success = true, product });   // return product -- self.products.push(product)
            }

            return Json(new { success = false });
        }
    }
}
