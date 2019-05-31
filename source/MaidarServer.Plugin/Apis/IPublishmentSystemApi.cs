using System.Collections.Generic;
using MaiDarServer.Plugin.Models;

namespace MaiDarServer.Plugin.Apis
{
    public interface IPublishmentSystemApi
    {
        int GetPublishmentSystemIdByFilePath(string path);

        string GetPublishmentSystemPath(int publishmentSystemId);

        List<int> GetPublishmentSystemIds();

        IPublishmentSystemInfo GetPublishmentSystemInfo(int publishmentSystemId);

        List<IPublishmentSystemInfo> GetPublishmentSystemInfoList(string adminName);
    }
}
