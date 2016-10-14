using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MienDev.AspNetCore.Identity.MongoDB;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OptionBuilderExtentsion
    {
        public static IServiceCollection AddMongoIdentity<TUser>(this IServiceCollection services, IConfiguration configuration)
            where TUser : IdentityUser
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            //services.Configure<MongoDbSettings>(Configuration.GetSection("MongoDb"));
            services.Configure<MongoOption>(configuration);

            // DI database context
            services.AddScoped<IMongoDatabase>(provider =>
            {
                var options = provider.GetService<IOptions<MongoOption>>();
                var client = new MongoClient(options.Value.ConnectionString);
                return client.GetDatabase(options.Value.DatabaseName);
            });

            services.AddTransient<IMongoCollection<TUser>>();

            //services.AddSingleton<IUserStore<TUser>>(provider =>
            //{
            //    var options = provider.GetService<IOptions<MongoOption>>();
            //    var client = new MongoClient(options.Value.ConnectionString);

            //    var database = client.GetDatabase(options.Value.DatabaseName);
            //    var loggerFactory = provider.GetService<ILoggerFactory>();

            //    return new MongoUserStore<TUser>(database, loggerFactory);
            //});
            //return services;
        }

        /// <summary>
        /// Adds an Entity Framework implementation of identity information stores.
        /// </summary>
        /// <param name="builder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddMongoDbStores<TContext>(this IdentityBuilder builder)
            where TContext:IMongoDatabase
        {
        
            builder.Services.TryAdd(GetDefaultServices(builder.UserType, builder.RoleType, typeof(TContext)));

            //services.AddSingleton<IUserStore<MongoIdentityUser>>(provider =>
            //{
            //    var options = provider.GetService<IOptions<MongoDbSettings>>();
            //    var client = new MongoClient(options.Value.ConnectionString);
            //    var database = client.GetDatabase(options.Value.DatabaseName);
            //    var loggerFactory = provider.GetService<ILoggerFactory>();

            //    return new MongoUserStore<MongoIdentityUser>(database, loggerFactory);
            //});

            return builder;
        }

        private static IServiceCollection GetDefaultServices(Type userType, Type roleType, Type contextType)
        {
            Type userStoreType;
            Type roleStoreType;

            userStoreType = typeof(MongoUserStore<>).MakeGenericType(userType, roleType, contextType);
            roleStoreType = typeof(MongoRoleStore<>).MakeGenericType(roleType, contextType);

            var services = new ServiceCollection();
            services.AddScoped(
                typeof(IUserStore<>).MakeGenericType(userType),
                userStoreType);
            services.AddScoped(
                typeof(IRoleStore<>).MakeGenericType(roleType),
                roleStoreType);
            return services;
        }
    }
}