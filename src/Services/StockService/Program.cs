using Microsoft.EntityFrameworkCore;
using StockService.Data;
using System.Reflection;
using BuildingBlocks.Messaging.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRabbitMqBus(builder.Configuration, Assembly.GetExecutingAssembly());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Lê a string de conexão do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("Mysql");
builder.Services.AddDbContext<StockDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
    mysqlOptions => mysqlOptions.EnableRetryOnFailure(
        maxRetryCount: 10,
        maxRetryDelay: TimeSpan.FromSeconds(5),
        errorNumbersToAdd: null
    )));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stock API V1");
});
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<StockDbContext>();
    var retries = 5;
    while (retries > 0)
    {
        try
        {
            dbContext.Database.Migrate();
            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migration Falhou: {ex.Message}, tentando novamente...");
            Thread.Sleep(5000);
            retries--;
        }
    }
}

app.Run();