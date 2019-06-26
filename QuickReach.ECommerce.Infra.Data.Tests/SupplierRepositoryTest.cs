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
    public class SupplierRepositoryTest
    {
        #region CreateSupplier_WithValidData_ShouldAddtoDatabase
        [Fact]
        public void CreateSupplier_WithValidData_ShouldAddtoDatabase()
        {
            //Arrange
            var connectionBuilder = new SqliteConnectionStringBuilder()
            {
                DataSource = ":memory:"
            };
            var connection = new SqliteConnection(connectionBuilder.ConnectionString);
            var options = new DbContextOptionsBuilder<ECommerceDbContext>().UseSqlite(connection).Options;

            var supplier = new Supplier
            {
                Name = "Shoe Supplier",
                Description = "Fast Supply"
            };
            using (var context = new ECommerceDbContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();
                //Act
                context.Suppliers.Add(supplier);
                context.SaveChanges();
            }
            using (var context = new ECommerceDbContext(options))
            {
                var actual = context.Suppliers.Find(supplier.ID);

                Assert.NotNull(actual);
                Assert.Equal(supplier.Name, actual.Name);
                Assert.Equal(supplier.Description, actual.Description);
            }
        }
        #endregion
        #region UpdateSupplier_WithValidData_ShouldUpdateandAddtoDatabase 
        [Fact]
        public void UpdateSupplier_WithValidData_ShouldUpdateandAddtoDatabase()
        {
            var connectionBuilder = new SqliteConnectionStringBuilder()
            {
                DataSource = ":memory:"
            };
            var connection = new SqliteConnection(connectionBuilder.ConnectionString);
            var options = new DbContextOptionsBuilder<ECommerceDbContext>().UseSqlite(connection).Options;
            //Arrange
            var expectedName = "Converse Supplier";
            var expectedDescription = "A Converse Brand";
            var expectedID = 0;

            using (var context = new ECommerceDbContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();
                var supplier = new Supplier
                {
                    Name = "Adidas Supplier",
                    Description = "Shoe Supplier"
                };
                context.Suppliers.Add(supplier);
                context.SaveChanges();
                expectedID = supplier.ID;
            }
            using (var context = new ECommerceDbContext(options))
            {
                var supplier = context.Suppliers.Find(expectedID);

                supplier.Name = expectedName;
                supplier.Description = expectedDescription;

                var sut = new SupplierRepository(context);
                //Act
                sut.Update(supplier.ID, supplier);

                var actual = context.Suppliers.Find(expectedID);
                //Assert
                Assert.Equal(expectedName, actual.Name);
                Assert.Equal(expectedDescription, actual.Description);
            }
        }
        #endregion
        #region RetrieveSupplier_HasPagination_ShowsCorrectData
        [Fact]
        public void RetrieveSupplier_WithSkipandCount_ReturnsontoCorrectPage()
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
                    context.Suppliers.Add(new Supplier
                    {
                        Name = string.Format("Add Supplier {0}", i),
                        Description = string.Format("Add Description {0}", i)
                    });
                }
                context.SaveChanges();
            }
            using (var context = new ECommerceDbContext(options))
            {
                //Act
                var sut = new SupplierRepository(context);
                var list = sut.Retrieve(5, 5);
                //Arrange
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
            var entity = new Supplier
            {
                Name = "Adidas Supplier",
                Description = "Supplier for Shoes",
                IsActive = true
            };

            using (var context = new ECommerceDbContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();
                //Arrange
                context.Suppliers.Add(entity);
                context.SaveChanges();
            }

            using (var context = new ECommerceDbContext(options))
            {
                var sut = new SupplierRepository(context);
                //Act
                sut.Delete(entity.ID);
                //Assert
                entity = context.Suppliers.Find(entity.ID);
                Assert.Null(entity);
            }
            #endregion
        }
        #region Retrieve_WithNonExistentEntityID_ReturnsNul
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
                var sut = new SupplierRepository(context);

                //Act
                var actual = sut.Retrieve(-1);
                Assert.Null(actual);
            }
        }
}
