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
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        public int PageSize = 4;    // We will change this later

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public ViewResult List(Cart cart, string category, int page = 1)
        {
            var products = _productRepository.Products
                                             .Where(p => category == null || p.Category == category)
                                             .OrderBy(p => p.ProductID)
                                             .Skip((page - 1)*PageSize)
                                             .Take(PageSize);

            string carttotal = cart.ComputeTotalValue().ToString("c");
            int quantity = cart.Lines.Sum(x => x.Quantity);

            ProductListViewModel viewModel = new ProductListViewModel
                                                 {
                                                     Products = products,
                                                     PagingInfo = new PagingInfo
                                                                      {
                                                                          CurrentPage = page,
                                                                          ItemsPerPage = PageSize,
                                                                          TotalItems = category == null 
                                                                            ? _productRepository.Products.Count()
                                                                            : _productRepository.Products.Count(x => x.Category == category)
                                                                      },
                                                     CurrentCategory = category,
                                                     CartTotal = carttotal,
                                                     CartQuantity = quantity
                                                 };
            

            return View(viewModel);
        }
    }
}
