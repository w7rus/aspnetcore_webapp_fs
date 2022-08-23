using System.Diagnostics;
using API.AuthHandlers;
using API.Extensions;
using Common.Enums;
using Common.Filters;
using Common.Models;
using Common.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;

namespace API;

public class Startup
{
    #region Ctor

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        _env = env;
        Configuration = configuration;
    }

    #endregion

    #region Fields

    private readonly IWebHostEnvironment _env;
    private IConfiguration Configuration { get; }

    #endregion

    #region Methods

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCustomOptions(Configuration);
        services.AddCustomConfigureOptions();

        services.AddCors();

        services
            .AddControllers(options => { options.Filters.Add<HttpResponseExceptionFilter>(); })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API",
                Version = "v1",
                Description = "An API of ASP.NET Core Web Application File Server",
                Contact = new OpenApiContact
                {
                    Name = "Grigory (w7rus) Bragin",
                    Url = new Uri("https://t.me/w7rus"),
                    Email = "bragingrigory@gmail.com"
                }
            });

            c.EnableAnnotations();
        });

        services.AddHttpContextAccessor();

        var miscOptions = Configuration
            .GetSection(nameof(MiscOptions))
            .Get<MiscOptions>();

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(miscOptions.CorsAllowedOrigins).AllowAnyMethod().AllowAnyHeader();
            });
        });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errorModelResult = new ErrorModelResult
                {
                    TraceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier
                };

                foreach (var modelError in context.ModelState.Values.SelectMany(modelStateValue =>
                             modelStateValue.Errors))
                    errorModelResult.Errors.Add(new ErrorModelResultEntry(ErrorType.ModelState,
                        modelError.ErrorMessage));

                return new BadRequestObjectResult(errorModelResult);
            };
        });

        //Authentication
        {
            services.AddAuthentication()
                .AddScheme<DefaultAuthenticationSchemeOptions, DefaultAuthenticationHandler>(
                    AuthenticationSchemes.Default, null!);
            services.AddAuthentication()
                .AddScheme<AccessTokenAuthenticationSchemeOptions, AccessTokenAuthenticationHandler>(
                    AuthenticationSchemes.AccessToken, null!);
        }

        services.AddServices();
        services.AddHandlers();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<MiscOptions> miscOptions)
    {
        Log.Logger.Information("Startup.Configure()");
        Log.Logger.Information($"EnvironmentName: {env.EnvironmentName}");

        if (env.IsDevelopment())
        {
            // app.UseDeveloperExceptionPage();

            Log.Logger.Information("Add Swagger & SwaggerUI");
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
        }
        else
        {
            // app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "[{httpContextTraceIdentifier}] {httpContextRequestProtocol} {httpContextRequestMethod} {httpContextRequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("httpContextTraceIdentifier",
                    Activity.Current?.Id ?? httpContext.TraceIdentifier);
                diagnosticContext.Set("httpContextConnectionId", httpContext.Connection.Id);
                diagnosticContext.Set("httpContextConnectionRemoteIpAddress", httpContext.Connection.RemoteIpAddress);
                diagnosticContext.Set("httpContextConnectionRemotePort", httpContext.Connection.RemotePort);
                diagnosticContext.Set("httpContextRequestHost", httpContext.Request.Host);
                diagnosticContext.Set("httpContextRequestPath", httpContext.Request.Path);
                diagnosticContext.Set("httpContextRequestProtocol", httpContext.Request.Protocol);
                diagnosticContext.Set("httpContextRequestIsHttps", httpContext.Request.IsHttps);
                diagnosticContext.Set("httpContextRequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("httpContextRequestMethod", httpContext.Request.Method);
                diagnosticContext.Set("httpContextRequestContentType", httpContext.Request.ContentType);
                diagnosticContext.Set("httpContextRequestContentLength", httpContext.Request.ContentLength);
                diagnosticContext.Set("httpContextRequestQueryString", httpContext.Request.QueryString);
                diagnosticContext.Set("httpContextRequestQuery", httpContext.Request.Query);
                diagnosticContext.Set("httpContextRequestHeaders", httpContext.Request.Headers);
                diagnosticContext.Set("httpContextRequestCookies", httpContext.Request.Cookies);
            };
        });

        app.UseExceptionHandler("/Error");

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    #endregion
}