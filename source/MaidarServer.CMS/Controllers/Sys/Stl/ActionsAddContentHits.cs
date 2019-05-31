﻿using MaiDar.Core;

namespace MaiDarServer.CMS.Controllers.Sys.Stl
{
    public class ActionsAddContentHits
    {
        public const string Route = "sys/stl/actions/add_content_hits/{publishmentSystemId}/{channelId}/{contentId}";

        public static string GetUrl(string apiUrl, int publishmentSystemId, int channelId, int contentId)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{publishmentSystemId}", publishmentSystemId.ToString());
            apiUrl = apiUrl.Replace("{channelId}", channelId.ToString());
            apiUrl = apiUrl.Replace("{contentId}", contentId.ToString());
            return apiUrl;
        }
    }
}