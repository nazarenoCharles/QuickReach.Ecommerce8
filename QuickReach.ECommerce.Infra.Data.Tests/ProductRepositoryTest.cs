using QuickReach.ECommerce.Domain.Models;
using QuickReach.ECommerce.Infra.Data.Repositories;
using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace QuickReach.ECommerce.Infra.Data.Tests
{
    public class ProductRepositoryTest
    {
        [Fact]
        public void CreateNewProduct_WithValidEntities_ShouldAddIntoDatabase()
        {
            var context = new ECommerceDbContext();
            var sut = new ProductRepository(context);
            var product = new Product{
                Name = "Superstar",
                Description = "Adidas Superstar Edition",
                Price = 100,
                CategoryID = 1,
                ImageURL ="Superstar.jpeg"
            };
            sut.Create(product);
            //Act
            Assert.True(product.ID != 0);
            //Assert
            var prodEntity = sut.Retrieve(product.ID);
            Assert.NotNull(prodEntity);

            //Clean
            sut.Delete(product.ID);
        }

    }
}
