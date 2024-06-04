namespace RiverBooks.SharedKernel.Authentication;

public interface IUserClaimsProvider
{
    string? GetClaim(string claimType);
    string? GetEmailAddress();
    Guid? GetId();
}
