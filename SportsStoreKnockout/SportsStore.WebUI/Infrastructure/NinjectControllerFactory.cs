using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Moq;
using Ninject;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Concrete;
using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private readonly IKernel _ninjectKernel;

        public NinjectControllerFactory()
        {
            _ninjectKernel = new StandardKernel();
            AddBindings();
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            return controllerType == null
                       ? null
                       : (IController) _ninjectKernel.Get(controllerType);
        }

        private void AddBindings()
        {
            _ninjectKernel.Bind<IProductRepository>().To<ProductRepository>();

            EmailSettings emailSettings = new EmailSettings
                                              {
                                                  WriteAsFile =
                                                  bool.Parse(ConfigurationManager.AppSettings["Email.WriteAsFile"] ?? "false")
                                              };
            _ninjectKernel.Bind<IOrderProcessor>().To<EmailOrderProcessor>().WithConstructorArgument("settings",
                                                                                                     emailSettings);
        }

        private Mock<IProductRepository> DummyProductRepository()
        {
            // Mock implementation of the IProductRepository Interface
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product>
                                                    {
                                                        new Product {Name = "Football", Price = 25},
                                                        new Product {Name = "Surf board", Price = 179},
                                                        new Product {Name = "Running shoes", Price = 95, Description = "Light and comfortable shoes"}
                                                    }.AsQueryable());

            return mock;
        }
    }
}