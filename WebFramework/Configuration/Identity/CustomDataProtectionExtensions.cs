using System.IO;
using System.Security.Cryptography.X509Certificates;
using Common.WebToolkit;
using Entities.Identity.Settings;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Services.Identity;

namespace WebFramework.Configuration.Identity
{
    public static class CustomDataProtectionExtensions
    {
        public static IServiceCollection AddCustomDataProtection(
            this IServiceCollection services, SiteSettings siteSettings)
        {
            services.AddSingleton<IXmlRepository, DataProtectionKeyService>();
            services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(serviceProvider =>
            {
                return new ConfigureOptions<KeyManagementOptions>(options =>
                {
                    var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
                    using var scope = scopeFactory.CreateScope();
                    options.XmlRepository = scope.ServiceProvider.GetRequiredService<IXmlRepository>();
                });
            });

            var certificate = LoadCertificateFromFile(siteSettings);
            services
                .AddDataProtection()
                .SetDefaultKeyLifetime(siteSettings.DataProtectionOptions.DataProtectionKeyLifetime)
                .SetApplicationName(siteSettings.DataProtectionOptions.ApplicationName)
                .ProtectKeysWithCertificate(certificate);

            return services;
        }

        private static X509Certificate2 LoadCertificateFromFile(SiteSettings siteSettings)
        {
            // NOTE:
            // You should check out the identity of your application pool and make sure
            // that the `Load user profile` option is turned on, otherwise the crypto subsystem won't work.

            var certificate = siteSettings.DataProtectionX509Certificate;
            var fileName = Path.Combine(ServerInfo.GetAppDataFolderPath(), certificate.FileName);

            // For decryption the certificate must be in the certificate store. It's a limitation of how EncryptedXml works.
            using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadWrite);
                store.Add(new X509Certificate2(fileName, certificate.Password, X509KeyStorageFlags.Exportable));
            }

            return new X509Certificate2(
                fileName,
                certificate.Password,
                keyStorageFlags: X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet
                                 | X509KeyStorageFlags.Exportable);
        }
    }
}