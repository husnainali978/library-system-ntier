using System.Text.Json.Serialization;
using Library.API.Middleware;
using Library.BusinessLogic;
using Library.DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Book <-> Loan <-> Member navigation properties form cycles; avoid
        // failing serialization when an entity is returned straight from the
        // Business Logic Layer.
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Each layer registers its own services - Program.cs is the only place that
// knows both the DataAccess and BusinessLogic layers exist.
var connectionString = builder.Configuration.GetConnectionString("LibraryDatabase")
    ?? "Data Source=library.db";

builder.Services.AddDataAccess(connectionString);
builder.Services.AddBusinessLogic();

var app = builder.Build();

// Ensure the SQLite database, schema and seed data exist on startup so the
// API is immediately usable without a manual migration step.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    db.Database.EnsureCreated();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
