﻿using System;
using System.Linq;
using System.Web.Http;
using MaiDar.Core;
using MaiDarServer.API.Model;
using MaiDarServer.CMS.Controllers.Sys.Stl.Comments;
using MaiDarServer.CMS.Core;

namespace MaiDarServer.API.Controllers.Sys.Stl.Comments
{
    [RoutePrefix("api")]
    public class CommentsGetController : ApiController
    {
        [HttpGet, Route(Get.Route)]
        public IHttpActionResult Main(int siteId, int channelId, int contentId)
        {
            try
            {
                var body = new RequestBody();

                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(siteId);
                if (!publishmentSystemInfo.Additional.IsCommentable)
                {
                    return Ok(new
                    {
                        IsCommentable = false
                    });
                }

                var requestCount = body.GetQueryInt("requestCount", 20);
                var requestOffset = body.GetQueryInt("requestOffset");

                var totalCount = DataProvider.CommentDao.GetTotalCountWithChecked(siteId, channelId, contentId);
                var comments =
                    DataProvider.CommentDao.GetCommentInfoListChecked(siteId, channelId, contentId, requestCount,
                            requestOffset)
                        .Select(
                            commentInfo =>
                                new Comment(commentInfo,
                                    MaiDarDataProvider.UserDao.GetUserInfoByUserName(commentInfo.UserName)))
                        .ToList();

                return Ok(new
                {
                    IsCommentable = true,
                    User = body.UserInfo,
                    Comments = comments,
                    TotalCount = totalCount
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
