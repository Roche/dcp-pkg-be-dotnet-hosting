using OpenDCP.Framework.ConfigurationProvider.Constants;
using OpenDCP.Framework.ConfigurationProvider.Factories;
using OpenDCP.Framework.ConfigurationProvider.Models;
using OpenDCP.Framework.ServiceDiscovery.Factories;
using OpenDCP.Framework.ServiceDiscovery.Interfaces;
using OpenDCP.Framework.ServiceDiscovery.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OpenDCP.Framework.Hosting.Extensions
{
    public static class DCPServiceCollectionExtensions
    {
        public static IServiceCollection AddDCPServiceCollection(
            this IServiceCollection services,
            IConfiguration initialConfiguration,
            SharedConfigurationOptions sharedOptions = null,
            SecretConfigurationProviderOptions secretOptions = null)
        {
            // Fallback to default options if none are provided
            sharedOptions ??= new SharedConfigurationOptions
            {
                AddHeaders = true,
                AddMemoryCache = true,
                AddMessageBus = true,
                AddSecretProvider = true,
                AddIdP = false,
            };

            secretOptions ??= new SecretConfigurationProviderOptions
            {
                AddEncryptionKey = true,
                AddModuleClient = true,
                AddAudience = true,
                AddHeaders = true,
                AddLegacyHeaders = true,
                AddMessageBusConfiguration = true,
                MessageBusType = MessageBusType.RabbitMQ
            };

            // 1. Register Service Discovery Configuration
            services.AddSingleton<IServiceDiscoveryConfiguration>(provider =>
            {
                var serviceDiscoveryFactory = new ServiceDiscoveryConfigurationFactory();
                var driver = initialConfiguration["ServiceDiscoveryProvider:Driver"];
                return serviceDiscoveryFactory.Load(driver);
            });

            // 2. Register Service Discovery Service
            services.AddSingleton<IServiceDiscovery>(provider =>
            {
                var serviceDiscoveryFactory = new ServiceDiscoveryFactory();
                var driver = initialConfiguration["ServiceDiscoveryProvider:Driver"];
                return serviceDiscoveryFactory.Load(driver, provider);
            });

            // 3. Register the Final Chained/Enriched Configuration
            services.AddSingleton<IConfiguration>(provider =>
            {
                // Step A: Load Shared Service Discovery Config
                var sharedConfigurationProvider = provider.GetRequiredService<IServiceDiscoveryConfiguration>();
                var sharedEnrichedConfig = sharedConfigurationProvider.AddSharedConfiguration(
                    services,
                    initialConfiguration,
                    sharedOptions
                ) ?? initialConfiguration;

                // Step B: Initialize Secret Provider using the shared config
                var secretConfigurationFactory = new SecretConfigurationProviderFactory();
                var secretDriver = initialConfiguration["SecretConfigurationProvider:Driver"];
                var secretProvider = secretConfigurationFactory.Load(
                    secretDriver,
                    null,
                    sharedEnrichedConfig
                );

                // Step C: Execute secret enrichment
                var fullyEnrichedConfig = secretProvider.AddSecretsToConfiguration(
                    sharedEnrichedConfig,
                    secretOptions
                ) ?? sharedEnrichedConfig;

                return fullyEnrichedConfig;
            });

            // 4. Register Secret Provider targeting the final unified config
            services.AddSingleton(provider =>
            {
                var enrichedConfig = provider.GetRequiredService<IConfiguration>();
                var secretConfigurationFactory = new SecretConfigurationProviderFactory();
                var secretDriver = initialConfiguration["SecretConfigurationProvider:Driver"];
                return secretConfigurationFactory.Load(secretDriver, null, enrichedConfig);
            });

            return services;
        }
    }
}
