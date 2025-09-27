using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StockService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
builder.Services.AddDbContext<StockDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
    mysqlOptions => mysqlOptions.EnableRetryOnFailure(
        maxRetryCount: 10,
        maxRetryDelay: TimeSpan.FromSeconds(5),
        errorNumbersToAdd: null
    )));

var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "fallback-secret";
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var rabbitConfig = new
{
    Host = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost",
    Username = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest",
    Password = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "guest",
    Queue = Environment.GetEnvironmentVariable("RABBITMQ_QUEUE") ?? "order-created"
};
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitConfig.Host, "/", h =>
        {
            h.Username(rabbitConfig.Username);
            h.Password(rabbitConfig.Password);
        });
        cfg.ReceiveEndpoint(rabbitConfig.Queue!, e =>
        {
            e.Consumer<OrderCreatedConsumer>(context);
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
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
