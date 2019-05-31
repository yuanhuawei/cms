using System;
using MaiDar.Core;

namespace MaiDarServer.CMS.Core
{
    public class UpgradeManager
    {
        public static void Upgrade()
        {
            foreach (var provider in MaiDarDataProvider.AllProviders)
            {
                if (string.IsNullOrEmpty(provider.TableName) || provider.TableColumns == null || provider.TableColumns.Count <= 0) continue;

                if (!MaiDarDataProvider.DatabaseDao.IsTableExists(provider.TableName))
                {
                    MaiDarDataProvider.DatabaseDao.CreateSystemTable(provider.TableName, provider.TableColumns);
                }
                else
                {
                    MaiDarDataProvider.DatabaseDao.AlterSystemTable(provider.TableName, provider.TableColumns);
                }
            }

            var configInfo = MaiDarDataProvider.ConfigDao.GetConfigInfo();
            configInfo.DatabaseVersion = AppManager.Version;
            configInfo.IsInitialized = true;
            configInfo.UpdateDate = DateTime.Now;
            MaiDarDataProvider.ConfigDao.Update(configInfo);
        }

        //public static void Upgrade(string version, out string errorMessage)
        //{
        //    errorMessage = string.Empty;
        //    if (!string.IsNullOrEmpty(version) && MaiDarDataProvider.ConfigDao.GetDatabaseVersion() != version)
        //    {
        //        var errorBuilder = new StringBuilder();
        //        MaiDarDataProvider.DatabaseDao.Upgrade(WebConfigUtils.DatabaseType, errorBuilder);

        //        //升级数据库

        //        errorMessage = $"<!--{errorBuilder}-->";
        //    }

        //    var configInfo = MaiDarDataProvider.ConfigDao.GetConfigInfo();
        //    configInfo.DatabaseVersion = version;
        //    configInfo.IsInitialized = true;
        //    configInfo.UpdateDate = DateTime.Now;
        //    MaiDarDataProvider.ConfigDao.Update(configInfo);
        //}
    }
}
