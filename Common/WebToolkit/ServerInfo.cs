using System;
using System.IO;
using System.Linq;

namespace Common.WebToolkit
{
    public static class ServerInfo
    {
        public static string GetAppDataFolderPath()
        {
            var appDataFolderPath = Path.Combine(GetWwwRootPath(), "App_Data");
            if (!Directory.Exists(appDataFolderPath))
            {
                Directory.CreateDirectory(appDataFolderPath);
            }
            return appDataFolderPath;
        }

        public static string GetWwwRootPath()
        {
            return Path.Combine(
                AppContext.BaseDirectory.Split(new[] { "bin" }, StringSplitOptions.RemoveEmptyEntries).First(),
                "wwwroot");
        }
    }
}