using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildingBlocks.Messaging.Extensions
{
    public static class MassTransitSetup
    {
        public static IServiceCollection AddRabbitMqBus(
            this IServiceCollection services,
            IConfiguration configuration,
            Assembly consumersAssembly)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumers(consumersAssembly);
                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMq:Host"], h =>
                    {
                        h.Username(configuration["RabbitMq:Username"] ?? "guest");
                        h.Password(configuration["RabbitMq:Password"] ?? "guest");
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
