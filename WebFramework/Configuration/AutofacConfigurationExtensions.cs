using System.Security.Principal;
using Data;
using Common;
using Autofac;
using Services.Services;
using Data.Repositories;
using Data.Contracts;
using Entities.Common;
using Entities.Identity.Settings;
using Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Services.Contracts.Identity;
using Services.Identity;

namespace WebFramework.Configuration
{
    public class AutofacConfigurationExtensions : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();

            var commonAssembly = typeof(SiteSettings).Assembly;
            var commonIdentityAssembly = typeof(IdentitySiteSettings).Assembly;
            var entitiesAssembly = typeof(IEntity).Assembly;
            var dataAssembly = typeof(ApplicationDbContext).Assembly;
            var servicesAssembly = typeof(JwtService).Assembly;

            containerBuilder.RegisterType<HttpContextAccessor>()
                .As<IHttpContextAccessor>()
                .InstancePerLifetimeScope();

            // containerBuilder.RegisterType<IHttpContextAccessor>()
            //     .As<IPrincipal>()
            //     .SingleInstance();

            containerBuilder.RegisterType<CustomNormalizer>()
                .As<ILookupNormalizer>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<HttpContextAccessor>()
                .As<IHttpContextAccessor>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<CustomSecurityStampValidator>()
                .As<ISecurityStampValidator>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<CustomSecurityStampValidator>()
                .As<SecurityStampValidator<User>>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<CustomPasswordValidator>()
                .As<IPasswordValidator<User>>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<CustomPasswordValidator>()
                .As<PasswordValidator<User>>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<CustomUserValidator>()
                .As<IUserValidator<User>>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<CustomUserValidator>()
                .As<UserValidator<User>>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<ApplicationClaimsPrincipalFactory>()
                .As<IUserClaimsPrincipalFactory<User>>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<ApplicationClaimsPrincipalFactory>()
                .As<UserClaimsPrincipalFactory<User, Role>>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<CustomIdentityErrorDescriber>()
                .As<IdentityErrorDescriber>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<ApplicationUserStore>()
                .As<IApplicationUserStore>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<ApplicationUserStore>()
                .As<UserStore<User, Role, ApplicationDbContext, int, UserClaim, UserRole, UserLogin, UserToken, RoleClaim>>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<ApplicationUserManager>()
                .As<IApplicationUserManager>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<ApplicationUserManager>()
                .As<UserManager<User>>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<ApplicationRoleManager>()
                .As<IApplicationRoleManager>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<ApplicationRoleManager>()
                .As<RoleManager<Role>>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<ApplicationSignInManager>()
                .As<IApplicationSignInManager>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<ApplicationSignInManager>()
                .As<SignInManager<User>>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<ApplicationRoleStore>()
                .As<IApplicationRoleStore>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<ApplicationRoleStore>()
                .As<RoleStore<Role, ApplicationDbContext, int, UserRole, RoleClaim>>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<AuthMessageSender>()
                .As<IEmailSender>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<AuthMessageSender>()
                .As<ISmsSender>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<IdentityDbInitializer>()
                .As<IIdentityDbInitializer>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<UsedPasswordsService>()
                .As<IUsedPasswordsService>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<SiteStatService>()
                .As<ISiteStatService>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<UsersPhotoService>()
                .As<IUsersPhotoService>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<SecurityTrimmingService>()
                .As<ISecurityTrimmingService>()
                .InstancePerLifetimeScope();

            // containerBuilder.RegisterType<AppLogItemsService>()
            //     .As<IAppLogItemsService>()
            //     .SingleInstance();

            //************

            containerBuilder.RegisterAssemblyTypes(commonAssembly, commonIdentityAssembly, entitiesAssembly, dataAssembly, servicesAssembly)
                .AssignableTo<IScopedDependency>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterAssemblyTypes(commonAssembly, commonIdentityAssembly, entitiesAssembly, dataAssembly, servicesAssembly)
                .AssignableTo<ITransientDependency>()
                .AsImplementedInterfaces()
                .InstancePerDependency();

            containerBuilder.RegisterAssemblyTypes(commonAssembly, commonIdentityAssembly, entitiesAssembly, dataAssembly, servicesAssembly)
                .AssignableTo<ISingletonDependency>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
