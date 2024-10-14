using Bitly.Configurators;
using Bitly.Database;
using Bitly.Middleware;
using Bitly.UrlServices;
using Carter;
using FluentValidation;
using Serilog;

namespace Bitly;

public class Startup
{
    private readonly IConfiguration configuration;
    public Startup(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public void ConfigureHost(IHostBuilder host)
    {
        host.UseSerilog((context, loggerConfiguration) =>
         {
             loggerConfiguration.ReadFrom.Configuration(context.Configuration);
             loggerConfiguration.WriteTo.Console().WriteTo.Debug();
         });
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // logging
        services.AddLogging();
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        //swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        //database & caching
        services.AddScoped<AppDbContext>();
        services.AddMemoryCache();

        //config
        services.AddOptions<UrlShortnerConfig>()
            .Bind(configuration.GetSection(UrlShortnerConfig.SectionName))
            .ValidateDataAnnotations();

        //fluent-validator
        services.AddValidatorsFromAssembly(typeof(UrlModule).Assembly);

        // mediator & interceptors
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(UrlModule).Assembly);
            cfg.AddOpenBehavior(typeof(FluentValidationBehavior<,>));
        });

        //services
        services.RegisterAllServices();

        // hosting
        services.AddCarter();
        services.AddHttpContextAccessor();
    }

    public void Configure(IApplicationBuilder app)
    {
        var environment = app.ApplicationServices.GetRequiredService<IHostEnvironment>();

        // logging
        app.UseExceptionHandler();

        //swagger
        if (environment.IsDevelopment())
            app.UseSwagger().UseSwaggerUI();

        //carter will scan & add all endpoints
        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapCarter());
    }
}
