using Application.Common.DTOs.ProductDTOs;
using Application.Features.Products.Queries.GetProductById;
using AutoFixture;
using Domain.Entities.Categories;
using Domain.Entities.Products;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Tests.Products
{
    public class GetProductByIdQueryTest
    {
        private readonly GetProductByIdQueryHandler _handler;
        private readonly IFixture _fixture;

        public GetProductByIdQueryTest()
        {
            var categories = new List<Category>
                {
                    new Category { Id = 1, Name = "Electronics", Description = "greeafherh" },
                    new Category { Id = 2, Name = "Accessories", Description = "greeafhenrh" },
                    new Category { Id = 3, Name = "Footwear", Description = "greeafaherh" }
                };

            var products = new List<Product>
                {
                    new Product { Id = 1, Name = "Laptop", CategoryId = 1, UnitPrice = 1000, Quantity = 10, Description = "kgifggeorigi", RowVersion = new byte[8] },
                    new Product { Id = 2, Name = "Mobile", CategoryId = 1, UnitPrice = 500, Quantity = 20 , Description = "kgifggeorigic", RowVersion = new byte[8]},
                    new Product { Id = 3, Name = "Airpods", CategoryId = 2, UnitPrice = 200, Quantity = 15 , Description = "kgifggeorigis", RowVersion = new byte[8]},
                    new Product { Id = 4, Name = "Smart Watch", CategoryId = 2, UnitPrice = 300, Quantity = 5 , Description = "kgifggeorigih", RowVersion = new byte[8]},
                    new Product { Id = 5, Name = "Boot", CategoryId = 3, UnitPrice = 150, Quantity = 8 , Description = "kgifggeorigij", RowVersion = new byte[8]}
                };

            var options = new DbContextOptionsBuilder<AppDbContext>()
                          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                          .Options;

            var context = new AppDbContext(options);

            context.AddRange(products);
            context.AddRange(categories);

            context.SaveChanges();

            _handler = new GetProductByIdQueryHandler(context);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task Handler_Should_ReturnFailure_When_ProductNotFound()
        {
            // Arrange
            var command = new GetProductByIdQuery(10);
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public async Task Handler_Should_ReturnProduct_When_ProductIsFound()
        {
            // Arrange
            var command = new GetProductByIdQuery(3);
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().BeOfType<ProductDto>();
        }

    }
}
