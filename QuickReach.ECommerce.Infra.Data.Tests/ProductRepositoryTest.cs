using QuickReach.ECommerce.Domain.Models;
using QuickReach.ECommerce.Infra.Data.Repositories;
using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace QuickReach.ECommerce.Infra.Data.Tests
{
    public class ProductRepositoryTest
    {
        #region CreateNewProduct_WithValidEntities_ShouldAddIntoDatabase
        [Fact]
        public void CreateNewProduct_WithValidEntities_ShouldAddIntoDatabase()
        {
            var connectionBuilder = new SqliteConnectionStringBuilder()
            {
                DataSource = ":memory:"
            };
            var connection = new SqliteConnection(connectionBuilder.ConnectionString);
            var options = new DbContextOptionsBuilder<ECommerceDbContext>().UseSqlite(connection).Options;

            var category = new Category
            {
                Name = "Shoes",
                Description = "Shoe Department",
                IsActive = true
            };
            using (var context = new ECommerceDbContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();
                context.Categories.Add(category);
                context.SaveChanges();
            }
            var product = new Product
            {
                Name = "Superstar Adidas",
                Description = "Adidas Classics",
                Price = 5000,
                CategoryID = category.ID,
                ImageURL = "superstar.jpeg"
            };
            using (var context = new ECommerceDbContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();
                context.Products.Add(product);
                context.SaveChanges();
            }
            using (var context = new ECommerceDbContext(options))
            {

                var sut = new ProductRepository(context);

                var actual = sut.Retrieve(product.ID);

                Assert.NotNull(actual);
                Assert.Equal(product.Name, actual.Name);
                Assert.Equal(product.Description, actual.Description);
                Assert.Equal(product.Price, actual.Price);
                Assert.Equal(product.CategoryID, actual.CategoryID);
                Assert.Equal(product.ImageURL, actual.ImageURL);

            }
            ////Arrange
            //var context = new ECommerceDbContext();
            //var productSut = new ProductRepository(context);
            //var categorySut = new CategoryRepository(context);
            //var category = new Category
            //{
            //    Name = "Shoes",
            //    Description = "Shoes Department",
            //    IsActive = true
            //};
            //categorySut.Create(category);

            //var product = new Product
            //{
            //    Name = "Superstar Shoes",
            //    Description = "Adidas Superstar Edition",
            //    Price = 100,
            //    CategoryID = category.ID,
            //    ImageURL ="Superstar.jpeg"
            //};
            //productSut.Create(product);

            ////Act
            //Assert.True(product.ID != 0);

            ////Assert
            //var prodEntity = productSut.Retrieve(product.ID);
            //Assert.NotNull(prodEntity);
            ////Clean

            //productSut.Delete(product.ID);
            //categorySut.Delete(category.ID);
        }
        #endregion
        #region Update_WithValidEntity_ShouldUpdateRecordsOntoDatabase
        [Fact]
        public void Update_WithValidEntity_ShouldUpdateRecordsOntoDatabase()
        {
            var connectionBuilder = new SqliteConnectionStringBuilder()
            {
                DataSource = ":memory:"
            };
            var connection = new SqliteConnection(connectionBuilder.ConnectionString);
            var options = new DbContextOptionsBuilder<ECommerceDbContext>().UseSqlite(connection).Options;
            var expectedName = "New Superstar";
            var expectedDescription = "Superstar Classics";
            var expectedPrice = 7500;
            var expectedImage = "newsuperstarimg.jpeg";
            int expectedID = 0;
            //FirstArrange
            var category = new Category
            {
                Name = "Shoes",
                Description = "Shoe Department",
                IsActive = true
            };
            using (var context = new ECommerceDbContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();
                context.Categories.Add(category);
                context.SaveChanges();
            }
            var product = new Product
            {
                Name = "Old Superstar Adidas",
                Description = " Old Adidas Classics",
                Price = 5000,
                CategoryID = category.ID,
                ImageURL = "oldsuperstar.jpeg"

            };
            using (var context = new ECommerceDbContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();
                context.Products.Add(product);
                context.SaveChanges();
                expectedID = product.ID;
            }
            using (var context = new ECommerceDbContext(options))
            {
                //SecondArrange
                var entity = context.Products.Find(expectedID);
                entity.Name = expectedName;
                entity.Description = expectedDescription;
                entity.Price = expectedPrice;
                entity.ImageURL = expectedImage;

                var sut = new ProductRepository(context);
                //Act
                sut.Update(entity.ID, entity);
                //Assert
                var actual = context.Products.Find(expectedID);
                Assert.Equal(expectedName, actual.Name);
                Assert.Equal(expectedDescription, actual.Description);
                Assert.Equal(expectedPrice, actual.Price);
                Assert.Equal(expectedImage, actual.ImageURL);

            }

            //{
            //    //arrange
            //    var context = new ECommerceDbContext();
            //    var sut = new ProductRepository(context);
            //    var categoryRepo = new CategoryRepository(context);
            //    var category = new Category
            //    {
            //        Name = "Shoes",
            //        Description = "Shoes Department",
            //        IsActive = true
            //    };
            //    categoryRepo.Create(category);
            //    var product = new Product
            //    {
            //        Name = "Adidas Shoes",
            //        Description = "Adidas Department",
            //        Price = 1500,
            //        CategoryID = category.ID,
            //        ImageURL = "OldSuperstar.jpeg"
            //    };

            //    sut.Create(product);
            //    //Update
            //    var actual = sut.Retrieve(product.ID);

            //    var expectedName = "Adidas Superstar Shoes";
            //    var expectedDescription = "Adidas Classic";
            //    var expectedPrice = 1800;
            //    var expectedImageURL = "NewSuperstar.jpeg";

            //    actual.Name = expectedName;
            //    actual.Description = expectedDescription;
            //    actual.Price = expectedPrice;
            //    actual.ImageURL = expectedImageURL;

            //    //Act
            //    sut.Update(actual.ID, actual);

            //    //Assert
            //    var expectedResult = sut.Retrieve(actual.ID);
            //    Assert.Equal(expectedName, expectedResult.Name);
            //    Assert.Equal(expectedDescription, expectedResult.Description);
            //    Assert.Equal(expectedPrice, expectedResult.Price);
            //    Assert.Equal(expectedImageURL, expectedResult.ImageURL);

            //    //Clean Up
            //    sut.Delete(product.ID);
            //    categoryRepo.Delete(category.ID);
        }
        #endregion
        #region Retrieve_WithNonExistentEntityID_ReturnsNull
        [Fact]
        public void Retrieve_WithNonExistentEntityID_ReturnsNull()
        {
            var connectionBuilder = new SqliteConnectionStringBuilder()
            {
                DataSource = ":memory:"
            };
            var connection = new SqliteConnection(connectionBuilder.ConnectionString);
            var options = new DbContextOptionsBuilder<ECommerceDbContext>().UseSqlite(connection).Options;

            using (var context = new ECommerceDbContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();
                //Arrange
                var sut = new ProductRepository(context);
                //Act
                var actual = sut.Retrieve(-1);
                Assert.Null(actual);
            }
        }
        #endregion
        #region DeleteProduct_WithValidData_ShouldRemoveIntoDatabase
        [Fact]
        public void DeleteProduct_WithValidData_ShouldRemoveIntoDatabase()
        {
            var connectionBuilder = new SqliteConnectionStringBuilder()
            {
                DataSource = ":memory:"
            };
            var connection = new SqliteConnection(connectionBuilder.ConnectionString);
            var options = new DbContextOptionsBuilder<ECommerceDbContext>().UseSqlite(connection).Options;
            var category = new Category
            {
                Name = "Shoes",
                Description = "Shoe Department",
                IsActive = true
            };
            using (var context = new ECommerceDbContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();
                context.Categories.Add(category);
                context.SaveChanges();
            }
            var product = new Product
            {
                Name = "Superstar Adidas",
                Description = "Adidas Classics",
                Price = 5000,
                CategoryID = category.ID,
                ImageURL = "superstar.jpeg"
            };
            using (var context = new ECommerceDbContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();
                context.Products.Add(product);
                context.SaveChanges();
            }

            using (var context = new ECommerceDbContext(options))
            {
                var sut = new ProductRepository(context);
                //Act
                sut.Delete(product.ID);
                //Assert
                product = context.Products.Find(product.ID);
                Assert.Null(product);
            }
            //    }
            //    //ASsert
            //    var context = new ECommerceDbContext();
            //    var productSut = new ProductRepository(context);
            //    var categorySut = new CategoryRepository(context);
            //    var category = new Category
            //    {
            //        Name = "Computer",
            //        Description = "Computer Department"
            //    };
            //    categorySut.Create(category);
            //    var product = new Product
            //    {
            //        Name = "Asus Rog",
            //        Description = "ROG Rog Masters",
            //        Price = 80000,
            //        CategoryID = category.ID,
            //        ImageURL = "asusrog.jpeg"
            //    };
            //    productSut.Create(product);

            //    //Act
            //    var actual = productSut.Retrieve(product.ID);
            //    //Assert
            //    productSut.Delete(actual.ID);
            //    Assert.Null(productSut.Retrieve(actual.ID));

            //    categorySut.Delete(category.ID);
            //}
            
        }
        #endregion
        #region Retrieve_WithSkipAndCount_ReturnsTheCorrectPage()
        [Fact]
        public void Retrieve_WithSkipAndCount_ReturnsTheCorrectPage()
        {
            var connectionBuilder = new SqliteConnectionStringBuilder()
            {
                DataSource = ":memory:"
            };
            var connection = new SqliteConnection(connectionBuilder.ConnectionString);
            var options = new DbContextOptionsBuilder<ECommerceDbContext>().UseSqlite(connection).Options;
            var category = new Category
            {
                Name = "Shoes",
                Description = "Shoe Department",
                IsActive = true
            };
            using (var context = new ECommerceDbContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();
                context.Categories.Add(category);
                context.SaveChanges();
            }
            using (var context = new ECommerceDbContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();
                //Arrange
                for (var i = 1; i < 20; i += 1)
                {
                    context.Products.Add(new Product
                    {
                        Name = string.Format("Add Products {0}", i),
                        Description = string.Format("Add Description {0}", i),
                        Price = decimal.Add(100, i),
                        CategoryID = category.ID,
                        ImageURL = string.Format("ImageURL {0}", i)
                    });
                }
                context.SaveChanges();
            }
            using (var context = new ECommerceDbContext(options))
            {
                //Act
                var sut = new ProductRepository(context);
                var list = sut.Retrieve(5, 5);
                //Arrange
                Assert.True(list.Count() == 5);
            }
        }
        #endregion
    }
}
