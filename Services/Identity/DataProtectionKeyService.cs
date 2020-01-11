using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using Data.Contracts;
using DNTCommon.Web.Core;
using Entities.Identity;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.Logging;

namespace Services.Identity
{
    public class DataProtectionKeyService : IXmlRepository
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DataProtectionKeyService> _logger;

        public DataProtectionKeyService(IServiceProvider serviceProvider, ILogger<DataProtectionKeyService> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            return _serviceProvider.RunScopedService<ReadOnlyCollection<XElement>, IRepository<AppDataProtectionKey>>(context =>
              {
                  var dataProtectionKeys = context.TableNoTracking;
                  var logger = _logger;
                  return dataProtectionKeys.Select(key => TryParseKeyXml(key.XmlData, logger)).ToList().AsReadOnly();
              });
        }

        private static XElement TryParseKeyXml(string xml, ILogger logger)
        {
            try
            {
                return XElement.Parse(xml);
            }
            catch (Exception e)
            {
                logger.LogWarning($"An exception occurred while parsing the key xml '{xml}'.", e);
                return null;
            }
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            // We need a separate context to call its SaveChanges several times,
            // without using the current request's context and changing its internal state.
            _serviceProvider.RunScopedService<IRepository<AppDataProtectionKey>>(context =>
            {
                var dataProtectionKeys = context;
                var entity = dataProtectionKeys.Table.SingleOrDefault(k => k.FriendlyName == friendlyName);
                if (entity != null)
                {
                    entity.XmlData = element.ToString();
                    dataProtectionKeys.Update(entity);
                }
                else
                {
                    dataProtectionKeys.Add(new AppDataProtectionKey
                    {
                        FriendlyName = friendlyName,
                        XmlData = element.ToString(SaveOptions.DisableFormatting)
                    });
                }
                //context.SaveChanges();
            });
        }
    }
}