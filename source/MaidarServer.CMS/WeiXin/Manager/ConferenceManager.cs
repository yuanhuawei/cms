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
	public class ConferenceManager
    {
        public static string GetImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, "weixin/conference/img/start.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
            }
        }

        public static string GetEndImageUrl(PublishmentSystemInfo publishmentSystemInfo, string endImageUrl)
        {
            if (string.IsNullOrEmpty(endImageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, "weixin/conference/img/end.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, endImageUrl));
            }
        }

        private static string GetConferenceUrl(PublishmentSystemInfo publishmentSystemInfo)
        {
            return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, "weixin/conference/index.html"));
        }

        public static string GetConferenceUrl(PublishmentSystemInfo publishmentSystemInfo, ConferenceInfo conferenceInfo, string wxOpenID)
        {
            var attributes = new NameValueCollection();
            attributes.Add("publishmentSystemID", conferenceInfo.PublishmentSystemId.ToString());
            attributes.Add("conferenceID", conferenceInfo.Id.ToString());
            attributes.Add("wxOpenID", wxOpenID);
            return PageUtils.AddQueryString(GetConferenceUrl(publishmentSystemInfo), attributes);
        }

        public static void AddContent(int publishmentSystemID, int conferenceID, string realName, string mobile, string email, string company, string position, string note, string ipAddress, string cookieSN, string wxOpenID, string userName)
        {
            DataProviderWx.ConferenceDao.AddUserCount(conferenceID);
            var contentInfo = new ConferenceContentInfo { Id = 0, PublishmentSystemId = publishmentSystemID, ConferenceId = conferenceID, IpAddress = ipAddress, CookieSn = cookieSN, WxOpenId = wxOpenID, UserName = userName, RealName = realName, Mobile = mobile, Email = email, Company = company, Position = position, Note = note, AddDate = DateTime.Now };
            DataProviderWx.ConferenceContentDao.Insert(contentInfo);
        }

        public static List<Article> Trigger(PublishmentSystemInfo publishmentSystemInfo, Model.KeywordInfo keywordInfo, string wxOpenID)
        {
            var articleList = new List<Article>();

            DataProviderWx.CountDao.AddCount(keywordInfo.PublishmentSystemId, ECountType.RequestNews);

            var conferenceInfoList = DataProviderWx.ConferenceDao.GetConferenceInfoListByKeywordId(keywordInfo.PublishmentSystemId, keywordInfo.KeywordId);

            foreach (var conferenceInfo in conferenceInfoList)
            {
                Article article = null;

                if (conferenceInfo != null && conferenceInfo.StartDate < DateTime.Now)
                {
                    var isEnd = false;

                    if (conferenceInfo.EndDate < DateTime.Now)
                    {
                        isEnd = true;
                    }

                    if (isEnd)
                    {
                        var endImageUrl = GetEndImageUrl(publishmentSystemInfo, conferenceInfo.EndImageUrl);

                        article = new Article()
                        {
                            Title = conferenceInfo.EndTitle,
                            Description = conferenceInfo.EndSummary,
                            PicUrl = endImageUrl
                        };
                    }
                    else
                    {
                        var imageUrl = GetImageUrl(publishmentSystemInfo, conferenceInfo.ImageUrl);
                        var pageUrl = GetConferenceUrl(publishmentSystemInfo, conferenceInfo, wxOpenID);

                        article = new Article()
                        {
                            Title = conferenceInfo.Title,
                            Description = conferenceInfo.Summary,
                            PicUrl = imageUrl,
                            Url = pageUrl
                        };
                    }
                }

                if (article != null)
                {
                    articleList.Add(article);
                }
            }

            return articleList;
        }
	}
}
