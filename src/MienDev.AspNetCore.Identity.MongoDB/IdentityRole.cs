using System;
using System.Collections.Generic;
using MienDev.AspNetCore.Identity.MongoDB.Models;

namespace MienDev.AspNetCore.Identity.MongoDB
{
    public class IdentityRole
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IdentityRole"/>.
        /// </summary>
        public IdentityRole() { }

        /// <summary>
        /// Initializes a new instance of <see cref="IdentityRole"/>.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        public IdentityRole(string roleName) : this()
        {
            Name = roleName;
        }

        /// <summary>
        /// Gets or sets the primary key for this role.
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// Gets or sets the name for this role.
        /// </summary>
        public virtual string Name { get; set; }

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