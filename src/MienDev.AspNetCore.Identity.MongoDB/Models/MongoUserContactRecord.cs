using System;

namespace MienDev.AspNetCore.Identity.MongoDB.Models
{
    /// <summary>
    /// a class for contact such as emails, phones
    /// </summary>
    public class MongoUserContactRecord : IEquatable<MongoUserContactRecord>
    {
        #region Props

        /// <summary>
        /// Indicate the contact record type, such as mobile, email, telephone.
        /// </summary>
        public virtual ContactType Type { get; set; } = ContactType.None;

        /// <summary>
        /// the contact vlaue 
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// the Normalized Value for unique.
        /// </summary>
        public virtual string NormalizedValue { get; set; }

        /// <summary>
        /// return is the contact info 
        /// </summary>
        public bool IsComfirmed { get; set; }

        /// <summary>
        /// return or set the UTC time on Confirm Occurrance.
        /// </summary>
        public DateTime? ConfirmedOn { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Set SetConfirmed
        /// </summary>
        /// <param name="timeOn"></param>
        public void SetConfirmed(DateTime? timeOn = null)
        {
            IsComfirmed = true;
            ConfirmedOn = timeOn ?? DateTime.UtcNow;
        }

        public void SetUnConfirmed(DateTime? timeOn = null)
        {
            IsComfirmed = false;
            ConfirmedOn = null;
        }

        /// <summary>
        /// override ToString, return the value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// override the equals.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MongoUserContactRecord other)
        {
            return other.NormalizedValue.Equals(NormalizedValue);
        } 
        #endregion
    }
}
