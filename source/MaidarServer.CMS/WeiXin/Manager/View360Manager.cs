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
	public class View360Manager
    {
        public static string GetImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, "weixin/view360/img/start.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
            }
        }

        public static string GetContentImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl, int sequence)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, $"weixin/view360/img/pic{sequence}.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
            }
        }

        private static string GetView360Url(PublishmentSystemInfo publishmentSystemInfo)
        {
            return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, "weixin/view360/index.html"));
        }

        public static string GetView360Url(PublishmentSystemInfo publishmentSystemInfo, View360Info view360Info, string wxOpenID)
        {
            var attributes = new NameValueCollection();
            attributes.Add("publishmentSystemID", view360Info.PublishmentSystemId.ToString());
            attributes.Add("view360ID", view360Info.Id.ToString());
            attributes.Add("wxOpenID", wxOpenID);
            return PageUtils.AddQueryString(GetView360Url(publishmentSystemInfo), attributes);
        }

        public static List<Article> Trigger(PublishmentSystemInfo publishmentSystemInfo, Model.KeywordInfo keywordInfo, string wxOpenID)
        {
            var articleList = new List<Article>();

            DataProviderWx.CountDao.AddCount(keywordInfo.PublishmentSystemId, ECountType.RequestNews);

            var view360InfoList = DataProviderWx.View360Dao.GetView360InfoListByKeywordId(keywordInfo.PublishmentSystemId, keywordInfo.KeywordId);

            foreach (var view360Info in view360InfoList)
            {
                Article article = null;

                if (view360Info != null)
                {
                    var imageUrl = GetImageUrl(publishmentSystemInfo, view360Info.ImageUrl);
                    var pageUrl = GetView360Url(publishmentSystemInfo, view360Info, wxOpenID);

                    article = new Article()
                    {
                        Title = view360Info.Title,
                        Description = view360Info.Summary,
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
	}
}
