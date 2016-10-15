using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace MienDev.AspNetCore.Identity.MongoDB
{
    public class RoleStore<TRole> :
    IQueryableRoleStore<TRole>,
    IRoleClaimStore<TRole>
    where TRole : IdentityRole
    {
        #region Private and Contructor
        private bool _disposed;
        private readonly IMongoCollection<TRole> _roles;

        public RoleStore(IMongoCollection<TRole> roles)
        {
            ThrowIfParaNull(roles);
            _roles = roles;
        }
        #endregion

        #region CreateAsync
        /// <summary>
        /// Creates a new role in a store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to create in the store.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrParaNull(cancellationToken, role);

            await _roles.InsertOneAsync(role, null, cancellationToken);

            return IdentityResult.Success;
        }
        #endregion

        #region UpdateAsync
        /// <summary>
        /// Updates a role in a store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to update in the store.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
        public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrParaNull(cancellationToken, role);

            await _roles.ReplaceOneAsync(
                r => r.Id == role.Id,
                role,
                new UpdateOptions() { IsUpsert = false },
                cancellationToken);

            throw new NotImplementedException();
        }
        #endregion

        #region DeleteAsync
        /// <summary>
        /// Deletes a role from the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to delete from the store.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrParaNull(cancellationToken,role);

            await _roles.DeleteOneAsync(r => r.Id == role.Id, cancellationToken);

            return IdentityResult.Success;
        }
        #endregion

        #region GetRoleIdAsync
        /// <summary>
        /// Gets the ID for a role from the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose ID should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the ID of the role.</returns>
        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            ThrowIfCancelOrParaNull(cancellationToken,role);
            // todo: user the name as Id
            return Task.FromResult(role.Id);
        }
        #endregion

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {

            throw new NotImplementedException();
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TRole> Roles { get; }
        public Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }


        #region ThrowIf


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

        private static void ThrowIfParaNull(params object[] para)
        {
            if (para.Length > 0 && para.Any(p => p == null))
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// ThrowIfCancelOrNull
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="paras"></param>
        private void ThrowIfCancelOrParaNull(CancellationToken cancellationToken, params object[] paras)
        {
            ThrowIfDisposed();

            cancellationToken.ThrowIfCancellationRequested();

            ThrowIfParaNull(paras);
        }

        #endregion

    }
}
