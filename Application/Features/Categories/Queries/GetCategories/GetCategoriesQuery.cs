
using Application.Common.DTOs.CategoryDTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Categories.Queries.GetCategories
{
    public sealed record GetCategoriesQuery() : IRequest<Result<List<CategoryDTO>>>
    {
    }
}
