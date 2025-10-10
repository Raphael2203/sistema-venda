using System.Text;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BuildingBlocks.Messaging.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRabbitMqBus(builder.Configuration, Assembly.GetExecutingAssembly());
builder.Services.AddControllers();
builder.Services.AddHttpClient("StockService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:StockService"] ?? "http://localhost:5502");
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("Mysql");
builder.Services.AddDbContext<SalesDbContext>(options =>
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
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sales API V1");
});
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SalesDbContext>();
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