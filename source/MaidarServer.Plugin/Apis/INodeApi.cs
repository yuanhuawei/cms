using MaiDarServer.Plugin.Models;
using System.Collections.Generic;

namespace MaiDarServer.Plugin.Apis
{
    public interface INodeApi
    {
        INodeInfo GetNodeInfo(int publishmentSystemId, int channelId);

        List<INodeInfo> GetNodeInfoList(int publishmentSystemId, string adminName);
    }
}
