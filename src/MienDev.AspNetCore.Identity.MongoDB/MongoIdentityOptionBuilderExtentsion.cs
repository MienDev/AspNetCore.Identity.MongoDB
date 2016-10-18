using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MienDev.AspNetCore.Identity.MongoDB;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MienDev.AspNetCore.Identity.MongoDB.Utils;
using MongoDB.Bson.IO;
using MongoDB.Driver;

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MongoIdentityOptionBuilderExtentsion
    {
        /// <summary>
        /// Config MongoDb Resolvable, and add MongoDBIdentity.
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
                var options = provider.GetService<IOptions<MongoIdentityOption>>().Value;
                options.ThrowIfNull(nameof(options));

                var client = new MongoClient(options.ConnectionString);
                return client.GetDatabase(options.DatabaseName);
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
        {

            builder.Services.TryAdd(GetDefaultServices(builder.UserType, builder.RoleType));

            return builder;
        }

        private static IServiceCollection GetDefaultServices(Type userType, Type roleType)
        {

            // 1. get interface type IUserStore<>, IRoleStore<>
            var contextType = typeof(IdentityDbContext<,>).MakeGenericType(userType, roleType);
            var userStoreInterfaceType = typeof(IUserStore<>).MakeGenericType(userType);
            if (userStoreInterfaceType == null) throw new ArgumentNullException(nameof(userStoreInterfaceType));

            var roleStoreInterfaceType = typeof(IRoleStore<>).MakeGenericType(roleType);
            if (roleStoreInterfaceType == null) throw new ArgumentNullException(nameof(roleStoreInterfaceType));

            // 2. get class type 
            var userStoreType = typeof(UserStore<,,>).MakeGenericType(userType, roleType, contextType);
            var roleStoreType = typeof(RoleStore<>).MakeGenericType(roleType);

            // 3. add to some new service collection
            var services = new ServiceCollection();
            services.AddScoped(userStoreInterfaceType, userStoreType);
            services.AddScoped(roleStoreInterfaceType, roleStoreType);

            return services;
        }
    }
}