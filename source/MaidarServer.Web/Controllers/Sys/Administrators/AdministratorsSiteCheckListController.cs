﻿using System.Collections.Generic;
using System.Web.Http;
using MaiDarServer.BackgroundPages.Cms;
using MaiDarServer.CMS.Controllers.Sys.Administrators;
using MaiDarServer.CMS.Core;
using MaiDarServer.CMS.Core.User;

namespace MaiDarServer.API.Controllers.Sys.Administrators
{
    [RoutePrefix("api")]
    public class AdministratorsSiteCheckListController : ApiController
    {
        [HttpGet, Route(SiteCheckList.Route)]
        public IHttpActionResult Main(string userName)
        {
            var body = new RequestBody();

            if (!body.IsAdminLoggin)
            {
                return Unauthorized();
            }

            var list = new List<object>();
            var unCheckedList = CheckManager.GetUserCountListUnChecked(body.AdminName);
            if (unCheckedList.Count <= 0) return Ok(list);

            var dict = new Dictionary<int, int>();

            foreach (var pair in unCheckedList)
            {
                var publishmentSystemId = pair.Key;
                var count = pair.Value;
                if (dict.ContainsKey(publishmentSystemId))
                {
                    dict[publishmentSystemId] = dict[publishmentSystemId] + count;
                }
                else
                {
                    dict[publishmentSystemId] = count;
                }
            }

            foreach (var publishmentSystemId in dict.Keys)
            {
                var count = dict[publishmentSystemId];
                if (!PublishmentSystemManager.IsExists(publishmentSystemId)) continue;

                list.Add(new
                {
                    Url = PageContentCheck.GetRedirectUrl(publishmentSystemId),
                    SiteName = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId).PublishmentSystemName,
                    Count = count
                });
            }

            return Ok(list);
        }
    }
}
