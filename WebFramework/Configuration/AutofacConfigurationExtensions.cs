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
                .InstancePerRequest();

            // containerBuilder.RegisterType<IHttpContextAccessor>()
            //     .As<IPrincipal>()
            //     .SingleInstance();

            containerBuilder.RegisterType<CustomNormalizer>()
                .As<ILookupNormalizer>()
                .InstancePerRequest();

            containerBuilder.RegisterType<HttpContextAccessor>()
                .As<IHttpContextAccessor>()
                .InstancePerRequest();

            containerBuilder.RegisterType<CustomSecurityStampValidator>()
                .As<ISecurityStampValidator>()
                .InstancePerRequest();

            containerBuilder.RegisterType<CustomSecurityStampValidator>()
                .As<SecurityStampValidator<User>>()
                .InstancePerRequest();

            containerBuilder.RegisterType<CustomPasswordValidator>()
                .As<IPasswordValidator<User>>()
                .InstancePerRequest();

            containerBuilder.RegisterType<CustomPasswordValidator>()
                .As<PasswordValidator<User>>()
                .InstancePerRequest();

            containerBuilder.RegisterType<CustomUserValidator>()
                .As<IUserValidator<User>>()
                .InstancePerRequest();

            containerBuilder.RegisterType<CustomUserValidator>()
                .As<UserValidator<User>>()
                .InstancePerRequest();

            containerBuilder.RegisterType<ApplicationClaimsPrincipalFactory>()
                .As<IUserClaimsPrincipalFactory<User>>()
                .InstancePerRequest();

            containerBuilder.RegisterType<ApplicationClaimsPrincipalFactory>()
                .As<UserClaimsPrincipalFactory<User, Role>>()
                .InstancePerRequest();

            containerBuilder.RegisterType<CustomIdentityErrorDescriber>()
                .As<IdentityErrorDescriber>()
                .InstancePerRequest();

            containerBuilder.RegisterType<ApplicationUserStore>()
                .As<IApplicationUserStore>()
                .InstancePerRequest();

            containerBuilder.RegisterType<ApplicationUserStore>()
                .As<UserStore<User, Role, ApplicationDbContext, int, UserClaim, UserRole, UserLogin, UserToken, RoleClaim>>()
                .InstancePerRequest();

            containerBuilder.RegisterType<ApplicationUserManager>()
                .As<IApplicationUserManager>()
                .InstancePerRequest();

            containerBuilder.RegisterType<ApplicationUserManager>()
                .As<UserManager<User>>()
                .InstancePerRequest();

            containerBuilder.RegisterType<ApplicationRoleManager>()
                .As<IApplicationRoleManager>()
                .InstancePerRequest();

            containerBuilder.RegisterType<ApplicationRoleManager>()
                .As<RoleManager<Role>>()
                .InstancePerRequest();

            containerBuilder.RegisterType<ApplicationSignInManager>()
                .As<IApplicationSignInManager>()
                .InstancePerRequest();

            containerBuilder.RegisterType<ApplicationSignInManager>()
                .As<SignInManager<User>>()
                .InstancePerRequest();

            containerBuilder.RegisterType<ApplicationRoleStore>()
                .As<IApplicationRoleStore>()
                .InstancePerRequest();

            containerBuilder.RegisterType<ApplicationRoleStore>()
                .As<RoleStore<Role, ApplicationDbContext, int, UserRole, RoleClaim>>()
                .InstancePerRequest();

            containerBuilder.RegisterType<AuthMessageSender>()
                .As<IEmailSender>()
                .InstancePerRequest();

            containerBuilder.RegisterType<AuthMessageSender>()
                .As<ISmsSender>()
                .InstancePerRequest();

            containerBuilder.RegisterType<IdentityDbInitializer>()
                .As<IIdentityDbInitializer>()
                .InstancePerRequest();

            containerBuilder.RegisterType<UsedPasswordsService>()
                .As<IUsedPasswordsService>()
                .InstancePerRequest();

            containerBuilder.RegisterType<SiteStatService>()
                .As<ISiteStatService>()
                .InstancePerRequest();

            containerBuilder.RegisterType<UsersPhotoService>()
                .As<IUsersPhotoService>()
                .InstancePerRequest();

            containerBuilder.RegisterType<SecurityTrimmingService>()
                .As<ISecurityTrimmingService>()
                .InstancePerRequest();

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
