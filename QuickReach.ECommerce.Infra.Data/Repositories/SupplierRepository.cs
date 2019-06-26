﻿using Microsoft.EntityFrameworkCore;
using QuickReach.ECommerce.Domain;
using QuickReach.ECommerce.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickReach.ECommerce.Infra.Data.Repositories
{
    public class SupplierRepository : RepositoryBase<Supplier>, ISupplierRepository
    {
        public SupplierRepository(ECommerceDbContext context) : base(context)
        {
           

        }

        public IEnumerable<Supplier> Retrieve(string search = "", int skip = 0, int count = 0)
        {
            var result = this.context.Suppliers.Where(c => c.Name.Contains(search)
                || c.Description.Contains(search))
                .Skip(skip)
                .Take(count)
                .ToList();

            return result;
        }
    }

    
}
