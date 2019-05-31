using System;
using System.IO;
using MaiDar.Core;
using MaiDarServer.CMS.Core;
using MaiDarServer.Plugin;

namespace MaiDarServer
{
    public class ServiceUtils
    {
        public static bool IsSiteServerDir
        {
            get
            {
                var physicalApplicationPath = Environment.CurrentDirectory;
                var webConfigPath = PathUtils.Combine(physicalApplicationPath, "web.config");
                var binPath = PathUtils.Combine(physicalApplicationPath, "bin/SiteServer.CMS.dll");

                return FileUtils.IsFileExists(webConfigPath) && FileUtils.IsFileExists(binPath);
            }
        }

        public static bool IsInitialized => MaiDarDataProvider.ConfigDao.IsInitialized();
    }
}
