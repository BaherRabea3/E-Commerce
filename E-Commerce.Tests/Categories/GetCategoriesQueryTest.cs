using Application.Common.DTOs.CategoryDTOs;
using Application.Features.Categories.Queries.GetCategories;
using AutoFixture;
using Domain.Entities.Categories;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Tests.Categories
{
    public class GetCategoriesQueryTest
    {

        private readonly GetCategoriesQueryHandler _handler;
        private readonly IFixture _fixture;

        public GetCategoriesQueryTest()
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

            _handler = new GetCategoriesQueryHandler(context);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task Handler_Should_ReturnSuccessWithListOfCategories()
        {
            var command = new GetCategoriesQuery();

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<List<CategoryDTO>>();
        }
    }
}
