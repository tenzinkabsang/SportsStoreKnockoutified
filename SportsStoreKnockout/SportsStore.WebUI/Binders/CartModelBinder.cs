using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Binders
{
    public class CartModelBinder :IModelBinder
    {
        private const string _sessionKey = "Cart";

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // get the Cart from the session
            Cart cart = (Cart) controllerContext.HttpContext.Session[_sessionKey];

            if(cart == null)
            {
                cart = new Cart();
                controllerContext.HttpContext.Session[_sessionKey] = cart;
            }

            return cart;
        }
    }
}