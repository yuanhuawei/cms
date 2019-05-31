﻿using MaiDar.Core;

namespace MaiDarServer.CMS.Controllers.Sys.Stl
{
    public class ActionsGovPublicApplyAdd
    {
        public const string Route = "sys/stl/actions/gov_public_apply_add/{publishmentSystemId}/{styleId}";

        public static string GetUrl(string apiUrl, int publishmentSystemId, int styleId)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{publishmentSystemId}", publishmentSystemId.ToString());
            apiUrl = apiUrl.Replace("{styleId}", styleId.ToString());
            return apiUrl;
        }
    }
}