using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Mongo.Migration.Startup.DotNetCore
{
    public class MongoMigrationStartupFilter : IStartupFilter
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ILogger<MongoMigrationStartupFilter> _logger;
        private readonly IMongoMigration _migration;

        public MongoMigrationStartupFilter(IHostApplicationLifetime applicationLifetime, IServiceScopeFactory serviceScopeFactory)
            : this(applicationLifetime, serviceScopeFactory, NullLoggerFactory.Instance)
        {
        }

        public MongoMigrationStartupFilter(IHostApplicationLifetime applicationLifetime, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory)
        {
            _migration = serviceScopeFactory.CreateScope().ServiceProvider.GetService<IMongoMigration>();
            _logger = loggerFactory.CreateLogger<MongoMigrationStartupFilter>();
            _applicationLifetime = applicationLifetime;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            try
            {
                _logger.LogInformation("Running migration. Please wait....");
                _migration.Run();
                _logger.LogInformation("Migration has been done");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetType().ToString());
                _applicationLifetime.StopApplication();
            }

            return next;
        }
    }
}