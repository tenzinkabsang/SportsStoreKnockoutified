using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Concrete
{
    public class ProductRepository :IProductRepository
    {
        private readonly EFDbContext _context = new EFDbContext();

        public IQueryable<Product> Products
        {
            get { return _context.Products; }
        }


        public bool RemoveProduct(int productId)
        {
            Product product = _context.Products.Find(productId);

            if (product == null)
                return false;

            _context.Products.Remove(product);
            _context.SaveChanges();

            return true;
        }

        public void InsertOrUpdate(Product product)
        {
            if (product.ProductID == default(int))
                _context.Products.Add(product);
            else
                _context.Entry(product).State = EntityState.Modified;

            _context.SaveChanges();
        }
    }
}
