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

// Lê a string de conexão do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("Mysql");
builder.Services.AddDbContext<StockDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
    mysqlOptions => mysqlOptions.EnableRetryOnFailure(
        maxRetryCount: 10,
        maxRetryDelay: TimeSpan.FromSeconds(5),
        errorNumbersToAdd: null
    )));

var jwtSecret = builder.Configuration["Jwt:Key"] ?? "stock-service-jwt";
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
    Host = builder.Configuration["RabbitMQ:Host"] ?? "localhost",
    Username = builder.Configuration["RabbitMQ:User"] ?? "guest",
    Password = builder.Configuration["RabbitMQ:Pass"] ?? "guest",
    Queue = builder.Configuration["RabbitMQ:Queue"] ?? "order-created"
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