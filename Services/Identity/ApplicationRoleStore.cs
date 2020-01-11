using System;
using System.Security.Claims;
using Data;
using Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Services.Contracts.Identity;

namespace Services.Identity
{
    public class ApplicationRoleStore :
        RoleStore<Role, ApplicationDbContext, int, UserRole, RoleClaim>,
        IApplicationRoleStore
    {
        private readonly ApplicationDbContext _context;
        private readonly IdentityErrorDescriber _describer;

        public ApplicationRoleStore(
            ApplicationDbContext context,
            IdentityErrorDescriber describer)
            : base(context, describer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(_context));
            _describer = describer ?? throw new ArgumentNullException(nameof(_describer));
        }

        #region BaseClass

        protected override RoleClaim CreateRoleClaim(Role role, Claim claim)
        {
            return new RoleClaim
            {
                RoleId = role.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            };
        }

        #endregion

        #region CustomMethods

        #endregion
    }
}