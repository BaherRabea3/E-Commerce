using Application.Common.DTOs.CategoryDTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Categories.Queries.GetCategoryByid
{
    public sealed record GetCategoryByIdQuery(int id) : IRequest<Result<CategoryDetailsDto>>
    {
    }
}
