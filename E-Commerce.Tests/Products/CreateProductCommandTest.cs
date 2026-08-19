using Application.Common.Interfaces;
using Application.Features.Products.Commands.CreateProduct;
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
    public class CreateProductCommandTest
    {

        
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly IFileService _fileService;
        private readonly CreateProductCommandHandler _handler;
        private readonly IFixture _fixture;
        private readonly AppDbContext _context;

        public CreateProductCommandTest()
        {
            var Categories = new List<Category>()
            {
                new Category { Id = 1, Name = "electronics", Description = "test"}
            };
            var Products = new List<Product>();
            DbContextMock<AppDbContext> dbContextMock = new DbContextMock<AppDbContext>
                (
                    new DbContextOptionsBuilder<AppDbContext>().Options
                );
            _context = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Categories, Categories);
            dbContextMock.CreateDbSetMock(temp => temp.Products, Products);

            _fileServiceMock = new Mock<IFileService>();
            _fileService = _fileServiceMock.Object;

            _handler = new(_fileService, _context);

            _fixture = new Fixture();
        }

        [Fact]
        public async Task Handler_Should_ReturnError_WhenCategoryNotExisted()
        {
            //Arrange
            CreateProductCommand Command = new("Laptop", "High-performance laptop", 1200.00m, 50, null, 10);

            //Act

            var result = await _handler.Handle(Command, default(CancellationToken));

            //Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Handler_Should_ReturnSuccess_WhenCategoryExisted()
        {
            //Arrange


            _fixture.Register<IFormFile>(() =>
            {
                var bytes = _fixture.CreateMany<byte>(100).ToArray();
                var stream = new MemoryStream(bytes);

                return new FormFile(stream, 0, stream.Length, "Image", "test.png")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/png"
                };
            });

            IFormFile Image = _fixture.Create<IFormFile>();


            CreateProductCommand Command = new("Laptop", "High-performance laptop", 1200.00m, 50, Image, 1);

            var ext = Path.GetExtension(Image.FileName).ToLower();

            var fileName = $"{Guid.NewGuid()}{ext}";

            _fileServiceMock.Setup(temp => temp.UploadFileAsync(Image, "Products"))
                .ReturnsAsync($"Products/{fileName}");

            //Act

            var result = await _handler.Handle(Command, default(CancellationToken));

            //Assert
            result.IsSuccess.Should().BeTrue();

            _fileServiceMock.Verify(x => x.UploadFileAsync(Image, "Products"), Times.Once());
        }

        [Fact]
        public async Task Handler_Should_ReThrow_WhenUploadFileFail()
        {
            //Arrange

            _fixture.Register<IFormFile>(() =>
            {
                var bytes = _fixture.CreateMany<byte>(100).ToArray();
                var stream = new MemoryStream(bytes);

                return new FormFile(stream, 0, stream.Length, "Image", "test.png")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/png"
                };
            });

            IFormFile Image = _fixture.Create<IFormFile>();


            CreateProductCommand Command = new("Laptop", "High-performance laptop", 1200.00m, 50, Image, 1);

            _fileServiceMock.Setup(temp => temp.UploadFileAsync(Image, "Products"))
                .ThrowsAsync(new IOException("disk is full"));

            //Act

            Func<Task> act = () => _handler.Handle(Command, default(CancellationToken));

            //Assert
            await act.Should().ThrowAsync<IOException>();

            _fileServiceMock.Verify(x => x.DeleteFile(It.IsAny<string?>()), Times.Once());
            _context.Products.Should().BeEmpty();
        }

    }
}
