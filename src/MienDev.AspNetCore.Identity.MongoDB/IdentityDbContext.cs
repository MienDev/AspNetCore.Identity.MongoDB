using System;
using Microsoft.AspNetCore.Identity;
using MienDev.AspNetCore.Identity.MongoDB.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace MienDev.AspNetCore.Identity.MongoDB
{
    /// <summary>
    /// mongo db context.
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TRole"></typeparam>
    public class IdentityDbContext<TUser, TRole>
        where TUser : IdentityUser
        where TRole : IdentityRole
    {
        public IMongoCollection<TUser> Users { get; set; }
        public IMongoCollection<TRole> Roles { get; set; }

        public IdentityDbContext()
        {
            // todo: indexing?
            RegisterConventions();
            RegisterMappings();
        }

        /// <summary>
        /// RegisterMappings
        /// </summary>
        protected void RegisterMappings()
        {
            
            if (!BsonClassMap.IsClassMapRegistered(typeof(IdentityUser)))
            {
                BsonClassMap.RegisterClassMap<IdentityUser>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIdMember(cm.GetMemberMap(c => c.Id));
                    cm.MapCreator(user => new IdentityUser(user.UserName));
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(UserClaim)))
            {

                BsonClassMap.RegisterClassMap<UserClaim>(cm =>
                {
                    cm.AutoMap();
                    cm.MapCreator(c => new UserClaim(c.ClaimType, c.ClaimValue));
                });
            }


            if (!BsonClassMap.IsClassMapRegistered(typeof(UserEmail)))
            {
                BsonClassMap.RegisterClassMap<UserEmail>(cm =>
                {
                    cm.AutoMap();
                    cm.MapCreator(cr => new UserEmail(cr.Value));
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(UserMobile)))
            {
                BsonClassMap.RegisterClassMap<UserMobile>(cm =>
                {
                    cm.AutoMap();
                    cm.MapCreator(cr => new UserMobile(cr.Value));
                });
            }


            if (!BsonClassMap.IsClassMapRegistered(typeof(UserLogin)))
            {
                BsonClassMap.RegisterClassMap<UserLogin>(cm =>
                {
                    cm.AutoMap();
                    cm.MapCreator(ul => new UserLogin(new UserLoginInfo(ul.LoginProvider, ul.ProviderKey, ul.ProviderDisplayName)));
                });
            }

        }

        /// <summary>
        /// RegisterConventions
        /// </summary>
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

        /// <summary>
        /// IsConventionApplicable
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsConventionApplicable(Type type)
        {
            return type == typeof(IdentityUser)
                   || type == typeof(UserClaim)
                   || type == typeof(UserContactRecord)
                   || type == typeof(UserEmail)
                   || type == typeof(UserMobile)
                   || type == typeof(UserLogin)
                ;
        }

        private static void RegisterConventionToNotSerializeEmptyLists()
        {
            //var pack = new ConventionPack();
            //pack.AddMemberMapConvention("Do not serialize empty lists", m =>
            //{
            //    if (typeof(ICollection).IsAssignableFrom(m.MemberType))
            //    {
            //        m.SetShouldSerializeMethod(instance =>
            //        {
            //            var value = (ICollection)m.Getter(instance);
            //            return value != null && value.Count > 0;
            //        });
            //    }
            //});
            //ConventionRegistry.Register("Do not serialize empty lists", pack, t => true);
        }
    }
}