using Domain.Common;
using FluentValidation;
using MediatR;

namespace Application.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : Result
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!validators.Any())
            {
                return await next(cancellationToken);
            }

            var context = new ValidationContext<TRequest>(request);

            var results = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var errors = results.SelectMany(x => x.Errors)
                .Where(e => e is not null)
                .Select(e => new Error(
                    e.PropertyName,
                    e.ErrorMessage))
                .Distinct()
                .ToArray();

            if (errors.Any())
            {
                return CreateValidationResult<TResponse>(errors);
            }

            return await next(cancellationToken);
        }
        private static TResult CreateValidationResult<TResult>(Error[] errors)
            where TResult : Result
        {
            if (typeof(TResult) == typeof(Result))
            {
                return (ValidationResult.WithErrors(errors) as TResult)! ;
            }

            object validationResult = typeof(ValidationResult<>)
                .GetGenericTypeDefinition()
                .MakeGenericType(typeof(TResult).GenericTypeArguments[0])
                .GetMethod(nameof(ValidationResult.WithErrors))!
                .Invoke(null , new object?[] { errors })!;

            return (TResult)validationResult;
        }
    }
}
