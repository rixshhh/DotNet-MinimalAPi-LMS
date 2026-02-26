using LMSMinimalApi.Persistence;
using LMSMinimalApi.Services;
using LMSMinimalApi.Web.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services
    .AddScoped<BookServices>()
    .AddScoped<UserServices>()
    .AddScoped<CategoryServices>()
    .AddScoped<BookIssuedServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

var apiGroup = app.MapGroup("api");

apiGroup.MapBookEndpoints()
    .MapUserEndpoints()
    .MapCategoryEndpoints()
    .MapBookIssuedEndpoints();

app.MapGet("/", () => $"Running in {app.Environment.EnvironmentName} right now.");


app.Run();