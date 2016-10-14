using System;
using System.Collections.Generic;
using System.Security.Claims;
using MienDev.AspNetCore.Identity.MongoDB.Models;
using MienDev.AspNetCore.Identity.MongoDB.Utils;

namespace MienDev.AspNetCore.Identity.MongoDB
{
    public class IdentityRole
    {
        public virtual string Id{get;set;}
        public virtual string Name{get;set;}
        /// <summary>
        ///     Normalized role name
        /// </summary>
        public virtual string NormalizedName { get; set; }

        /// <summary>
        ///     Concurrency stamp 
        /// </summary>
        public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        ///     Navigation property for claims in the role
        /// </summary>
        public virtual ICollection<MongoUserClaim> Claims { get; } = new List<MongoUserClaim>();
    }
}