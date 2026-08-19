using Application.Common.Interfaces;
using Application.Features.Products.Commands.UpdateProduct;
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
    public class UpdateProductCommandTest
    {
        private readonly UpdateProductCommandHandler _handler;
        private readonly IFixture _fixture;
        DbContextMock<AppDbContext> dbContextMock;
        private readonly Mock<IFileService> _fileServiceMock;

        public UpdateProductCommandTest()
        {
            var Categories = new List<Category>()
            {
                new Category {Id = 1 }
            };
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
            dbContextMock.CreateDbSetMock(temp => temp.Categories, Categories);


            _fileServiceMock = new Mock<IFileService>();
            var fileService = _fileServiceMock.Object;

            _handler = new(_context, fileService);

            _fixture = new Fixture();
        }

        [Fact]
        public async Task Handler_Should_ReturnFailure_When_ProductIsNotFound()
        {
            // Arrange
            var command = new UpdateProductCommand(2, _fixture.Create<string>(), _fixture.Create<string>(), 12345m, 25, null, 1);
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public async Task Handler_Should_ReturnFailure_When_CategoryIsNotFound()
        {
            // Arrange
            var command = new UpdateProductCommand(1, _fixture.Create<string>(), _fixture.Create<string>(), 12345m, 25, null, 2);
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            result.IsSuccess.Should().BeFalse();
        }
        [Fact]
        public async Task Handler_Should_ReThrow_When_UploadFileFailed()
        {
            // Arrange
            _fixture.Register<IFormFile>(() =>
            {
                var bytes = _fixture.CreateMany<byte>().ToArray();
                Stream baseStream = new MemoryStream(bytes);
                return new FormFile(baseStream, 0, baseStream.Length, "image", "p.png")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/png"
                };
            });
            var image = _fixture.Create<IFormFile>();
            var command = new UpdateProductCommand(1, _fixture.Create<string>(), _fixture.Create<string>(), 12345m, 25, image, 1);

            _fileServiceMock.Setup(x => x.UploadFileAsync(image, "Products"))
                .ThrowsAsync(new Exception("error"));
            //Act
            var act = () => _handler.Handle(command, CancellationToken.None);
            // Assert
            await act.Should().ThrowAsync<Exception>();

            _fileServiceMock.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Once);
        }
        [Fact]
        public async Task Handler_Should_ReturnSuccess_When_UploadFileSuccess()
        {
            // Arrange
            _fixture.Register<IFormFile>(() =>
            {
                var bytes = _fixture.CreateMany<byte>().ToArray();
                Stream baseStream = new MemoryStream(bytes);
                return new FormFile(baseStream, 0, baseStream.Length, "image", "p.png")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/png"
                };
            });

            var image = _fixture.Create<IFormFile>();

            var command = new UpdateProductCommand(1, _fixture.Create<string>(), _fixture.Create<string>(), 12345m, 25, image, 1);

            _fileServiceMock.Setup(x => x.UploadFileAsync(image, "Products"))
                .ReturnsAsync("Products/p.png");
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            result.IsSuccess.Should().BeTrue();

            dbContextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
            _fileServiceMock.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Once);
        }

    }
}
