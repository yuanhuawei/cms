﻿using System;
using System.Web.Http;
using MaiDar.Core;
using MaiDarServer.API.Model;
using MaiDarServer.CMS.Controllers.Sys.Stl.Comments;
using MaiDarServer.CMS.Core;
using MaiDarServer.CMS.Model;
using MaiDarServer.Plugin.Models;

namespace MaiDarServer.API.Controllers.Sys.Stl.Comments
{
    [RoutePrefix("api")]
    public class CommentsActionsAddController : ApiController
    {
        [HttpPost, Route(ActionsAdd.Route)]
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

                var account = body.GetPostString("account");
                var password = body.GetPostString("password");
                var replyId = body.GetPostInt("replyId");
                var content = body.GetPostString("content");

                if (replyId > 0)
                {
                    string replyUserName;
                    string replyContent;
                    DataProvider.CommentDao.GetUserNameAndContent(replyId, out replyUserName, out replyContent);
                    if (!string.IsNullOrEmpty(replyContent))
                    {
                        var displayName = MaiDarDataProvider.UserDao.GetDisplayName(replyUserName);
                        if (!string.IsNullOrEmpty(displayName))
                        {
                            displayName = $"@{displayName}：";
                        }

                        content += $" //{displayName}{replyContent}";
                    }
                }

                IUserInfo userInfo;
                if (!string.IsNullOrEmpty(account) && !string.IsNullOrEmpty(password))
                {
                    string userName;
                    string errorMessage;
                    if (!MaiDarDataProvider.UserDao.Validate(account, password, out userName, out errorMessage))
                    {
                        MaiDarDataProvider.UserDao.UpdateLastActivityDateAndCountOfFailedLogin(userName);
                        return BadRequest(errorMessage);
                    }

                    MaiDarDataProvider.UserDao.UpdateLastActivityDateAndCountOfLogin(userName);
                    userInfo = MaiDarDataProvider.UserDao.GetUserInfoByUserName(userName);

                    body.UserLogin(userName);
                }
                else
                {
                    userInfo = body.UserInfo;
                }

                if (!publishmentSystemInfo.Additional.IsAnonymousComments && !body.IsUserLoggin)
                {
                    return Unauthorized();
                }

                var commentInfo = new CommentInfo
                {
                    Id = 0,
                    PublishmentSystemId = siteId,
                    NodeId = channelId,
                    ContentId = contentId,
                    GoodCount = 0,
                    UserName = userInfo.UserName,
                    IsChecked = !publishmentSystemInfo.Additional.IsCheckComments,
                    AddDate = DateTime.Now,
                    Content = content
                };
                commentInfo.Id = DataProvider.CommentDao.Insert(commentInfo);

                return Ok(new
                {
                    Comment = new Comment(commentInfo, userInfo)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
