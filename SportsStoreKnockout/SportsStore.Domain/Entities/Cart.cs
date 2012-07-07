using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SportsStore.Domain.Entities
{
    public class Cart
    {
        private readonly List<CartLine> _lineCollection = new List<CartLine>(); 

        public void AddItem(Product product, int quantity)
        {
            CartLine line = _lineCollection.FirstOrDefault(x => x.Product.ProductID == product.ProductID);

            if (line == null)
                _lineCollection.Add(new CartLine {Product = product, Quantity = quantity});
            else
                line.Quantity += quantity;
        }

        public void RemoveLine(Product product)
        {
            _lineCollection.RemoveAll(l => l.Product.ProductID == product.ProductID);
        }

        public decimal ComputeTotalValue()
        {
            return _lineCollection.Sum(x => x.Product.Price*x.Quantity);
        }

        public void Clear()
        {
            _lineCollection.Clear();
        }

        public IEnumerable<CartLine>  Lines 
        {
            get { return _lineCollection; }
        }

        public void RemoveItem(Product product)
        {
            var lineItem = _lineCollection.FirstOrDefault(p => p.Product.ProductID == product.ProductID);

            if(lineItem == null)
                return;

            if (lineItem.Quantity > 1)
                lineItem.Quantity--;
            else
                _lineCollection.Remove(lineItem);
        }
    }

    public class CartLine
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public decimal LineTotal 
        {
            get { return Product.Price*Quantity; }
        }
    }
}
