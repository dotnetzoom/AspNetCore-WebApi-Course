using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Entities.Identity;
using Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SortOrder = Microsoft.Data.SqlClient.SortOrder;

namespace Services.Contracts.Identity
{
    public interface IApplicationRoleManager : IDisposable
    {
        #region BaseClass

        /// <summary>
        /// Gets an IQueryable collection of Roles if the persistence store is an <see cref="IQueryableRoleStore{TRole}"/>,
        /// otherwise throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <value>An IQueryable collection of Roles if the persistence store is an <see cref="IQueryableRoleStore{TRole}"/>.</value>
        /// <exception cref="NotSupportedException">Thrown if the persistence store is not an <see cref="IQueryableRoleStore{TRole}"/>.</exception>
        /// <remarks>
        /// Callers to this property should use <see cref="SupportsQueryableRoles"/> to ensure the backing role store supports
        /// returning an IQueryable list of roles.
        /// </remarks>
        IQueryable<Role> Roles { get; }

        /// <summary>
        /// Gets the normalizer to use when normalizing role names to keys.
        /// </summary>
        /// <value>
        /// The normalizer to use when normalizing role names to keys.
        /// </value>
        ILookupNormalizer KeyNormalizer { get; set; }

        /// <summary>
        /// Gets the <see cref="IdentityErrorDescriber"/> used to provider error messages.
        /// </summary>
        /// <value>
        /// The <see cref="IdentityErrorDescriber"/> used to provider error messages.
        /// </value>
        IdentityErrorDescriber ErrorDescriber { get; set; }

        /// <summary>
        /// Gets a list of validators for roles to call before persistence.
        /// </summary>
        /// <value>A list of validators for roles to call before persistence.</value>
        IList<IRoleValidator<Role>> RoleValidators { get; }

        /// <summary>
        /// Gets the <see cref="ILogger"/> used to log messages from the manager.
        /// </summary>
        /// <value>
        /// The <see cref="ILogger"/> used to log messages from the manager.
        /// </value>
        ILogger Logger { get; set; }

        /// <summary>
        /// Gets a flag indicating whether the underlying persistence store supports returning an <see cref="IQueryable"/> collection of roles.
        /// </summary>
        /// <value>
        /// true if the underlying persistence store supports returning an <see cref="IQueryable"/> collection of roles, otherwise false.
        /// </value>
        bool SupportsQueryableRoles { get; }

        /// <summary>
        /// Gets a flag indicating whether the underlying persistence store supports <see cref="Claim"/>s for roles.
        /// </summary>
        /// <value>
        /// true if the underlying persistence store supports <see cref="Claim"/>s for roles, otherwise false.
        /// </value>
        bool SupportsRoleClaims { get; }

        /// <summary>
        /// Adds a claim to a role.
        /// </summary>
        /// <param name="role">The role to add the claim to.</param>
        /// <param name="claim">The claim to add.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        Task<IdentityResult> AddClaimAsync(Role role, Claim claim);

        /// <summary>
        /// Creates the specified <paramref name="role"/> in the persistence store.
        /// </summary>
        /// <param name="role">The role to create.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        Task<IdentityResult> CreateAsync(Role role);

        /// <summary>
        /// Deletes the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role to delete.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> for the delete.
        /// </returns>
        Task<IdentityResult> DeleteAsync(Role role);

        /// <summary>
        /// Finds the role associated with the specified <paramref name="roleId"/> if any.
        /// </summary>
        /// <param name="roleId">The role ID whose role should be returned.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the role
        /// associated with the specified <paramref name="roleId"/>
        /// </returns>
        Task<Role> FindByIdAsync(string roleId);

        /// <summary>
        /// Finds the role associated with the specified <paramref name="roleName"/> if any.
        /// </summary>
        /// <param name="roleName">The name of the role to be returned.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the role
        /// associated with the specified <paramref name="roleName"/>
        /// </returns>
        Task<Role> FindByNameAsync(string roleName);

        /// <summary>
        /// Gets a list of claims associated with the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role whose claims should be returned.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the list of <see cref="Claim"/>s
        /// associated with the specified <paramref name="role"/>.
        /// </returns>
        Task<IList<Claim>> GetClaimsAsync(Role role);

        /// <summary>
        /// Gets a normalized representation of the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The value to normalize.</param>
        /// <returns>A normalized representation of the specified <paramref name="key"/>.</returns>
        string NormalizeKey(string key);

        /// <summary>
        /// Removes a claim from a role.
        /// </summary>
        /// <param name="role">The role to remove the claim from.</param>
        /// <param name="claim">The claim to remove.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        Task<IdentityResult> RemoveClaimAsync(Role role, Claim claim);

        /// <summary>
        /// Gets a flag indicating whether the specified <paramref name="roleName"/> exists.
        /// </summary>
        /// <param name="roleName">The role name whose existence should be checked.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing true if the role name exists, otherwise false.
        /// </returns>
        Task<bool> RoleExistsAsync(string roleName);

        /// <summary>
        /// Updates the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role to updated.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> for the update.
        /// </returns>
        Task<IdentityResult> UpdateAsync(Role role);

        /// <summary>
        /// Updates the normalized name for the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role whose normalized name needs to be updated.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        Task UpdateNormalizedRoleNameAsync(Role role);

        /// <summary>
        /// Gets the name of the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role whose name should be retrieved.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the name of the
        /// specified <paramref name="role"/>.
        /// </returns>
        Task<string> GetRoleNameAsync(Role role);

        /// <summary>
        /// Sets the name of the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role whose name should be set.</param>
        /// <param name="name">The name to set.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        Task<IdentityResult> SetRoleNameAsync(Role role, string name);

        /// <summary>
        /// Gets the ID of the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role whose ID should be retrieved.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the ID of the
        /// specified <paramref name="role"/>.
        /// </returns>
        Task<string> GetRoleIdAsync(Role role);

        #endregion

        #region CustomMethods

        IList<Role> FindUserRoles(int userId);

        Task<List<Role>> GetAllCustomRolesAsync();

        IList<User> GetApplicationUsersInRole(string roleName);

        IList<Role> GetRolesForCurrentUser();

        IList<Role> GetRolesForUser(int userId);

        IList<UserRole> GetUserRolesInRole(string roleName);

        bool IsCurrentUserInRole(string roleName);

        bool IsUserInRole(int userId, string roleName);

        IList<RoleAndUsersCountViewModel> GetAllCustomRolesAndUsersCountList();

        Task<PagedUsersListViewModel> GetPagedApplicationUsersInRoleListAsync(
                int roleId,
                int pageNumber, int recordsPerPage,
                string sortByField, SortOrder sortOrder,
                bool showAllUsers);

        Task<Role> FindRoleIncludeRoleClaimsAsync(int roleId);

        Task<IdentityResult> AddOrUpdateRoleClaimsAsync(
            int roleId,
            string roleClaimType,
            IList<string> selectedRoleClaimValues);

        #endregion
    }
}