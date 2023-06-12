using Harvzor.Optional;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Morcatko.AspNetCore.JsonMergePatch;
using UpdateApi.Filters;

namespace UpdateApi;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        _configuration = configuration;
        _env = env;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddMvc()
            .AddSystemTextJsonMergePatch();
        
        services
            .AddControllers(options =>
            {
                // Microsoft's solution for getting JSON PATCH to work in a solution that mostly used JSON.NET
                // https://learn.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-7.0 
                options.InputFormatters.Insert(0, MyJPIF.GetJsonPatchInputFormatter());
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new Harvzor.Optional.SystemTextJson.OptionalJsonConverter());
            });
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.OperationFilter<JsonMergePatchDocumentOperationFilter>();
            
            // Solves issue with DotNext Optional and Harvzor Optional colliding.
            options.CustomSchemaIds(type => type.ToString());
            
            options.MapType<Optional<string>>(() => new OpenApiSchema
            {
                Type = "string"
            });
            
            options.MapType<Optional<bool>>(() => new OpenApiSchema
            {
                Type = "boolean"
            });
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();
        
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();
        
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
    
    private static class MyJPIF
    {
        public static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
        {
            var builder = new ServiceCollection()
                .AddLogging()
                .AddMvc()
                .AddNewtonsoftJson()
                .Services.BuildServiceProvider();

            return builder
                .GetRequiredService<IOptions<MvcOptions>>()
                .Value
                .InputFormatters
                .OfType<NewtonsoftJsonPatchInputFormatter>()
                .First();
        }
    }
}
