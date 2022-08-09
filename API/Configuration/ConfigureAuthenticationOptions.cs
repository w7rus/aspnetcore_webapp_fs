using Common.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace API.Configuration;

public class ConfigureAuthenticationOptions : IConfigureOptions<AuthenticationOptions>
{
    public void Configure(AuthenticationOptions options)
    {
        options.DefaultAuthenticateScheme = AuthenticationSchemes.Default;
        // options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }
}