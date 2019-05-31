using System.Collections.Generic;
using MaiDarServer.CMS.Core;
using MaiDarServer.Plugin.Apis;
using MaiDarServer.Plugin.Models;

namespace MaiDarServer.CMS.Plugin.Apis
{
    public class PublishmentSystemApi : IPublishmentSystemApi
    {
        private PublishmentSystemApi() { }

        public static PublishmentSystemApi Instance { get; } = new PublishmentSystemApi();

        public int GetPublishmentSystemIdByFilePath(string path)
        {
            var publishmentSystemInfo = PathUtility.GetPublishmentSystemInfo(path);
            return publishmentSystemInfo?.PublishmentSystemId ?? 0;
        }

        public string GetPublishmentSystemPath(int publishmentSystemId)
        {
            if (publishmentSystemId <= 0) return null;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            return publishmentSystemInfo == null ? null : PathUtility.GetPublishmentSystemPath(publishmentSystemInfo);
        }

        public List<int> GetPublishmentSystemIds()
        {
            return PublishmentSystemManager.GetPublishmentSystemIdList();
        }

        public IPublishmentSystemInfo GetPublishmentSystemInfo(int publishmentSystemId)
        {
            return PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
        }

        public List<IPublishmentSystemInfo> GetPublishmentSystemInfoList(string adminName)
        {
            return PublishmentSystemManager.GetWritingPublishmentSystemInfoList(adminName);
        }
    }
}
