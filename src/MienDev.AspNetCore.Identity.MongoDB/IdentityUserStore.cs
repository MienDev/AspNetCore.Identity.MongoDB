using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using MienDev.AspNetCore.Identity.MongoDB.Models;
using MienDev.AspNetCore.Identity.MongoDB.Utils;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace MienDev.AspNetCore.Identity.MongoDB
{
    public class UserStore<TUser, TRole, TContext> :
            IUserLoginStore<TUser>,
            IUserRoleStore<TUser>,
            IUserClaimStore<TUser>,
            IUserPasswordStore<TUser>,
            IUserSecurityStampStore<TUser>,
            IUserEmailStore<TUser>,
            IUserLockoutStore<TUser>,
            IUserPhoneNumberStore<TUser>,
            IQueryableUserStore<TUser>,
            IUserTwoFactorStore<TUser>,
            IUserAuthenticationTokenStore<TUser>
        where TUser : IdentityUser
        where TRole : IdentityRole
        where TContext : IdentityDbContext<TUser, TRole>

    {
        private bool _disposed;
        public readonly TContext Context;
        // private readonly IMongoCollection<TUser> _usersCollection;

        public UserStore(TContext context)
        {
            Context = context;
        }

        #region IUserStore Section

        #region GetUserIdAsync of IUserStore<TUser>

        /// <summary>
        /// Gets the user identifier for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose identifier should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the identifier for the specified <paramref name="user" />.</returns>
        public virtual Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull();

            return Task.FromResult(ConvertIdToString(user.Id));
        }

        #endregion

        #region GetUserNameAsync of IUserStore<TUser>

        /// <summary>
        /// Gets the user name for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose name should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the name for the specified <paramref name="user" />.</returns>
        public virtual Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull();

            return Task.FromResult(user.UserName);
        }

        #endregion

        #region SetUserNameAsync of IUserStore<TUser>

        /// <summary>
        /// Sets the given <paramref name="userName" /> for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose name should be set.</param>
        /// <param name="userName">The user name to set.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull();

            user.UserName = userName;
            // todo: check whether need to set Normalized UserName here 

            return TaskCache.CompletedTask;
        }

        #endregion

        #region GetNormalizedUserNameAsync of IUserStore<TUser>

        /// <summary>
        /// Gets the normalized user name for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose normalized name should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the normalized user name for the specified <paramref name="user" />.</returns>
        public virtual Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull();

            return Task.FromResult(user.NormalizedUserName);
        }

        #endregion

        #region SetNormalizedUserNameAsync of IUserStore<TUser>

        /// <summary>
        /// Sets the given normalized name for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose name should be set.</param>
        /// <param name="normalizedName">The normalized name to set.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual Task SetNormalizedUserNameAsync(TUser user, string normalizedName,
            CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull();

            user.NormalizedUserName = normalizedName;
            return TaskCache.CompletedTask;
        }

        #endregion

        #region CreateAsync of IUserStore<TUser>

        /// <summary>
        /// Creates the specified <paramref name="user" /> in the user store.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the creation operation.</returns>
        public virtual async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull();

            await Context.Users
                .InsertOneAsync(user, null, cancellationToken)
                .ConfigureAwait(false);

            return IdentityResult.Success;
        }

        #endregion

        #region UpdateAsync of IUserStore<TUser>

        /// <summary>
        /// Updates the specified <paramref name="user" /> in the user store.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the update operation.</returns>
        public virtual async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull();

            var query = Builders<TUser>.Filter.And(
                Builders<TUser>.Filter.Eq(u => u.Id, user.Id),
                Builders<TUser>.Filter.Eq(u => u.DeletedOn, null));

            var replaceResult = await Context.Users
                .ReplaceOneAsync(query, user, new UpdateOptions { IsUpsert = false }, cancellationToken)
                .ConfigureAwait(false);

            return replaceResult.IsModifiedCountAvailable && replaceResult.ModifiedCount == 1
                ? IdentityResult.Success
                : IdentityResult.Failed();
        }

        #endregion

        #region DeleteAsync of IUserStore<TUser>

        /// <summary>
        /// Deletes the specified <paramref name="user" /> from the user store.
        /// </summary>
        /// <param name="user">The user to delete.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the update operation.</returns>
        public virtual async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull();

            // set deleted = DateTime.UtcNow
            user.Delete();

            var query = Builders<TUser>.Filter.Eq(u => u.Id, user.Id);
            var update = Builders<TUser>.Update.Set(u => u.DeletedOn, user.DeletedOn);

            await Context.Users.UpdateOneAsync(query, update, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return IdentityResult.Success;
        }

        #endregion

        #region FindByIdAsync of IUserStore<TUser>

        /// <summary>
        /// Finds and returns a user, if any, who has the specified <paramref name="userId" />.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the user matching the specified <paramref name="userId" /> if it exists.
        /// </returns>
        public virtual Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            userId.ThrowIfStringEmpty($"{nameof(userId)} is required for finding user.");

            var query = Builders<TUser>.Filter.And(
                Builders<TUser>.Filter.Eq(u => u.Id, userId),
                Builders<TUser>.Filter.Eq(u => u.DeletedOn, null)
            );

            return Context.Users.Find(query).FirstOrDefaultAsync(cancellationToken);
        }

        #endregion

        #region FindByNameAsync of IUserStore<TUser>

        /// <summary>
        /// Finds and returns a user, if any, who has the specified normalized user name.
        /// </summary>
        /// <param name="normalizedUserName">The normalized user name to search for.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the user matching the specified <paramref name="normalizedUserName" /> if it exists.
        /// </returns>
        public virtual Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            normalizedUserName.ThrowIfStringEmpty($"{nameof(normalizedUserName)} is required for finding user by Name.");

            var query = Builders<TUser>.Filter.And(
                Builders<TUser>.Filter.Eq(u => u.NormalizedUserName, normalizedUserName),
                Builders<TUser>.Filter.Eq(u => u.DeletedOn, null));

            return Context.Users.Find(query).FirstOrDefaultAsync(cancellationToken);
        }

        #endregion

        #endregion

        #region IUserLoginStore Section

        #region AddLoginAsync of IUserLoginStore<TUser>

        /// <summary>
        /// Adds an external <see cref="T:Microsoft.AspNetCore.Identity.UserLoginInfo" /> to the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user to add the login to.</param>
        /// <param name="login">The external <see cref="T:Microsoft.AspNetCore.Identity.UserLoginInfo" /> to add to the specified <paramref name="user" />.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is reuqired for add login.");
            login.ThrowIfNull($"{nameof(login)} is reuqired for add login.");

            // keep user login unique.
            if (user.Logins.Any(x => x.Equals(login)))
            {
                throw new InvalidOperationException("Login already exists.");
            }

            user.AddLogin(new UserLogin(login));

            // todo:not save to db

            return TaskCache.CompletedTask;
        }

        #endregion

        #region RemoveLoginAsync of IUserLoginStore<TUser>

        /// <summary>
        /// Attempts to remove the provided login information from the specified <paramref name="user" />.
        /// and returns a flag indicating whether the removal succeed or not.
        /// </summary>
        /// <param name="user">The user to remove the login information from.</param>
        /// <param name="loginProvider">The login provide whose information should be removed.</param>
        /// <param name="providerKey">The key given by the external login provider for the specified user.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey,
            CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required for remove login.");
            loginProvider.ThrowIfStringEmpty($"{nameof(loginProvider)} is required for remove login.");
            providerKey.ThrowIfStringEmpty($"{nameof(providerKey)} is required for remove login.");

            var login = new UserLoginInfo(loginProvider, providerKey, string.Empty);

            var loginToRemove = user.Logins.FirstOrDefault(l => l.Equals(login));
            if (loginToRemove != null)
            {
                user.RemoveLogin(loginToRemove);
            }

            return TaskCache.CompletedTask;
        }

        #endregion

        #region GetLoginsAsync of IUserLoginStore<TUser>

        /// <summary>
        /// Retrieves the associated logins for the specified <param ref="user" />.
        /// </summary>
        /// <param name="user">The user whose associated logins to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> for the asynchronous operation, containing a list of <see cref="T:Microsoft.AspNetCore.Identity.UserLoginInfo" /> for the specified <paramref name="user" />, if any.
        /// </returns>
        public virtual Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is reuquired.");

            var logins =
                user.Logins.Select(
                    login => new UserLoginInfo(
                        login.LoginProvider,
                        login.ProviderKey,
                        login.ProviderDisplayName));

            return Task.FromResult<IList<UserLoginInfo>>(logins.ToList());
        }

        #endregion

        #region FindByLoginAsync of IUserLoginStore<TUser>

        /// <summary>
        /// Retrieves the user associated with the specified login provider and login provider key..
        /// </summary>
        /// <param name="loginProvider">The login provider who provided the <paramref name="providerKey" />.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider" /> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> for the asynchronous operation, containing the user, if any which matched the specified login provider and key.
        /// </returns>
        public virtual Task<TUser> FindByLoginAsync(string loginProvider, string providerKey,
            CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            loginProvider.ThrowIfStringEmpty($"{nameof(loginProvider)} is required.");
            providerKey.ThrowIfStringEmpty($"{nameof(providerKey)} is required.");

            var notDeletedQuery = Builders<TUser>.Filter.Eq(u => u.DeletedOn, null);

            var loginQuery = Builders<TUser>.Filter.ElemMatch(usr => usr.Logins,
                Builders<UserLogin>.Filter.And(
                    Builders<UserLogin>.Filter.Eq(lg => lg.LoginProvider, loginProvider),
                    Builders<UserLogin>.Filter.Eq(lg => lg.ProviderKey, providerKey)
                )
            );

            var query = Builders<TUser>.Filter.And(notDeletedQuery, loginQuery);

            return Context.Users.Find(query).FirstOrDefaultAsync(cancellationToken);
        }

        #endregion

        #endregion

        #region IUserRoleStore Section
        #region Implementation of IUserRoleStore<TUser>

        /// <summary>
        /// Add a the specified <paramref name="user" /> to the named role.
        /// </summary>
        /// <param name="user">The user to add to the named role.</param>
        /// <param name="roleName">The name of the role to add the user to.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");
            roleName.ThrowIfStringEmpty($"{nameof(roleName)} is required.");

            // find in role collection
            var role = await Context.Roles
                .Find(a => string.Equals(a.Name, roleName, StringComparison.CurrentCultureIgnoreCase))
                .FirstOrDefaultAsync(cancellationToken);

            role.ThrowIfNull($"{nameof(role)} is not found.");

            user.Roles.Add(role);

            // throw new NotImplementedException();
        }

        #endregion

        #region Implementation of IUserRoleStore<TUser>

        /// <summary>
        /// Add a the specified <paramref name="user" /> from the named role.
        /// </summary>
        /// <param name="user">The user to remove the named role from.</param>
        /// <param name="roleName">The name of the role to remove.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");
            roleName.ThrowIfStringEmpty($"{nameof(roleName)} is required.");

            var rolesToDelete =
                user.Roles.Where(role => string.Equals(role.Name, roleName, StringComparison.CurrentCultureIgnoreCase));

            foreach (var identityRole in rolesToDelete)
            {
                user.Roles.Remove(identityRole);
            }

            return TaskCache.CompletedTask;
        }

        #endregion

        #region GetRolesAsync of IUserRoleStore<TUser>

        /// <summary>
        /// Gets a list of role names the specified <paramref name="user" /> belongs to.
        /// </summary>
        /// <param name="user">The user whose role names to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list of role names.</returns>
        public virtual Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");

            return Task.FromResult((IList<string>)user.Roles.Select(role => role.Name).ToList());
        }

        #endregion

        #region IsInRoleAsync of IUserRoleStore<TUser>

        /// <summary>
        /// Returns a flag indicating whether the specified <paramref name="user" /> is a member of the give named role.
        /// </summary>
        /// <param name="user">The user whose role membership should be checked.</param>
        /// <param name="roleName">The name of the role to be checked.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a flag indicating whether the specified <paramref name="user" /> is
        /// a member of the named role.
        /// </returns>
        public virtual Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");
            roleName.ThrowIfStringEmpty($"{nameof(roleName)} is required.");

            var isIn = user.Roles.Any(r => string.Equals(r.Name, roleName, StringComparison.CurrentCultureIgnoreCase));

            return Task.FromResult(isIn);
        }

        #endregion

        #region GetUsersInRoleAsync of IUserRoleStore<TUser>

        /// <summary>
        /// Returns a list of Users who are members of the named role.
        /// </summary>
        /// <param name="roleName">The name of the role whose membership should be returned.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list of users who are in the named role.
        /// </returns>
        public virtual async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            roleName.ThrowIfStringEmpty($"{nameof(roleName)} is required.");

            var users = await Context.Users
                .Find(u => u.Roles
                    .Any(r => r.NormalizedName == roleName))
                .ToListAsync(cancellationToken);

            return users;
        }

        #endregion

        #endregion

        #region IUserClaimStore section

        #region GetClaimsAsync of IUserClaimStore<TUser>

        /// <summary>
        /// Gets a list of <see cref="T:System.Security.Claims.Claim" />s to be belonging to the specified <paramref name="user" /> as an asynchronous operation.
        /// </summary>
        /// <param name="user">The role whose claims to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the result of the asynchronous query, a list of <see cref="T:System.Security.Claims.Claim" />s.
        /// </returns>
        public virtual Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            // return 
            throw new NotImplementedException();
        }

        #endregion

        #region AddClaimsAsync of IUserClaimStore<TUser>

        /// <summary>Add claims to a user as an asynchronous operation.</summary>
        /// <param name="user">The user to add the claim to.</param>
        /// <param name="claims">The collection of <see cref="T:System.Security.Claims.Claim" />s to add.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ReplaceClaimAsync of IUserClaimStore<TUser>

        /// <summary>
        /// Replaces the given <paramref name="claim" /> on the specified <paramref name="user" /> with the <paramref name="newClaim" />
        /// </summary>
        /// <param name="user">The user to replace the claim on.</param>
        /// <param name="claim">The claim to replace.</param>
        /// <param name="newClaim">The new claim to replace the existing <paramref name="claim" /> with.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region RemoveClaimsAsync of IUserClaimStore<TUser>

        /// <summary>
        /// Removes the specified <paramref name="claims" /> from the given <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user to remove the specified <paramref name="claims" /> from.</param>
        /// <param name="claims">A collection of <see cref="T:System.Security.Claims.Claim" />s to remove.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GetUsersForClaimAsync of IUserClaimStore<TUser>

        /// <summary>
        /// Returns a list of users who contain the specified <see cref="T:System.Security.Claims.Claim" />.
        /// </summary>
        /// <param name="claim">The claim to look for.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the result of the asynchronous query, a list of <typeparamref name="TUser" /> who
        /// contain the specified claim.
        /// </returns>
        public virtual Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region IUserPasswordStore & IUserSecurityStampStore

        #region Implementation of IUserPasswordStore<TUser>

        /// <summary>
        /// Sets the password hash for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose password hash to set.</param>
        /// <param name="passwordHash">The password hash to set.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull();
            passwordHash.ThrowIfStringEmpty($"{nameof(passwordHash)} is required.");

            user.PasswordHash = passwordHash;

            return TaskCache.CompletedTask;
        }

        #endregion

        #region Implementation of IUserPasswordStore<TUser>

        /// <summary>
        /// Gets the password hash for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose password hash to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, returning the password hash for the specified <paramref name="user" />.</returns>
        public virtual Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");

            return Task.FromResult(user.PasswordHash);
        }

        #endregion

        #region Implementation of IUserPasswordStore<TUser>

        /// <summary>
        /// Gets a flag indicating whether the specified <paramref name="user" /> has a password.
        /// </summary>
        /// <param name="user">The user to return a flag for, indicating whether they have a password or not.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, returning true if the specified <paramref name="user" /> has a password
        /// otherwise false.
        /// </returns>
        public virtual Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");

            return Task.FromResult(string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        #endregion

        #region Implementation of IUserSecurityStampStore<TUser>

        /// <summary>
        /// Sets the provided security <paramref name="stamp" /> for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose security stamp should be set.</param>
        /// <param name="stamp">The security stamp to set.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");
            stamp.ThrowIfStringEmpty($"{nameof(stamp)} is required.");

            user.SecurityStamp = stamp;

            return TaskCache.CompletedTask;
        }

        #endregion

        #region Implementation of IUserSecurityStampStore<TUser>

        /// <summary>
        /// Get the security stamp for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose security stamp should be set.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the security stamp for the specified <paramref name="user" />.</returns>
        public virtual Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");

            return Task.FromResult(user.SecurityStamp);
        }

        #endregion

        #endregion

        #region IUserEmailStore Section

        #region SetEmailAsync of IUserEmailStore<TUser>

        /// <summary>
        /// Sets the <paramref name="email" /> address for a <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose email should be set.</param>
        /// <param name="email">The email to set.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");
            email.ThrowIfStringEmpty($"{nameof(email)} is required.");

            user.Email = new UserEmail(email);

            return TaskCache.CompletedTask;
        }

        #endregion

        #region GetEmailAsync of IUserEmailStore<TUser>

        /// <summary>
        /// Gets the email address for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose email should be returned.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object containing the results of the asynchronous operation, the email address for the specified <paramref name="user" />.</returns>
        public virtual Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");

            return Task.FromResult(user.Email.Value);
        }

        #endregion

        #region GetEmailConfirmedAsync of IUserEmailStore<TUser>

        /// <summary>
        /// Gets a flag indicating whether the email address for the specified <paramref name="user" /> has been verified, true if the email address is verified otherwise
        /// false.
        /// </summary>
        /// <param name="user">The user whose email confirmation status should be returned.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous operation, a flag indicating whether the email address for the specified <paramref name="user" />
        /// has been confirmed or not.
        /// </returns>
        public virtual Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");

            return Task.FromResult(user.Email.IsComfirmed);

        }

        #endregion

        #region SetEmailConfirmedAsync of IUserEmailStore<TUser>

        /// <summary>
        /// Sets the flag indicating whether the specified <paramref name="user" />'s email address has been confirmed or not.
        /// </summary>
        /// <param name="user">The user whose email confirmation status should be set.</param>
        /// <param name="confirmed">A flag indicating if the email address has been confirmed, true if the address is confirmed otherwise false.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");

            user.Email.ConfirmedOn = confirmed ? DateTime.UtcNow : default(DateTime?);

            return TaskCache.CompletedTask;
        }

        #endregion

        #region FindByEmailAsync of IUserEmailStore<TUser>

        /// <summary>
        /// Gets the user, if any, associated with the specified, normalized email address.
        /// </summary>
        /// <param name="normalizedEmail">The normalized email address to return the user for.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous lookup operation, the user if any associated with the specified normalized email address.
        /// </returns>
        public virtual Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            normalizedEmail.ThrowIfStringEmpty($"{nameof(normalizedEmail)} is required.");

            return Context.Users.Find(x => x.Email.NormalizedValue == normalizedEmail).FirstOrDefaultAsync(cancellationToken);
        }

        #endregion

        #region GetNormalizedEmailAsync of IUserEmailStore<TUser>

        /// <summary>
        /// Returns the normalized email for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose email address to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous lookup operation, the normalized email address if any associated with the specified user.
        /// </returns>
        public virtual Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");

            return Task.FromResult(user.Email.NormalizedValue);
        }

        #endregion

        #region SetNormalizedEmailAsync of IUserEmailStore<TUser>

        /// <summary>
        /// Sets the normalized email for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose email address to set.</param>
        /// <param name="normalizedEmail">The normalized email to set for the specified <paramref name="user" />.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual Task SetNormalizedEmailAsync(TUser user, string normalizedEmail,
            CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");
            normalizedEmail.ThrowIfNull($"{nameof(normalizedEmail)} is required.");

            user.Email.NormalizedValue = normalizedEmail;

            return TaskCache.CompletedTask;
        }

        #endregion

        #endregion

        #region Lockout Section

        #region GetLockoutEndDateAsync of IUserLockoutStore<TUser>

        /// <summary>
        /// Gets the last <see cref="T:System.DateTimeOffset" /> a user's last lockout expired, if any.
        /// Any time in the past should be indicates a user is not locked out.
        /// </summary>
        /// <param name="user">The user whose lockout date should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the result of the asynchronous query, a <see cref="T:System.DateTimeOffset" /> containing the last time
        /// a user's lockout expired, if any.
        /// </returns>
        public virtual Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");

            return Task.FromResult(user.LockoutEnd);
        }

        #endregion

        #region SetLockoutEndDateAsync of IUserLockoutStore<TUser>

        /// <summary>
        /// Locks out a user until the specified end date has passed. Setting a end date in the past immediately unlocks a user.
        /// </summary>
        /// <param name="user">The user whose lockout date should be set.</param>
        /// <param name="lockoutEnd">The <see cref="T:System.DateTimeOffset" /> after which the <paramref name="user" />'s lockout should end.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd,
            CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");

            user.LockoutEnd = lockoutEnd;

            return TaskCache.CompletedTask;
        }

        #endregion

        #region IncrementAccessFailedCountAsync of IUserLockoutStore<TUser>

        /// <summary>
        /// Records that a failed access has occurred, incrementing the failed access count.
        /// </summary>
        /// <param name="user">The user whose cancellation count should be incremented.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the incremented failed access count.</returns>
        public virtual Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");

            user.AccessFailedCount++;

            return Task.FromResult(user.AccessFailedCount);
        }

        #endregion

        #region ResetAccessFailedCountAsync of IUserLockoutStore<TUser>

        /// <summary>Resets a user's failed access count.</summary>
        /// <param name="user">The user whose failed access count should be reset.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        /// <remarks>This is typically called after the account is successfully accessed.</remarks>
        public virtual Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");

            user.AccessFailedCount = 0;

            return Task.FromResult(user.AccessFailedCount);
        }

        #endregion

        #region GetAccessFailedCountAsync of IUserLockoutStore<TUser>

        /// <summary>
        /// Retrieves the current failed access count for the specified <paramref name="user" />..
        /// </summary>
        /// <param name="user">The user whose failed access count should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the failed access count.</returns>
        public virtual Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");

            return Task.FromResult(user.AccessFailedCount);
        }

        #endregion

        #region GetLockoutEnabledAsync of IUserLockoutStore<TUser>

        /// <summary>
        /// Retrieves a flag indicating whether user lockout can enabled for the specified user.
        /// </summary>
        /// <param name="user">The user whose ability to be locked out should be returned.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, true if a user can be locked out, otherwise false.
        /// </returns>
        public virtual Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");

            return Task.FromResult(user.LockoutEnabled);
        }

        #endregion

        #region SetLockoutEnabledAsync of IUserLockoutStore<TUser>

        /// <summary>
        /// Set the flag indicating if the specified <paramref name="user" /> can be locked out..
        /// </summary>
        /// <param name="user">The user whose ability to be locked out should be set.</param>
        /// <param name="enabled">A flag indicating if lock out can be enabled for the specified <paramref name="user" />.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");

            user.LockoutEnabled = enabled;
            return TaskCache.CompletedTask;
        }

        #endregion

        #endregion

        #region IUserPhoneNumber

        #region Implementation of IUserPhoneNumberStore<TUser>

        /// <summary>
        /// Sets the telephone number for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose telephone number should be set.</param>
        /// <param name="phoneNumber">The telephone number to set.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");

            user.PhoneNumber = new UserMobile(phoneNumber);

            return TaskCache.CompletedTask;
        }

        #endregion

        #region GetPhoneNumberAsync of IUserPhoneNumberStore<TUser>

        /// <summary>
        /// Gets the telephone number, if any, for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose telephone number should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the user's telephone number, if any.</returns>
        public virtual Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");

            return Task.FromResult(user.PhoneNumber.Value);
        }

        #endregion

        #region GetPhoneNumberConfirmedAsync of IUserPhoneNumberStore<TUser>

        /// <summary>
        /// Gets a flag indicating whether the specified <paramref name="user" />'s telephone number has been confirmed.
        /// </summary>
        /// <param name="user">The user to return a flag for, indicating whether their telephone number is confirmed.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, returning true if the specified <paramref name="user" /> has a confirmed
        /// telephone number otherwise false.
        /// </returns>
        public virtual Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");

            return Task.FromResult(user.PhoneNumber.IsComfirmed);
        }

        #endregion

        #region Implementation of IUserPhoneNumberStore<TUser>

        /// <summary>
        /// Sets a flag indicating if the specified <paramref name="user" />'s phone number has been confirmed..
        /// </summary>
        /// <param name="user">The user whose telephone number confirmation status should be set.</param>
        /// <param name="confirmed">A flag indicating whether the user's telephone number has been confirmed.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");

            user.PhoneNumber.ConfirmedOn = DateTime.UtcNow;

            return TaskCache.CompletedTask;
        }

        #endregion

        #endregion

        #region Users of IQueryableUserStore<TUser>

        /// <summary>
        /// Returns an <see cref="T:System.Linq.IQueryable`1" /> collection of users.
        /// </summary>
        /// <value>An <see cref="T:System.Linq.IQueryable`1" /> collection of users.</value>
        public IQueryable<TUser> Users => Context.Users.AsQueryable();

        #endregion

        #region SetTwoFactorEnabledAsync of IUserTwoFactorStore<TUser>

        /// <summary>
        /// Sets a flag indicating whether the specified <paramref name="user" /> has two factor authentication enabled or not,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose two factor authentication enabled status should be set.</param>
        /// <param name="enabled">A flag indicating whether the specified <paramref name="user" /> has two factor authentication enabled.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");

            user.TwoFactorEnabled = enabled;

            return TaskCache.CompletedTask;
        }

        #endregion

        #region GetTwoFactorEnabledAsync of IUserTwoFactorStore<TUser>

        /// <summary>
        /// Returns a flag indicating whether the specified <paramref name="user" /> has two factor authentication enabled or not,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose two factor authentication enabled status should be set.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a flag indicating whether the specified
        /// <paramref name="user" /> has two factor authentication enabled or not.
        /// </returns>
        public virtual Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");

            return Task.FromResult(user.TwoFactorEnabled);
        }

        #endregion

        #region SetTokenAsync of IUserAuthenticationTokenStore<TUser>

        /// <summary>Sets the token value for a particular user.</summary>
        /// <param name="user">The user.</param>
        /// <param name="loginProvider">The authentication provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="value">The value of the token.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual Task SetTokenAsync(TUser user, string loginProvider, string name, string value,
            CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");

            throw new NotImplementedException();
            // return TaskCache.CompletedTask;
        }

        #endregion

        #region Implementation of IUserAuthenticationTokenStore<TUser>

        /// <summary>Deletes a token for a user.</summary>
        /// <param name="user">The user.</param>
        /// <param name="loginProvider">The authentication provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        public virtual Task RemoveTokenAsync(TUser user, string loginProvider, string name,
            CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");

            throw new NotImplementedException();

           //  return TaskCache.CompletedTask;
        }

        #endregion

        #region Implementation of IUserAuthenticationTokenStore<TUser>

        /// <summary>Returns the token value.</summary>
        /// <param name="user">The user.</param>
        /// <param name="loginProvider">The authentication provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual Task<string> GetTokenAsync(TUser user, string loginProvider, string name,
            CancellationToken cancellationToken)
        {
            ThrowIfCancelOrDispose(cancellationToken);
            user.ThrowIfNull($"{nameof(user)} is required.");
            loginProvider.ThrowIfStringEmpty($"{nameof(loginProvider)} is required.");
            name.ThrowIfStringEmpty($"{nameof(name)} token is required.");

            throw new NotImplementedException();

            throw new NotImplementedException();
        }

        #endregion

        #region Helper Method

        /// <summary>
        /// Converts the provided <paramref name="id"/> to a strongly typed key object.
        /// </summary>
        /// <param name="id">The id to convert.</param>
        /// <returns>An instance of <typeparamref name="TKey"/> representing the provided <paramref name="id"/>.</returns>
        public virtual string ConvertIdFromString(string id)
        {
            return id;
        }

        /// <summary>
        /// Converts the provided <paramref name="id"/> to its string representation.
        /// </summary>
        /// <param name="id">The id to convert.</param>
        /// <returns>An <see cref="string"/> representation of the provided <paramref name="id"/>.</returns>
        public virtual string ConvertIdToString(string id)
        {
            return id;
        }

        #endregion

        #region Dispose

        protected void ThrowIfCancelOrDispose(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
        }

        /// <summary>
        /// Throws if this class has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Dispose the stores
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
        } 
        #endregion
    }
}