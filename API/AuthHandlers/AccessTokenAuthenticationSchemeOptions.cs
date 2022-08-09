using Microsoft.AspNetCore.Authentication;

namespace API.AuthHandlers;

public class AccessTokenAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public string AccessToken { get; set; }
}