using ASP.NET_Core_Web_Application_File_Server.Extensions;
using Common.Models;
using Common.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;

namespace ASP.NET_Core_Web_Application_File_Server;

public class Startup
{
    #region Fields

    private readonly IWebHostEnvironment _env;
    private IConfiguration Configuration { get; }

    #endregion

    #region Ctor

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        _env = env;
        Configuration = configuration;
    }

    #endregion

    #region Methods

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCustomLogging(Log.Logger, _env);

        services.AddCustomOptions(Configuration);

        services.AddCors();

        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API",
                Version = "v1",
                Description = "An API of ASP.NET Core Web Application File Server",
                Contact = new OpenApiContact
                {
                    Name = "Grigory Bragin",
                    Url = new Uri("https://t.me/w7rus"),
                    Email = "bragingrigory@gmail.com"
                }
            });

            c.EnableAnnotations();
        });

        services.AddHttpContextAccessor();

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errorModelResult = new ErrorModelResult
                {
                    Errors = new List<KeyValuePair<string, string>>()
                };

                foreach (var modelError in context.ModelState.Values.SelectMany(modelStateValue =>
                             modelStateValue.Errors))
                    errorModelResult.Errors.Add(new(Localize.ErrorType.ModelState,
                        modelError.ErrorMessage));

                return new BadRequestObjectResult(errorModelResult);
            };
        });

        services.AddServices();
        services.AddHandlers();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<MiscOptions> miscOptions)
    {
        Log.Logger.Debug("Configure!");

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors(options => options.WithOrigins(miscOptions.Value.CorsAllowedOrigins).AllowAnyMethod());

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    #endregion
}