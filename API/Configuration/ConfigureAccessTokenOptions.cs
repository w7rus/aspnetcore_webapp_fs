using System.Text;
using API.AuthHandlers;
using Common.Models;
using Common.Options;
using Microsoft.Extensions.Options;

namespace API.Configuration;

public class ConfigureAccessTokenOptions : IConfigureNamedOptions<AccessTokenAuthenticationSchemeOptions>
{
    private readonly MiscOptions _miscOptions;

    public ConfigureAccessTokenOptions(IOptions<MiscOptions> miscOptions)
    {
        _miscOptions = miscOptions.Value;
    }

    public void Configure(string name, AccessTokenAuthenticationSchemeOptions options)
    {
        options.AccessToken = _miscOptions.AuthenticationAccessToken;
    }

    public void Configure(AccessTokenAuthenticationSchemeOptions options)
    {
        Configure(string.Empty, options);
    }
}