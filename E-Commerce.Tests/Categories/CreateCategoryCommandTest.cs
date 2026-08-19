
using Application.Features.Categories.Commands.CreateCategory;
using AutoFixture;
using Domain.Entities.Categories;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace E_Commerce.Tests.Categories
{
    public class CreateCategoryCommandTest
    {
        private readonly DbContextMock<AppDbContext> _dbCotnextMock;
        private readonly CreateCategoryCommandHandler _handler;
        private readonly IFixture _fixture;
        public CreateCategoryCommandTest()
        {
            var categories = new List<Category>()
            {
                new Category {Id = 1, Name = "Electronics", Description = "gejgergioeeo"}
            };

            _dbCotnextMock = new DbContextMock<AppDbContext>(
                new DbContextOptionsBuilder<AppDbContext>().Options
                );
            var context = _dbCotnextMock.Object;
            _dbCotnextMock.CreateDbSetMock(x => x.Categories, categories);
            _handler = new(context);

            _fixture = new Fixture();
        }

        [Fact]
        public async Task Handler_Should_ReturnSuccess()
        {
            var command = new CreateCategoryCommand(_fixture.Create<string>(), _fixture.Create<string>());

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _dbCotnextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None),Times.Once);

        }
        [Fact]
        public async Task Handler_Should_ReturnFailure_When_NameIsDuplicate()
        {
            var command = new CreateCategoryCommand("Electronics", _fixture.Create<string>());

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
        }
    }
}
