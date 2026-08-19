using Application.Common.Interfaces;
using Application.Features.Products.Commands.UpdateProduct;
using Application.Features.Products.Commands.UpdateProductStock;
using AutoFixture;
using Domain.Entities.Categories;
using Domain.Entities.Products;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace E_Commerce.Tests.Products
{
    public class UpdateProductStockCommandTest
    {
        private readonly UpdateProductStockCommandHndler _handler;
        private readonly IFixture _fixture;
        DbContextMock<AppDbContext> dbContextMock;
        public UpdateProductStockCommandTest()
        {
            var Products = new List<Product>()
            {
                new Product {Id = 1 , CategoryId = 1}
            };
            dbContextMock = new DbContextMock<AppDbContext>
            (
                new DbContextOptionsBuilder<AppDbContext>().Options
            );
            var _context = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Products, Products);

            _handler = new(_context);

            _fixture = new Fixture();
        }

        [Fact]
        public async Task Handler_Should_ReturnFailure_When_ProductIsNotFound()
        {
            // Arrange
            var command = new UpdateProductStockCommand(2, 25);
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public async Task Handler_Should_ReturnSuccess_When_UploadFileSuccess()
        {
            // Arrange
            var command = new UpdateProductStockCommand(1, 25);
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            result.IsSuccess.Should().BeTrue();

            dbContextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

    }
}
