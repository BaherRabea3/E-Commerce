
using Domain.Common;
using MediatR;

namespace Application.Features.Categories.Commands.DeleteCategory
{
    public sealed record DeleteCategoryCommand(int id) : IRequest<Result>
    {
    }
}
