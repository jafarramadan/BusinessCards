using BusinessCard.Data;
using BusinessCard.Models;
using BusinessCard.Repositories;
using BusinessCard.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            // Allow common Angular dev server ports
            policy.WithOrigins(
                    "http://localhost:4200",
                    "https://localhost:4200",
                    "http://localhost:4201",
                    "https://localhost:4201"
                  )
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // Allow cookies/credentials if needed
        });
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Entity Framework and DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BusinessCardsDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register repositories (following Dependency Inversion Principle - SOLID)
builder.Services.AddScoped<IRepository<Card>, CardRepository>();

// Register services (following Dependency Inversion Principle - SOLID)
builder.Services.AddScoped<ICardService, CardService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS must be called before UseAuthorization and UseRouting
app.UseCors("AllowAngularApp");

app.UseAuthorization();

app.MapControllers();
app.Run();
