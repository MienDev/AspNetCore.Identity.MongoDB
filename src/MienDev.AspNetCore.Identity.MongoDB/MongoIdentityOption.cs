using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MienDev.AspNetCore.Identity.MongoDB
{
    /// <summary>
    /// MongoIdentity Configurature Option
    /// </summary>
    public class MongoIdentityOption
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        /// <summary>
        /// UserCollectionName
        /// </summary>
        public string UserCollectionName { get; set; } = MongoIdentityConst.UserCollectionName;
    }
}
