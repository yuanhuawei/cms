﻿using System;
using System.Web.Http;
using MaiDarServer.CMS.Controllers.Sys.Stl.Comments;
using MaiDarServer.CMS.Core;

namespace MaiDarServer.API.Controllers.Sys.Stl.Comments
{
    [RoutePrefix("api")]
    public class CommentsActionsGoodController : ApiController
    {
        [HttpPost, Route(ActionsGood.Route)]
        public IHttpActionResult Main(int siteId, int channelId, int contentId)
        {
            try
            {
                var body = new RequestBody();

                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(siteId);
                if (!publishmentSystemInfo.Additional.IsCommentable)
                {
                    return Unauthorized();
                }

                DataProvider.CommentDao.AddGoodCount(body.GetPostInt("id"));

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
