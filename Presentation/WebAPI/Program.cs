// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application;

using Infrastructure;
using Infrastructure.DataAccessManagers.EFCores;
using Infrastructure.SeedManagers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.OpenApi.Models;

using WebAPI.Common.Filters;
using WebAPI.Common.Handlers;
using WebAPI.Common.Middlewares;

var builder = WebApplication.CreateBuilder(args);


//>>> Create Logs folder for Serilog
var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Logs");
if (!Directory.Exists(logPath))
{
    Directory.CreateDirectory(logPath);
}

//>>> Create Docs folder for DocumentManager
var docPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Docs");
if (!Directory.Exists(docPath))
{
    Directory.CreateDirectory(docPath);
}

//>>> Create Imgs folder for ImageManager
var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Imgs");
if (!Directory.Exists(imgPath))
{
    Directory.CreateDirectory(imgPath);
}

//>>> Infrastructure Layer
builder.Services.AddInfrastructureServices(builder.Configuration);

//>>> Application Layer
builder.Services.AddApplicationServices();

//>>> Common
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});
builder.Services.AddRazorPages();
builder.Services.AddControllers()
    .AddOData(opt => opt
        .Count()
        .Filter()
        .Expand()
        .Select()
        .OrderBy()
        .SetMaxTop(null));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Indotalent API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

    c.OperationFilter<SwaggerOperationFilter>();
});


builder.Services.Configure<ApiBehaviorOptions>(x =>
{
    x.SuppressModelStateInvalidFilter = true;
});

//>>> Register Seeder
builder.Services.RegisterSystemSeedManager(builder.Configuration);
builder.Services.RegisterDemoSeedManager(builder.Configuration);

var app = builder.Build();

//create database
app.CreateDatabase();

//seed database with system data
app.SeedSystemData();

//seed database with demo data
if (app.Configuration.GetValue<bool>("IsDemoVersion"))
{
    app.SeedDemoData();
}

app.UseExceptionHandler(options => { });

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("EnableSwaggerInProduction"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Indotalent V1");
    });

}

if (app.Environment.IsDevelopment())
{

    //no cache during development
    app.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
            ctx.Context.Response.Headers.Append("Pragma", "no-cache");
            ctx.Context.Response.Headers.Append("Expires", "0");
        }
    });

}


await app.RunAsync();
