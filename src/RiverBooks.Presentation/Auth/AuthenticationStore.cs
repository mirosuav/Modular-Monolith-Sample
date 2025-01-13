using System.Diagnostics.CodeAnalysis;
using RiverBooks.SharedKernel.Authentication;

namespace RiverBooks.Presentation.Auth;

public class AuthenticationStore
{
    private AuthToken? _storedToken;

    public void SetToken(AuthToken token)
    {
        _storedToken = token;
    }

    public void ClearToken()
    {
        _storedToken = null;
    }

    public bool TryGetToken([NotNullWhen(true)]out AuthToken? token)
    {
        if (_storedToken is null or { Token: [] })
        {
            token = null;
            return false;
        }
        else
        {
            token = _storedToken;
            return true;
        }
    }

};

