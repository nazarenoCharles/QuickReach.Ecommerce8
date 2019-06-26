using Microsoft.EntityFrameworkCore;
using QuickReach.ECommerce.Domain;
using QuickReach.ECommerce.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickReach.ECommerce.Infra.Data.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(ECommerceDbContext context) : base(context)
        {

        }

        public IEnumerable<Product> Retrieve(string search = "", int skip = 0, int count = 0)
        {
            var result = this.context.Products.Where(c => c.Name.Contains(search)
            || c.Description.Contains(search))
            .Skip(skip)
            .Take(count)
            .ToList();

            return result;
        }
        public override Product Retrieve(int entityID)
        {
            var entity = this.context.Products.Include(c => c.Category)
                .Where(c => c.ID == entityID)
                .FirstOrDefault();
            return entity;
        }
        public override Product Create(Product newProduct)
        {
            var category = this.context.Categories.Where(c => c.ID == newProduct.CategoryID).FirstOrDefault();
            if(category == null)
            {
                throw new SystemException("You cannot create products if that category is missing!");
            }
            this.context.Set<Product>().Add(newProduct);
            this.context.SaveChanges();
            return newProduct;
        }




    }
}
