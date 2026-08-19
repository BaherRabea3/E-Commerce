
using Application.Features.Categories.Commands.CreateCategory;
using Application.Features.Categories.Commands.DeleteCategory;
using AutoFixture;
using Domain.Entities.Categories;
using Domain.Entities.Products;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace E_Commerce.Tests.Categories
{
    public class DeleteCategoryCommandTest
    {
        private readonly DbContextMock<AppDbContext> _dbCotnextMock;
        private readonly DeleteCategoryCommandHandler _handler;
        private readonly IFixture _fixture;
        public DeleteCategoryCommandTest()
        {
            var categories = new List<Category>()
            {
                new Category {Id = 1, Name = "Electronics", Description = "gejgergioeeo"},
                new Category { Id = 2, Name = "Accessories", Description = "greeafhenrh" }
            };
            var products = new List<Product>
                {
                    new Product { Id = 1, Name = "Laptop", CategoryId = 1, UnitPrice = 1000, Quantity = 10, Description = "kgifggeorigi", RowVersion = new byte[8] },
                    new Product { Id = 2, Name = "Mobile", CategoryId = 1, UnitPrice = 500, Quantity = 20 , Description = "kgifggeorigic", RowVersion = new byte[8]},
                    new Product { Id = 3, Name = "Airpods", CategoryId = 1, UnitPrice = 200, Quantity = 15 , Description = "kgifggeorigis", RowVersion = new byte[8]},
                    new Product { Id = 4, Name = "Smart Watch", CategoryId = 1, UnitPrice = 300, Quantity = 5 , Description = "kgifggeorigih", RowVersion = new byte[8]}
                };

            _dbCotnextMock = new DbContextMock<AppDbContext>(
                new DbContextOptionsBuilder<AppDbContext>().Options
                );
            var context = _dbCotnextMock.Object;
            _dbCotnextMock.CreateDbSetMock(x => x.Categories, categories);
            _dbCotnextMock.CreateDbSetMock(x => x.Products, products);

            _handler = new(context);

            _fixture = new Fixture();
        }

        [Fact]
        public async Task Handler_Should_ReturnFailure_When_CategoryHasProducts()
        {
            //Arrange
            var command = new DeleteCategoryCommand(1);
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);
            //Assert
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public async Task Handler_Should_ReturnFailure_When_CategoryIsNotFound()
        {
            //Arrange
            var command = new DeleteCategoryCommand(3);
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);
            //Assert
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public async Task Handler_Should_ReturnSuccess_When_CategoryIsDoesNotHaveProducts()
        {
            //Arrange
            var command = new DeleteCategoryCommand(2);
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);
            //Assert
            result.IsSuccess.Should().BeTrue();
            _dbCotnextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None),Times.Once);
        }
    }
}
