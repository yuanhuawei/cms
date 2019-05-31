using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using MaiDar.Core;
using MaiDarServer.CMS.Core;
using MaiDarServer.CMS.Model;
using MaiDarServer.CMS.WeiXin.Data;
using MaiDarServer.CMS.WeiXin.Model.Enumerations;
using MaiDarServer.CMS.WeiXin.WeiXinMP.Entities.Response;

namespace MaiDarServer.CMS.WeiXin.Manager
{
	public class AppointmentManager
    {
        public static string GetImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, "weixin/appointment/img/start.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
            }
        }

        public static string GetItemTopImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, "weixin/appointment/img/item.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
            }
        }

        public static string GetContentImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, "weixin/appointment/img/top.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
            }
        }

        public static string GetContentResultTopImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, "weixin/appointment/img/result.jpg"));
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
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, "weixin/appointment/img/end.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, endImageUrl));
            }
        }

        public static string GetIndexUrl(PublishmentSystemInfo publishmentSystemInfo, int appointmentID, string wxOpenID)
        {
            var attributes = new NameValueCollection();
            attributes.Add("publishmentSystemID", publishmentSystemInfo.PublishmentSystemId.ToString());
            attributes.Add("appointmentID", appointmentID.ToString());
            attributes.Add("wxOpenID", wxOpenID);
            var url = PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, "weixin/appointment/index.html"));
            return PageUtils.AddQueryString(url, attributes);
        }

        public static string GetItemUrl(PublishmentSystemInfo publishmentSystemInfo, int appointmentID, int itemID, string wxOpenID)
        {
            var attributes = new NameValueCollection();
            attributes.Add("publishmentSystemID", publishmentSystemInfo.PublishmentSystemId.ToString());
            attributes.Add("appointmentID", appointmentID.ToString());
            attributes.Add("itemID", itemID.ToString());
            attributes.Add("wxOpenID", wxOpenID);
            var url = PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, "weixin/appointment/item.html"));
            return PageUtils.AddQueryString(url, attributes);
        }

        public static List<Article> Trigger(Model.KeywordInfo keywordInfo, string wxOpenID)
        {
            var articleList = new List<Article>();

            DataProviderWx.CountDao.AddCount(keywordInfo.PublishmentSystemId, ECountType.RequestNews);

            var appointmentInfoList = DataProviderWx.AppointmentDao.GetAppointmentInfoListByKeywordId(keywordInfo.PublishmentSystemId, keywordInfo.KeywordId);

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(keywordInfo.PublishmentSystemId);

            foreach (var appointmentInfo in appointmentInfoList)
            {
                Article article = null;

                if (appointmentInfo != null && appointmentInfo.StartDate < DateTime.Now)
                {
                    var isEnd = false;

                    if (appointmentInfo.EndDate < DateTime.Now)
                    {
                        isEnd = true;
                    }

                    if (isEnd)
                    {
                        var endImageUrl = GetEndImageUrl(publishmentSystemInfo, appointmentInfo.EndImageUrl);

                        article = new Article()
                        {
                            Title = appointmentInfo.EndTitle,
                            Description = appointmentInfo.EndSummary,
                            PicUrl = endImageUrl
                        };
                    }
                    else
                    {
                        var imageUrl = GetImageUrl(publishmentSystemInfo, appointmentInfo.ImageUrl);
                        var pageUrl = GetIndexUrl(publishmentSystemInfo, appointmentInfo.Id, wxOpenID);
                        if (appointmentInfo.ContentIsSingle)
                        {
                            var itemID = DataProviderWx.AppointmentItemDao.GetItemId(publishmentSystemInfo.PublishmentSystemId, appointmentInfo.Id);
                            pageUrl = GetItemUrl(publishmentSystemInfo, appointmentInfo.Id, itemID, wxOpenID);
                        }

                        article = new Article()
                        {
                            Title = appointmentInfo.Title,
                            Description = appointmentInfo.Summary,
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
