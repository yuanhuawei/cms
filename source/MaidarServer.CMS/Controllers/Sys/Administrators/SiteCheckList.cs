﻿using MaiDar.Core;

namespace MaiDarServer.CMS.Controllers.Sys.Administrators
{
    public class SiteCheckList
    {
        public const string Route = "sys/administrators/{userName}/site_check_list";

        public static string GetUrl(string apiUrl, string userName)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{userName}", userName);
            return apiUrl;
        }
    }
}