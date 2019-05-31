﻿using System.Collections.Generic;
using MaiDarServer.CMS.Core;
using MaiDarServer.Plugin.Apis;
using MaiDarServer.Plugin.Models;

namespace MaiDarServer.CMS.Plugin.Apis
{
    public class NodeApi : INodeApi
    {
        private NodeApi() { }

        public static NodeApi Instance { get; } = new NodeApi();

        public INodeInfo GetNodeInfo(int publishmentSystemId, int channelId)
        {
            return NodeManager.GetNodeInfo(publishmentSystemId, channelId);
        }

        public List<INodeInfo> GetNodeInfoList(int publishmentSystemId, string adminName)
        {
            return PublishmentSystemManager.GetWritingNodeInfoList(adminName, publishmentSystemId);
        }
    }
}
