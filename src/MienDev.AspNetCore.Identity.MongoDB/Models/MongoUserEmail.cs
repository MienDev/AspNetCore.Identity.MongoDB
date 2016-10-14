using System;
using MienDev.AspNetCore.Identity.MongoDB.Utils;

namespace MienDev.AspNetCore.Identity.MongoDB.Models
{
    public class MongoUserEmail : MongoUserContactRecord
    {
        public MongoUserEmail(string email = null)
        {
            if (email.IsEmpty())
            {
                throw new ArgumentException($"{nameof(email)} is required.");
            }

            Value = email;
        }

        public override ContactType Type => ContactType.Email;

        public void SetNormalizedEmail(string normalizedEmail)
        {
            NormalizedValue = normalizedEmail;
        }
    }
}
