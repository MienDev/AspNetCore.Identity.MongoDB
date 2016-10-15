using System;
using MienDev.AspNetCore.Identity.MongoDB.Utils;

namespace MienDev.AspNetCore.Identity.MongoDB.Models
{
    /// <summary>
    /// Email ContactRecord
    /// </summary>
    public class UserEmail : UserContactRecord
    {
        #region Constructor
        public UserEmail(string email = null)
        {
            if (email.IsEmpty())
            {
                throw new ArgumentException($"{nameof(email)} is required.");
            }
            Value = email;
        }
        #endregion

        public override ContactType Type => ContactType.Email;

        public void SetNormalizedEmail(string normalizedEmail)
        {
            NormalizedValue = normalizedEmail;
        }
    }
}
