﻿using Microsoft.EntityFrameworkCore;
using QuickReach.ECommerce.Domain;
using QuickReach.ECommerce.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickReach.ECommerce.Infra.Data.Repositories
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(ECommerceDbContext context) : base(context)
        {

        }

        public IEnumerable<Category> Retrieve(string search = "", int skip = 0, int count = 0)
        {
            var result = this.context.Categories.Where(c => c.Name.Contains(search) ||
            c.Description.Contains(search))
            .Skip(skip)
            .Take(count)
            .ToList();

            return result;
        }

        public override Category Retrieve(int entityID)
        {
            var entity = this.context.Categories.Include(c => c.Products)
                .AsNoTracking()
                .Where(c => c.ID == entityID)
                .FirstOrDefault();
            return entity;
        }
        public override void Delete(int entityID)
        {
            

            var entityToRemove = Retrieve(entityID);
            this.context.Remove<Category>(entityToRemove);
            this.context.SaveChanges();
        }
    }
}
