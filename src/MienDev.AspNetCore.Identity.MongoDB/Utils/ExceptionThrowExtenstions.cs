using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MienDev.AspNetCore.Identity.MongoDB.Utils
{
    public static class ExceptionThrowExtenstions
    {
        /// <summary>
        /// Throw exception if para null
        /// A convenint method
        /// However, use this method, you would miss the real param Name information, 
        /// since 'param' is formal parameter.
        /// </summary>
        /// <param name="param"></param>
        public static void ThrowIfNull(this object param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }
        }
    }
}
