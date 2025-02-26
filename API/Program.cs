using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnectionString"));
});
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod()
    .WithOrigins("http://localhost:3000","https://localhost:3000"));

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try{
    var context = services.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await DbInitializer.SeedData(context);
}
catch(Exception ex){
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError("Error in migrations, details: "+ ex.Message);
}

app.Run();
