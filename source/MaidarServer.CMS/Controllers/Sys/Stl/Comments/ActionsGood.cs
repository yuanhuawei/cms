﻿using MaiDar.Core;

namespace MaiDarServer.CMS.Controllers.Sys.Stl.Comments
{
    public class ActionsGood
    {
        public const string Route = "sys/stl/comments/{siteId}/{channelId}/{contentId}/actions/good";

        public static string GetUrl(string apiUrl, int siteId, int channelId, int contentId)
        {
            return PageUtils.Combine(apiUrl, Route
                .Replace("{siteId}", siteId.ToString())
                .Replace("{channelId}", channelId.ToString())
                .Replace("{contentId}", contentId.ToString()));
        }
    }
}