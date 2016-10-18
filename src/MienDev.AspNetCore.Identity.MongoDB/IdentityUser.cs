using System;
using System.Collections.Generic;
using System.Security.Claims;
using MienDev.AspNetCore.Identity.MongoDB.Models;
using MienDev.AspNetCore.Identity.MongoDB.Utils;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MienDev.AspNetCore.Identity.MongoDB
{
    #region IdentityUser
    /// <summary>
    /// Identity User with default role type
    /// </summary>
    public class IdentityUser : IdentityUser<IdentityRole>
    {
        public IdentityUser() : base() { }
        public IdentityUser(string userName) : base(userName) { }
    }
    #endregion

    #region IdentityUser<TUserRole>

    /// <summary>
    /// Class for Identity User
    /// keep the same name with entityframework Store
    /// </summary>
    public class IdentityUser<TUserRole>
    {

        #region Contructor
        /// <summary>
        /// Initializes a new instance of <see cref="IdentityUser"/>.
        /// </summary>
        /// <remarks>
        /// The Id property is initialized to from a new GUID string value.
        /// </remarks>
        public IdentityUser()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="IdentityUser"/>.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <remarks>
        /// The Id property is initialized to from a new GUID string value.
        /// </remarks>
        public IdentityUser(string userName) : this()
        {
            UserName = userName;
        }

        public IdentityUser(string userName, string userEmail) : this(userName)
        {
            Email = new UserEmail(userEmail);
        }

        #endregion


        /// <summary>
        /// Gets or sets the primary key for this user.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the user name for this user.
        /// </summary>
        [BsonRequired]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the normalized user name for this user.
        /// </summary>
        public string NormalizedUserName { get; set; }

        /// <summary>
        /// Gets or sets the email address for this user.
        /// </summary>
        public virtual UserEmail Email { get; set; }

        /// <summary>
        /// Gets or sets a telephone number for the user.
        /// </summary>
        public virtual UserMobile PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if two factor authentication is enabled for this user.
        /// </summary>
        /// <value>True if 2fa is enabled, otherwise false.</value>
        public virtual bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// Gets or sets a salted and hashed representation of the password for this user.
        /// </summary>
        public virtual string PasswordHash { get; set; }

        /// <summary>
        /// A random value that must change whenever a users credentials change (password changed, login removed)
        /// </summary>
        public virtual string SecurityStamp { get; set; }

        /// <summary>
        /// A random value that must change whenever a user is persisted to the store
        /// </summary>
        public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        #region LockoutEnabled
        /// <summary>
        /// Gets or sets the date and time, in UTC, when any user lockout ends.
        /// </summary>
        /// <remarks>
        /// A value in the past means the user is not locked out.
        /// </remarks>
        public virtual DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if the user could be locked out.
        /// </summary>
        /// <value>True if the user could be locked out, otherwise false.</value>
        public virtual bool LockoutEnabled { get; set; }
           //  => LockoutEnd.HasValue && LockoutEnd.Value.ToUniversalTime().Subtract(DateTime.UtcNow).TotalSeconds > 0;

        /// <summary>
        /// Gets or sets the number of failed login attempts for the current user.
        /// </summary>
        public virtual int AccessFailedCount { get; set; }
        #endregion

        #region Claims Logins
        ///// <summary>
        ///// Navigation property for the roles this user belongs to.
        ///// </summary>
        public virtual ICollection<TUserRole> Roles { get; set; } = new List<TUserRole>();
        // public virtual ICollection<TUserRole> Roles { get; } = new List<TUserRole>();

        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public virtual ICollection<UserClaim> Claims { get; } = new List<UserClaim>();

        /// <summary>
        /// Navigation property for this users login accounts.
        /// </summary>
        public virtual ICollection<UserLogin> Logins { get; } = new List<UserLogin>();
        #endregion

        #region Addon (diffrent from aspnet/Identity/IdentityUser)
        /// <summary>
        /// Nullable UTC time that user is created On, default null.
        /// </summary>
        public DateTime? CreatedOn { get; set; } = null;

        /// <summary>
        /// Nullable UTC time that user is created On, default null.
        /// </summary>
        public DateTime? DeletedOn { get; set; }
        #endregion

        /// <summary>
        /// Returns the username for this user.
        /// </summary>
        public override string ToString()
        {
            return UserName;
        }

        #region Helper Method( can remove )
        public virtual void EnableTwoFactorAuthentication()
        {
            TwoFactorEnabled = true;
        }

        public virtual void DisableTwoFactorAuthentication()
        {
            TwoFactorEnabled = false;
        }

        public virtual void EnableLockout()
        {
            LockoutEnabled = true;
        }

        public virtual void DisableLockout()
        {
            LockoutEnabled = false;
        }
        #endregion


        public virtual void SetEmail(string email)
        {
            Email = new UserEmail(email);
        }

        public virtual void SetPhoneNumber(string phoneNumber)
        {
            var mongoUserPhoneNumber = new UserMobile(phoneNumber);
            SetPhoneNumber(mongoUserPhoneNumber);
        }

        public virtual void SetPhoneNumber(UserMobile mongoUserPhoneNumber)
        {
            PhoneNumber = mongoUserPhoneNumber;
        }

        public virtual void SetPasswordHash(string passwordHash)
        {
            PasswordHash = passwordHash;
        }

        public virtual void SetSecurityStamp(string securityStamp)
        {
            SecurityStamp = securityStamp;
        }

        public virtual void SetAccessFailedCount(int accessFailedCount)
        {
            AccessFailedCount = accessFailedCount;
        }

        public virtual void ResetAccessFailedCount()
        {
            AccessFailedCount = 0;
        }

        public virtual void LockUntil(DateTime lockoutEndDate)
        {
            LockoutEnd = lockoutEndDate; // new FutureOccurrence(lockoutEndDate);
        }

        public virtual void AddClaim(Claim claim)
        {
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            AddClaim(new UserClaim(claim));
        }

        public virtual void AddClaim(UserClaim mongoUserClaim)
        {
            if (mongoUserClaim == null)
            {
                throw new ArgumentNullException(nameof(mongoUserClaim));
            }

            Claims.Add(mongoUserClaim);
        }

        public virtual void RemoveClaim(UserClaim mongoUserClaim)
        {
            if (mongoUserClaim == null)
            {
                throw new ArgumentNullException(nameof(mongoUserClaim));
            }

            Claims.Remove(mongoUserClaim);
        }

        public virtual void AddLogin(UserLogin mongoUserLogin)
        {
            mongoUserLogin.ThrowIfNull($"{nameof(mongoUserLogin)} is required.");

            Logins.Add(mongoUserLogin);
        }

        public virtual void RemoveLogin(UserLogin mongoUserLogin)
        {
            if (mongoUserLogin == null)
            {
                throw new ArgumentNullException(nameof(mongoUserLogin));
            }

            Logins.Remove(mongoUserLogin);
        }

        public void Delete()
        {
            if (DeletedOn != null)
            {
                throw new InvalidOperationException($"User '{Id}' has already been deleted.");
            }

            DeletedOn = DateTime.UtcNow;
        }

        private static string GenerateId(string userName)
        {
            return Guid.NewGuid().ToString("N");
            //return userName.ToLower();
        }



    }

    #endregion
}