using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MienDev.AspNetCore.Identity.MongoDB;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MienDev.AspNetCore.Identity.MongoDB.Utils;
using MongoDB.Driver;

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MongoIdentityOptionBuilderExtentsion
    {
        /// <summary>
        /// Make MongoDb Resolvable, and add MongoDBIdentity.
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <typeparam name="TRole"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddMongoIdentity<TUser, TRole>(this IServiceCollection services,
            IConfiguration configuration)
            where TUser : IdentityUser
            where TRole : IdentityRole
        {
            services.ThrowIfNull();

            #region For MongoDB

            // 1. get MongoIdentity options
            services.Configure<MongoIdentityOption>(configuration);

            // 2. config MongoConnection
            MongoConfig.EnsureConfigured();
            // 3. add database DI
            // TODO: Sigleliton?
            services.AddScoped<IMongoDatabase>(provider =>
            {
                var options = provider.GetService<IOptions<MongoIdentityOption>>();
                var client = new MongoClient(options.Value.ConnectionString);
                return client.GetDatabase(options.Value.DatabaseName);
            });

            // 3. Get IMongoCollection<Type>
            services.AddScoped<IMongoCollection<TUser>>(provider =>
            {
                var db = provider.GetService<IMongoDatabase>();
                return db.GetCollection<TUser>(typeof(TUser).Name);
            });

            services.AddScoped<IMongoCollection<TRole>>(provider =>
            {
                var db = provider.GetService<IMongoDatabase>();
                return db.GetCollection<TRole>(typeof(TRole).Name);
            });

            #endregion

            return services;
        }

        /// <summary>
        /// Adds an Entity Framework implementation of identity information stores.
        /// </summary>
        /// <param name="builder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddMongoIdentityStores(this IdentityBuilder builder)
            //where TContext : IMongoDatabase
        {
            builder.Services.TryAdd(GetDefaultServices(builder.UserType, builder.RoleType));

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

        private static IServiceCollection GetDefaultServices(Type userType, Type roleType)
        {
            Type IUserStoreType = typeof(IUserStore<>).MakeGenericType(userType);
            Type IRoleStoreType = typeof(IRoleStore<>).MakeGenericType(roleType);

            var userStoreType = typeof(UserStore<,>).MakeGenericType(userType, roleType);
            var roleStoreType = typeof(RoleStore<>).MakeGenericType(roleType);

            var services = new ServiceCollection();
            services.AddScoped(IUserStoreType, userStoreType);
            services.AddScoped(IRoleStoreType, roleStoreType);
            return services;
        }
    }
}