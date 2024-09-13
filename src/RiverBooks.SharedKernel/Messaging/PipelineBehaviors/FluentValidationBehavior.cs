using FluentValidation;
using MediatR;
using RiverBooks.SharedKernel.Extensions;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.SharedKernel.Messaging.PipelineBehaviors;

public class FluentValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) :
  IPipelineBehavior<TRequest, TResponse>
  where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request,
      RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var errors = validationResults
                .Where(r => !r.IsValid)
                .SelectMany(r => r.AsErrors())
                .ToList();

            if (errors is [])
                return await next();

            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(ResultOf<>))
            {
                var resultType = typeof(TResponse).GetGenericArguments()[0];

                var factoryMethod = typeof(ResultOf<>)
                    .MakeGenericType(resultType)
                    .GetMethod(nameof(ResultOf<int>.Failure), [typeof(IEnumerable<Error>)]);

                if (factoryMethod is not null)
                {
                    return (TResponse)factoryMethod.Invoke(null, new object[] { errors })!;
                }
            }
            else
            {
                var validationFailures = validationResults
                    .Where(r => !r.IsValid)
                    .SelectMany(r => r.Errors)
                    .ToList();

                throw new ValidationException(validationFailures);
            }
        }

        return await next();
    }
}