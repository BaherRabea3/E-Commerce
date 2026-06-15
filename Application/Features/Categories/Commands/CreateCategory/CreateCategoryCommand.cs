
using Domain.Common;
using MediatR;

namespace Application.Features.Categories.Commands.CreateCategory
{
    public sealed record CreateCategoryCommand(string name , string description) : IRequest<Result<int>>
    {
    }
}
