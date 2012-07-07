using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminControllerTests
    {
        [TestMethod]
        public void Index_Contains_All_Products()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                                                    {
                                                        new Product {ProductID = 1, Name = "P1"},
                                                        new Product {ProductID = 2, Name = "P2"},
                                                        new Product {ProductID = 3, Name = "P3"}
                                                    }.AsQueryable());

            AdminController target = new AdminController(mock.Object);

            // Act
            List<Product> result = ((IEnumerable<Product>)target.Index().Model).ToList();

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("P2", result[1].Name);
        }

        [TestMethod]
        public void AjaxDelete_PassesProductIdToRepositoryRemoveMethod_DeletesProduct()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            AdminController target = new AdminController(mock.Object);
            
            // Act
            var result = target.AjaxDelete(new Product{ProductID = 3});

            // Assert
            mock.Verify(m => m.RemoveProduct(3), Times.Once());
        }
    }
}
