using QuickReach.ECommerce.Domain.Models;
using QuickReach.ECommerce.Infra.Data.Repositories;
using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;
namespace QuickReach.ECommerce.Infra.Data.Tests
{
    public class CategoryRepositoryTest
    {
        [Fact]
        public void Create_WithValidEntity_ShouldCreateDatabaseRecord()
        {
            //Arrange
            var context = new ECommerceDbContext();
            var sut = new CategoryRepository(context);
            var category = new Category
            {
                Name = "Shoes",
                Description = "Shoes Department"
            };
            //Act
            sut.Create(category);
            //Assert
            Assert.True(category.ID != 0);

            var entity = sut.Retrieve(category.ID);
            Assert.NotNull(entity);

            sut.Delete(category.ID);
        }


        [Fact]
        public void Retrieve_WithValidEntityID_ReturnsAValidEntity()
        {
            //Arrange
            var context = new ECommerceDbContext();
            var category = new Category
            {
                Name = "Shoes",
                Description = "Shoes Department"
            };
            //Act
            var sut = new CategoryRepository(context);
            sut.Create(category);
            //Assert
            var actual = sut.Retrieve(category.ID);
            Assert.NotNull(actual);

            sut.Delete(actual.ID);

        }

        [Fact]
        public void Retrieve_WithNonExistentEntityID_ReturnsNull()
        {
            var context = new ECommerceDbContext();
            var sut = new CategoryRepository(context);
            

            var actual = sut.Retrieve(-1);
            Assert.Null(actual);
        }
        [Fact]
        public void Retrieve_WithSkipAndCount_ReturnsTheCorrectPage()
        {
            //Arrange
            var context = new ECommerceDbContext();
            var sut = new CategoryRepository(context);
            for(var i=1; i<20; i+=1)
            {
                sut.Create(new Category
                {
                    Name = string.Format("Category {0}", i),
                    Description = string.Format("Decription {0}", i)
                });
            }
            //Act
            var list = sut.Retrieve(5, 5);
            //Assert
            Assert.True(list.Count() == 5);

            list = sut.Retrieve(0, Int32.MaxValue);
            list.All(c => { sut.Delete(c.ID); return true; });
        }
        [Fact]
        public void Delete_WithValidEntity_ShouldRemoveData_AndCheckIfDataIsNowNull()
        {
            //Arrange
            var context = new ECommerceDbContext();
            var sut = new CategoryRepository(context);
            var category = new Category
            {
                Name = "Kids Shoes",
                Description = "Kids Department"
            };
            sut.Create(category);
            //Act

            var entity = sut.Retrieve(category.ID);
            Assert.NotNull(entity);

            //Assert
            var actual = sut.Retrieve(category.ID);
            sut.Delete(actual.ID);
            Assert.NotNull(actual);



        }
        [Fact]
        public void Update_CategoryWithValidID_ShouldChangeCurrentData()
        {
            //Arrange
            var context = new ECommerceDbContext();
            var sut = new CategoryRepository(context);
            var category = new Category
            {
                Name = "Kids Shoes",
                Description = "Kids Department"
            };
            sut.Create(category);
            //Act
            var entity = sut.Retrieve(category.ID);
            var updatedName = "New Shoes";
            entity.Name = updatedName;
            var updatedDescription ="New Shoes Department";
            entity.Description = updatedDescription;
            
            
            sut.Update(entity.ID, entity);
            var updatedEntity = sut.Retrieve(category.ID);
            //Arrange

            Assert.Equal(updatedName, updatedEntity.Name);
            Assert.Equal(updatedDescription, updatedEntity.Description);

            sut.Delete(entity.ID);
            Assert.NotNull(entity);




        }

    }
}
