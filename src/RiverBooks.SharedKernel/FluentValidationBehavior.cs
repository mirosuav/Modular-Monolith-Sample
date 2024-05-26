using FluentValidation;
using MediatR;
using RiverBooks.SharedKernel.Extensions;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.SharedKernel;

public class FluentValidationBehavior<TRequest, TResponse> :
  IPipelineBehavior<TRequest, TResponse>
  where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public FluentValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

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
                if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(ResultOr<>))
                {
                    var resultType = typeof(TResponse).GetGenericArguments()[0];
                    var invalidMethod = typeof(ResultOr<>)
                        .MakeGenericType(resultType)
                        .GetMethod(nameof(ResultOr<int>.Failure), new[] { typeof(IEnumerable<Error>) });

                    if (invalidMethod != null)
                    {
                        return (TResponse)invalidMethod.Invoke(null, new object[] { resultErrors });
                    }
                }
                else if (typeof(TResponse) == typeof(ResultOr))
                {
                    return (TResponse)(object)ResultOr.Failure(resultErrors);
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
