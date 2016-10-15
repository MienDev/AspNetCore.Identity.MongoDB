using System;
using System.Threading;
using Microsoft.AspNetCore.Identity;
using MienDev.AspNetCore.Identity.MongoDB.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace MienDev.AspNetCore.Identity.MongoDB
{
    /// <summary>
    /// Mongo Config
    /// </summary>
    internal static class MongoConfig
    {
        private static bool _initialized;
        private static object _initializationLock = new object();
        private static object _initializationTarget;

        public static void EnsureConfigured()
        {
            EnsureConfiguredImpl();
        }

        private static void EnsureConfiguredImpl()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                Configure();
                return null;
            });
        }

        private static void Configure()
        {
            RegisterConventions();

            BsonClassMap.RegisterClassMap<IdentityUser>(cm =>
            {
                cm.AutoMap();
                cm.SetIdMember(cm.GetMemberMap(c => c.Id));
                cm.MapCreator(user => new IdentityUser(user.UserName));
            });

            BsonClassMap.RegisterClassMap<UserClaim>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(c => new UserClaim(c.ClaimType, c.ClaimValue));
            });

            BsonClassMap.RegisterClassMap<UserEmail>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(cr => new UserEmail(cr.Value));
            });

            BsonClassMap.RegisterClassMap<UserMobile>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(cr => new UserMobile(cr.Value));
            });

            BsonClassMap.RegisterClassMap<UserLogin>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(l => new UserLogin(new UserLoginInfo(l.LoginProvider, l.ProviderKey, l.ProviderDisplayName)));
            });
        }

        private static void RegisterConventions()
        {
            var pack = new ConventionPack
            {
                new IgnoreIfNullConvention(false),
                new CamelCaseElementNameConvention(),
                new EnumRepresentationConvention(BsonType.String)
            };

            ConventionRegistry.Register("AspNetCore.Identity.MongoDB", pack, IsConventionApplicable);
        }

        private static bool IsConventionApplicable(Type type)
        {
            return type == typeof(IdentityUser)
                   || type == typeof(UserClaim)
                   || type == typeof(UserContactRecord)
                   || type == typeof(UserEmail)
                   || type == typeof(UserLogin)
                   || type == typeof(UserMobile)
                ;
        }
    }
}