using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.HtmlHelpers;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void Can_Paginate()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                                                    {
                                                        new Product {ProductID = 1, Name = "P1"},
                                                        new Product {ProductID = 2, Name = "P2"},
                                                        new Product {ProductID = 3, Name = "P3"},
                                                        new Product {ProductID = 4, Name = "P4"},
                                                        new Product {ProductID = 5, Name = "P5"},
                                                    }.AsQueryable());

            ProductController target = new ProductController(mock.Object) {PageSize = 3};

            // Act
            var products = (ProductListViewModel)target.List(new Cart(), null, 2).Model;
            var result = products.Products.ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("P4", result[0].Name);
            Assert.AreEqual("P5", result[1].Name);
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            // Arrange - define an HTML helper - we need to do this in order to apply the extension method
            HtmlHelper myHelper = null;

            PagingInfo pagingInfo = new PagingInfo
                                        {
                                            CurrentPage = 2,
                                            TotalItems = 28,
                                            ItemsPerPage = 10
                                        };
            Func<int, string> pageUrlDelegate = i => "Page" + i;

            // Act
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            // Assert
            Assert.AreEqual(result.ToString(), @"<a href=""Page1"">1</a><a class=""selected"" href=""Page2"">2</a><a href=""Page3"">3</a>");
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                                                    {
                                                        new Product {ProductID = 1, Name = "P1"},
                                                        new Product {ProductID = 2, Name = "P2"},
                                                        new Product {ProductID = 3, Name = "P3"},
                                                        new Product {ProductID = 4, Name = "P4"},
                                                        new Product {ProductID = 5, Name = "P5"},
                                                    }.AsQueryable());
            ProductController target = new ProductController(mock.Object) {PageSize = 3};

            // Act
            ProductListViewModel result = (ProductListViewModel)target.List(new Cart(), null, 2).Model;

            // Assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(2, pageInfo.CurrentPage);
            Assert.AreEqual(3, pageInfo.ItemsPerPage);
            Assert.AreEqual(5, pageInfo.TotalItems);
            Assert.AreEqual(2, pageInfo.TotalPages);
        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                                                    {
                                                        new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                                                        new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                                                        new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                                                        new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                                                        new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
                                                    }.AsQueryable());

            ProductController target = new ProductController(mock.Object) {PageSize = 3};

            // Act
            var result = ((ProductListViewModel) target.List(new Cart(), "Cat2", 1).Model).Products.ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                                                    {
                                                        new Product {ProductID = 1, Name = "P1", Category = "Apples"},
                                                        new Product {ProductID = 2, Name = "P2", Category = "Apples"},
                                                        new Product {ProductID = 3, Name = "P3", Category = "Plums"},
                                                        new Product {ProductID = 4, Name = "P4", Category = "Oranges"}
                                                    }.AsQueryable());
            NavController target = new NavController(mock.Object);

            // Act
            var results = ((IEnumerable<string>)target.Menu().Model).ToList();

            // Assert
            Assert.AreEqual(3, results.Count);
            Assert.AreEqual("Apples", results[0]);
            Assert.AreEqual("Oranges", results[1]);
            Assert.AreEqual("Plums", results[2]);
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                                                    {
                                                        new Product {ProductID = 1, Name = "P1", Category = "Apples"},
                                                        new Product {ProductID = 4, Name = "P2", Category = "Oranges"}
                                                    }.AsQueryable());

            NavController target = new NavController(mock.Object);

            string categoryToSelect = "Apples";

            // Act
            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            // Assert
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                                                    {
                                                        new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                                                        new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                                                        new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                                                        new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                                                        new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
                                                    }.AsQueryable());

            ProductController target = new ProductController(mock.Object) {PageSize = 3};

            // Act
            int res1 = TotalItemsCount(target, "Cat1");
            int res2 = TotalItemsCount(target, "Cat2");
            int res3 = TotalItemsCount(target, "Cat3");
            int resAll = TotalItemsCount(target, null);

            // Assert
            Assert.AreEqual(2, res1);
            Assert.AreEqual(2, res2);
            Assert.AreEqual(1, res3);
            Assert.AreEqual(5, resAll);
        }

        [TestMethod]
        public void Cannot_Checkout_Empty_Cart()
        {
            // Arrange
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            ShippingDetails shippingDetails = new ShippingDetails();
            CartController target = new CartController(null, mock.Object);

            // Act
            ViewResult result = target.Checkout(cart, shippingDetails);

            // Assert
            // check that the order hasn't been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
            Assert.AreEqual(string.Empty, result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            // Arrange
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            CartController target = new CartController(null, mock.Object);
            target.ModelState.AddModelError("error", "error");

            // Act
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            // Assert
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
            Assert.AreEqual(string.Empty, result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        private int TotalItemsCount(ProductController target, string selectedCategory)
        {
            return ((ProductListViewModel)target.List(new Cart(), selectedCategory).Model).PagingInfo.TotalItems;
        }
    }
}
