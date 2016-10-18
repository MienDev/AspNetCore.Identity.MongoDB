using System;
using System.ComponentModel;
using MienDev.AspNetCore.Identity.MongoDB.Utils;

namespace MienDev.AspNetCore.Identity.MongoDB.Models
{
    /// <summary>
    /// Email ContactRecord
    /// </summary>
    [TypeConverter(typeof(UserEmailConverter))]
    public class UserEmail : UserContactRecord
    {
        #region Contructor
        public UserEmail()
        {
            base.Type = ContactType.Email;
        }

        public UserEmail(string email = null)
        {
            email.ThrowIfStringEmpty($"{nameof(email)} is required.");

            Value = email;
        }
        #endregion

        /// <summary>
        /// Indicate the contact record type, such as mobile, email, telephone.
        /// </summary>
        public override ContactType Type => ContactType.Email;

        public void SetNormalizedEmail(string normalizedEmail)
        {
            NormalizedValue = normalizedEmail;
        }
    }
}
