using MienDev.AspNetCore.Identity.MongoDB.Utils;

namespace MienDev.AspNetCore.Identity.MongoDB.Models
{
    public class UserMobile : UserContactRecord
    {
        #region Contructor
        public UserMobile()
        {
            base.Type = ContactType.Mobile;
        }

        public UserMobile(string phone) : this()
        {
            phone.ThrowIfStringEmpty();
            Value = phone;
        } 
        #endregion

        #region Overrides of UserContactRecord

        /// <summary>
        /// Indicate the contact record type, such as mobile, email, telephone.
        /// </summary>
        public override ContactType Type =>ContactType.Mobile;

        #endregion
    }
}
