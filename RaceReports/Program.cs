using Microsoft.EntityFrameworkCore;
using RaceReports.Data;

var builder = WebApplication.CreateBuilder(args);

// Registrerar controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Kopplar DbContext till SQL Server
builder.Services.AddDbContext<RaceReportsContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

// Swagger endast i development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
