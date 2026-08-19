using Application.Common.Interfaces;
using Application.Features.Products.Commands.CreateProduct;
using Application.Features.Products.Commands.DeleteProduct;
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
    public partial class DeleteProductCommandTest
    {
        private readonly DeleteProductCommandHandler _handler;
        private readonly IFixture _fixture;
        DbContextMock<AppDbContext> dbContextMock;
        public DeleteProductCommandTest()
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
            var command = new DeleteProductCommand(2);
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public async Task Handler_Should_ReturnSuccess_When_ProductIsFound()
        {
            // Arrange
            var command = new DeleteProductCommand(1);
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            result.IsSuccess.Should().BeTrue();
            dbContextMock.Verify(x => x.Products.Remove(It.IsAny<Product>()), Times.Once);
            dbContextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);


        }
    }
}
