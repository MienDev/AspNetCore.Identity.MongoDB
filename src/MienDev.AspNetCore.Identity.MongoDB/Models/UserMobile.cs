using System;
using MienDev.AspNetCore.Identity.MongoDB.Utils;

namespace MienDev.AspNetCore.Identity.MongoDB.Models
{
    public class UserMobile : UserContactRecord
    {
        public UserMobile(string phone)
        {
            if (phone.IsEmpty())
            {
                throw new ArgumentException($"{nameof(phone)} is required.");
            }

            Value = phone;
        }

        public override ContactType Type => ContactType.Mobile;
    }
}
