using System;

namespace MienDev.AspNetCore.Identity.MongoDB.Utils
{
    public static class ExceptionThrowExtenstions
    {
        /// <summary>
        /// Throw exception if para null
        /// A convenint method
        /// since 'param' is formal parameter.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="message">show the message that about this error</param>
        public static void ThrowIfNull(this object obj, string message = null)
        {           
            if (obj == null) throw new ArgumentNullException(message);
        }

        /// <summary>
        /// Throw If String is Null Or WhiteSpace
        /// </summary>
        /// <param name="str"></param>
        /// <param name="message"></param>
        public static void ThrowIfStringEmpty(this string str, string message = null)
        {
           if(string.IsNullOrWhiteSpace(str)) throw new ArgumentNullException(message);
        }
    }
}
