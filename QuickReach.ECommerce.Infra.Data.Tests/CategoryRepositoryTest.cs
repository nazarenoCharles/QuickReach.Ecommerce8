using QuickReach.ECommerce.Domain.Models;
using QuickReach.ECommerce.Infra.Data.Repositories;
using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace QuickReach.ECommerce.Infra.Data.Tests
{
    public class CategoryRepositoryTest
    {
        #region Create_WithValidEntity_ShouldCreateDatabaseRecord
        [Fact]
        public void Create_WithValidEntity_ShouldCreateDatabaseRecord()
        {
            // Arrange
            var connectionBuilder = new SqliteConnectionStringBuilder()
            {
                DataSource = ":memory:"
            };
            var connection = new SqliteConnection(connectionBuilder.ConnectionString);
            var options = new DbContextOptionsBuilder<ECommerceDbContext>().UseSqlite(connection).Options;
            var expected = new Category
            {
                Name = "Shoes",
                Description = "Shoes Department"
            };

            // Act
            using (var context = new ECommerceDbContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();
                var sut = new CategoryRepository(context);

                sut.Create(expected);
            }
            // Assert
            using (var context = new ECommerceDbContext(options))
            {
                var actual = context.Categories.Find(expected.ID);

                Assert.NotNull(actual);
                Assert.Equal(expected.Name, actual.Name);
                Assert.Equal(expected.Description, actual.Description);

            }
        }
        #endregion

        #region Retrieve_WithValidEntityID_ReturnsAValidEntity
        [Fact]
        public void Retrieve_WithValidEntityID_ReturnsAValidEntity()
        {
            //Arrange
            var connectionBuilder = new SqliteConnectionStringBuilder()
            {
                DataSource = ":memory:"
            };
            var connection = new SqliteConnection(connectionBuilder.ConnectionString);
            var options = new DbContextOptionsBuilder<ECommerceDbContext>().UseSqlite(connection).Options;
            var expected = new Category
            {
                Name = "Shoes",
                Description = "Shoes Department",
                IsActive = true
            };
            using (var context = new ECommerceDbContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();
                var sut = new CategoryRepository(context);
                sut.Create(expected);
            }
            using (var context = new ECommerceDbContext(options))
            {
                var sut = new CategoryRepository(context);

                //ACT
                var actual = sut.Retrieve(expected.ID);
                //Assert
                Assert.NotNull(actual);

                Assert.Equal(expected.Name, actual.Name);
                Assert.Equal(expected.Description, actual.Description);
            }
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
                var sut = new CategoryRepository(context);
                //Act
                var actual = sut.Retrieve(-1);
                Assert.Null(actual);
            }
        }
        #endregion

        #region Retrieve_WithSkipAndCount_ReturnsTheCorrectPage
        [Fact]
        public void Retrieve_WithSkipAndCount_ReturnsTheCorrectPage()
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
                for (var i = 1; i < 20; i += 1)
                {
                    context.Categories.Add(new Category
                    {
                        Name = string.Format("Category {0}", i),
                        Description = string.Format("Decription {0}", i)
                    });
                }
                context.SaveChanges();
            }

            //Act and Assert
            using (var context = new ECommerceDbContext(options))
            {
                var sut = new CategoryRepository(context);

                var list = sut.Retrieve(5, 5);
                Assert.True(list.Count() == 5);
            }
        }
        #endregion

        #region Delete_WithValidEntity_ShouldRemoveData_AndCheckIfDataIsNotNull
        [Fact]
        public void Delete_WithValidEntity_ShouldRemoveData_AndCheckIfDataIsNotNull()
        {
            var connectionBuilder = new SqliteConnectionStringBuilder()
            {
                DataSource = ":memory:"
            };
            var connection = new SqliteConnection(connectionBuilder.ConnectionString);
            var options = new DbContextOptionsBuilder<ECommerceDbContext>().UseSqlite(connection).Options;
            var entity = new Category
            {
                Name = "Shoes",
                Description = "Shoes Department"
            };
            using (var context = new ECommerceDbContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();
                //Arrange
                context.Categories.Add(entity);
                context.SaveChanges();
            }

            using (var context = new ECommerceDbContext(options))
            {
                var sut = new CategoryRepository(context);
                //Act
                sut.Delete(entity.ID);
                //Assert
                entity = context.Categories.Find(entity.ID);
                Assert.Null(entity);
            }
        }
        #endregion

        #region Update_WithValidEntity_ShouldChangeCurrentData
        [Fact]
        public void Update_WithValidEntity_ShouldChangeCurrentData()
        {
            var connectionBuilder = new SqliteConnectionStringBuilder()
            {
                DataSource = ":memory:"
            };
            var connection = new SqliteConnection(connectionBuilder.ConnectionString);
            var options = new DbContextOptionsBuilder<ECommerceDbContext>().UseSqlite(connection).Options;

            var expectName = "Updated Category";
            var expectDescription = "Updated Description";
            int expectedID = 0;

            using(var context = new ECommerceDbContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();
                //FirstArrange
                var entity = new Category
                {
                    Name = "Old Category",
                    Description = "old Description"
                };

                context.Categories.Add(entity);
                context.SaveChanges();
                expectedID = entity.ID;
            }
            using (var context = new ECommerceDbContext(options))
            {
    
                //SecondArrange
                var entity = context.Categories.Find(expectedID);
               
                entity.Name = expectName;
                entity.Description = expectDescription;

                var sut = new CategoryRepository(context);
                //Act
                sut.Update(entity.ID, entity);

                var actual = context.Categories.Find(expectedID);
                //Assert
                Assert.Equal(expectName, actual.Name);
                Assert.Equal(expectDescription, actual.Description);
            }
        }
        #endregion

        #region Delete_CategorybutProductHasData_ShouldThrowandException
        [Fact]
        public void Delete_CategorybutProductHasData_ShouldThrowException()
        {
            
            var connectionBuilder = new SqliteConnectionStringBuilder()
            {
                DataSource = ":memory:"
            };
            var connection = new SqliteConnection(connectionBuilder.ConnectionString);
            var options = new DbContextOptionsBuilder<ECommerceDbContext>().UseSqlite(connection).Options;
            //ARRANGE
            var category = new Category
            {
                Name = "Cellphone",
                Description = "Cellphone Department"
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
                Name = "IPhone X Max",
                Description = "iPhone X Max 128GB Rose Gold",
                Price = 50000,
                CategoryID = category.ID,
                ImageURL = "https://ss7.vzw.com/is/image/VerizonWireless/iPhoneX-Svr-back"
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
                var sut = new CategoryRepository(context);
                var actual = context.Categories.Find(category.ID);

                Assert.Throws<SystemException>(() => sut.Delete(category.ID));

            }

        }
        #endregion
    }
}
