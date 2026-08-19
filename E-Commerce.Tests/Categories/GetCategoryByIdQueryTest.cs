using Application.Common.DTOs.CategoryDTOs;
using Application.Features.Categories.Queries.GetCategories;
using Application.Features.Categories.Queries.GetCategoryByid;
using AutoFixture;
using Domain.Entities.Categories;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Tests.Categories
{
    public class GetCategoryByIdQueryTest
    {

        private readonly GetCategoryByIdQueryHandler _handler;
        private readonly IFixture _fixture;

        public GetCategoryByIdQueryTest()
        {
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Electronics", Description = "greeafherh" },
                new Category { Id = 2, Name = "Accessories", Description = "greeafhenrh" },
                new Category { Id = 3, Name = "Footwear", Description = "greeafaherh" }
            };

            var options = new DbContextOptionsBuilder<AppDbContext>()
                          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                          .Options;

            var context = new AppDbContext(options);

            context.AddRange(categories);

            context.SaveChanges();

            _handler = new GetCategoryByIdQueryHandler(context);
            _fixture = new Fixture();
        }


        [Fact]
        public async Task Handler_Should_ReturnFailure_When_CategoryIsNotFound()
        {
            var command = new GetCategoryByIdQuery(5);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CategoryErrors.NotFound(5));
        }
        [Fact]
        public async Task Handler_Should_ReturnSuccessWithCategoryDto()
        {
            var command = new GetCategoryByIdQuery(1);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<CategoryDetailsDto>();
        }
    }
}
