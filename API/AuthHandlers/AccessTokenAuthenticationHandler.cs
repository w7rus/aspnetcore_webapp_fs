using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace API.AuthHandlers;

public class AccessTokenAuthenticationHandler : AuthenticationHandler<AccessTokenAuthenticationSchemeOptions>
{
    public AccessTokenAuthenticationHandler(
        IOptionsMonitor<AccessTokenAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    ) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var executingEndpoint = Context.GetEndpoint();

        if (executingEndpoint == null)
            return Task.FromResult(AuthenticateResult.Fail(new NullReferenceException(nameof(executingEndpoint))));
        
        if (executingEndpoint.Metadata.OfType<AllowAnonymousAttribute>().Any()
            || executingEndpoint.Metadata.OfType<AllowAnonymousAttribute>().Any())
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(), Scheme.Name)));
        
        var authorizationBearerPayload =
            Context.Request.Headers[HeaderNames.Authorization].SingleOrDefault()?.Split(" ").Last();
        
        if (string.IsNullOrEmpty(authorizationBearerPayload))
            Context.Request.Cookies.TryGetValue(CookieKey.AccessToken, out authorizationBearerPayload);

        if (string.IsNullOrEmpty(authorizationBearerPayload))
            return Task.FromResult(AuthenticateResult.Fail(Localize.Error.AccessTokenNotProvided));
        
        if (authorizationBearerPayload != Options.AccessToken)
            return Task.FromResult(AuthenticateResult.Fail(new NullReferenceException(nameof(executingEndpoint))));

        var claimsIdentity = new ClaimsIdentity(new List<Claim>(), nameof(AccessTokenAuthenticationHandler));
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}