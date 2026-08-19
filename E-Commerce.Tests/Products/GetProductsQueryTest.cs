using Application.Features.Products.Queries.GetProducts;
using AutoFixture;
using Domain.Entities.Categories;
using Domain.Entities.Products;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace E_Commerce.Tests.Products
{
    public class GetProductsQueryTest
    {
        private readonly GetProductsQueryHandler _handler;
        private readonly IFixture _fixture;

        public GetProductsQueryTest()
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

            _handler = new GetProductsQueryHandler(context);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task Handle_Should_ReturnAllProducts_When_NoFiltersApplied()
        {
            // Arrange
            var query = new GetProductsQuery(null, null, null, null, null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.items.Should().HaveCount(5);
        }

        [Fact]
        public async Task Handle_Should_FilterBySearchTerm()
        {
            // Arrange
            var query = new GetProductsQuery(null, null, "Laptop", null, null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.items.Should().HaveCount(1);
            result.Value.items.First().name.Should().Be("Laptop");
        }

        [Fact]
        public async Task Handle_Should_FilterByPrice()
        {
            // Arrange
            var query = new GetProductsQuery(null, 500, null, null, null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.items.Should().HaveCount(1);
            result.Value.items.First().unitPrice.Should().Be(500);
        }

        [Fact]
        public async Task Handle_Should_FilterByCategoryId()
        {
            // Arrange
            var query = new GetProductsQuery(2, null, null, null, null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.items.Should().HaveCount(2);
            result.Value.items.All(p => p.category == "Accessories").Should().BeTrue();
        }

        [Fact]
        public async Task Handle_Should_ApplyPagination()
        {
            // Arrange
            var query = new GetProductsQuery(null, null, null, 1, 2);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.items.Should().HaveCount(2);
            result.Value.page.Should().Be(1);
            result.Value.pageSize.Should().Be(2);
        }
    }
}
