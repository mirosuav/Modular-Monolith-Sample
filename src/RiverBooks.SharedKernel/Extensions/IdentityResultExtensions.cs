using Microsoft.AspNetCore.Identity;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.SharedKernel.Extensions;

public static class IdentityResultExtensions
{
    public static List<Error> AsErrors(this IdentityResult identityResult) =>
        identityResult.Errors
        .Select(error => Error.CreateFailure(error.Code, error.Description))
        .ToList();
}


