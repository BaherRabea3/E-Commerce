using Domain.Common;
using MediatR;

namespace Application.Features.Categories.Commands.UpdateCategory
{
    public sealed record UpdateCategoryCommand(int id , string name , string description) 
        : IRequest<Result>
    {
    }
}
