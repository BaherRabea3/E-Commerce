using Application.Features.Categories.Commands.DeleteCategory;
using Application.Features.Categories.Commands.UpdateCategory;
using AutoFixture;
using Domain.Entities.Categories;
using Domain.Entities.Products;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace E_Commerce.Tests.Categories
{
    public class UpdateCategoryCommandTest
    {
        private readonly DbContextMock<AppDbContext> _dbContextMock;
        private readonly UpdateCategoryCommandHandler _handler;
        private readonly IFixture _fixture;
        public UpdateCategoryCommandTest()
        {
            var categories = new List<Category>()
            {
                new Category {Id = 1, Name = "Electronics", Description = "gejgergioeeo"},
                new Category { Id = 2, Name = "Accessories", Description = "greeafhenrh" }
            };

            _dbContextMock = new DbContextMock<AppDbContext>(
                new DbContextOptionsBuilder<AppDbContext>().Options
                );
            var context = _dbContextMock.Object;
            _dbContextMock.CreateDbSetMock(x => x.Categories, categories);

            _handler = new(context);

            _fixture = new Fixture();
        }

        [Fact]
        public async Task Handler_Should_ReturnFailure_When_CategoryIsNotFound()
        {
            //Arrange
            var command = new UpdateCategoryCommand(3, _fixture.Create<string>(), _fixture.Create<string>());
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);
            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CategoryErrors.NotFound(3));
        }
        [Fact]
        public async Task Handler_Should_ReturnFailure_When_CategoryNameIsDuplicated()
        {
            //Arrange
            var command = new UpdateCategoryCommand(2, "Electronics", _fixture.Create<string>());
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);
            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CategoryErrors.DuplicateName);
        }
        [Fact]
        public async Task Handler_Should_ReturnSuccess_When_CategoryNameIsUnique()
        {
            //Arrange
            var command = new UpdateCategoryCommand(2, "Clothes", _fixture.Create<string>());
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);
            //Assert
            result.IsSuccess.Should().BeTrue();
            _dbContextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None));
        }

    }
}
