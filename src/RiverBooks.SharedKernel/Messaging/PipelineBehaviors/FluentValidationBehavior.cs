using FluentValidation;
using MediatR;
using RiverBooks.SharedKernel.Extensions;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.SharedKernel.Messaging.PipelineBehaviors;

public class FluentValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) :
  IPipelineBehavior<TRequest, TResponse>
  where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(TRequest request,
      RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var resultErrors = validationResults.SelectMany(r => r.AsErrors()).ToList();
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

#nullable disable
            if (failures.Count != 0)
            {
                if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Resultable<>))
                {
                    var resultType = typeof(TResponse).GetGenericArguments()[0];
                    var invalidMethod = typeof(Resultable<>)
                        .MakeGenericType(resultType)
                        .GetMethod(nameof(Resultable<int>.Failure), [typeof(IEnumerable<Error>)]);

                    if (invalidMethod != null)
                    {
                        return (TResponse)invalidMethod.Invoke(null, new object[] { resultErrors });
                    }
                }
                else if (typeof(TResponse) == typeof(Resultable))
                {
                    return (TResponse)(object)Resultable.Failure(resultErrors);
                }
                else
                {
                    throw new ValidationException(failures);
                }
            }
#nullable enable
        }
        return await next();
    }
}
