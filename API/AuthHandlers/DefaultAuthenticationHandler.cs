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

public class DefaultAuthenticationHandler : AuthenticationHandler<DefaultAuthenticationSchemeOptions>
{
    public DefaultAuthenticationHandler(
        IOptionsMonitor<DefaultAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    ) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        return Task.FromResult(AuthenticateResult.NoResult());
    }
}