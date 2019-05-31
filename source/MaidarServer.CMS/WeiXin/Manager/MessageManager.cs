using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using MaiDar.Core;
using MaiDarServer.CMS.Core;
using MaiDarServer.CMS.Model;
using MaiDarServer.CMS.WeiXin.Data;
using MaiDarServer.CMS.WeiXin.Model;
using MaiDarServer.CMS.WeiXin.Model.Enumerations;
using MaiDarServer.CMS.WeiXin.WeiXinMP.Entities.Response;

namespace MaiDarServer.CMS.WeiXin.Manager
{
	public class MessageManager
    {
        public static string GetImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, "weixin/message/img/start.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
            }
        }

        public static string GetContentImageUrl(PublishmentSystemInfo publishmentSystemInfo, string contentImageUrl)
        {
            if (string.IsNullOrEmpty(contentImageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, "weixin/message/img/top.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, contentImageUrl));
            }
        }

        private static string GetMessageUrl(PublishmentSystemInfo publishmentSystemInfo)
        {
            return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, "weixin/message/index.html"));
        }

        public static string GetMessageUrl(PublishmentSystemInfo publishmentSystemInfo, MessageInfo messageInfo, string wxOpenID)
        {
            var attributes = new NameValueCollection();
            attributes.Add("publishmentSystemID", messageInfo.PublishmentSystemId.ToString());
            attributes.Add("messageID", messageInfo.Id.ToString());
            attributes.Add("wxOpenID", wxOpenID);
            return PageUtils.AddQueryString(GetMessageUrl(publishmentSystemInfo), attributes);
        }

        public static List<Article> Trigger(PublishmentSystemInfo publishmentSystemInfo, Model.KeywordInfo keywordInfo, string wxOpenID)
        {
            var articleList = new List<Article>();

            DataProviderWx.CountDao.AddCount(keywordInfo.PublishmentSystemId, ECountType.RequestNews);

            var messageInfoList = DataProviderWx.MessageDao.GetMessageInfoListByKeywordId(keywordInfo.PublishmentSystemId, keywordInfo.KeywordId);

            foreach (var messageInfo in messageInfoList)
            {
                Article article = null;

                if (messageInfo != null && !messageInfo.IsDisabled)
                {
                    var imageUrl = GetImageUrl(publishmentSystemInfo, messageInfo.ImageUrl);
                    var pageUrl = GetMessageUrl(publishmentSystemInfo, messageInfo, wxOpenID);

                    article = new Article()
                    {
                        Title = messageInfo.Title,
                        Description = messageInfo.Summary,
                        PicUrl = imageUrl,
                        Url = pageUrl
                    };
                }

                if (article != null)
                {
                    articleList.Add(article);
                }
            }

            return articleList;
        }

        public static void AddContent(int publishmentSystemID, int messageID, string displayName, string color, string content, string ipAddress, string cookieSN, string wxOpenID, string userName)
        {
            DataProviderWx.MessageDao.AddUserCount(messageID);
            var contentInfo = new MessageContentInfo { Id = 0, PublishmentSystemId = publishmentSystemID, MessageId = messageID, IpAddress = ipAddress, CookieSn = cookieSN, WxOpenId = wxOpenID, UserName = userName, ReplyCount = 0, LikeCount = 0, IsReply = false, ReplyId = 0, DisplayName = displayName, Color = color, Content = content, AddDate = DateTime.Now };
            DataProviderWx.MessageContentDao.Insert(contentInfo);
        }

        public static void AddReply(int publishmentSystemID, int messageID, int replyContentID, string displayName, string content, string ipAddress, string cookieSN, string wxOpenID, string userName)
        {
            DataProviderWx.MessageDao.AddUserCount(messageID);
            var contentInfo = new MessageContentInfo { Id = 0, PublishmentSystemId = publishmentSystemID, MessageId = messageID, IpAddress = ipAddress, CookieSn = cookieSN, WxOpenId = wxOpenID, UserName = userName, ReplyCount = 0, LikeCount = 0, IsReply = true, ReplyId = replyContentID, DisplayName = displayName, Color = string.Empty, Content = content, AddDate = DateTime.Now };
            DataProviderWx.MessageContentDao.Insert(contentInfo);
            DataProviderWx.MessageContentDao.AddReplyCount(replyContentID);
        }
	}
}
