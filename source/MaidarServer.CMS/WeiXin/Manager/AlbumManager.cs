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
	public class AlbumManager
    {
        public static string GetImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            return PageUtils.AddProtocolToUrl(string.IsNullOrEmpty(imageUrl) ? SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, "weixin/album/img/start.jpg") : PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
        }

        public static string GetContentImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
        }

        private static string GetAlbumUrl(PublishmentSystemInfo publishmentSystemInfo)
        {
            return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, "weixin/album/index.html"));
        }

        public static string GetAlbumUrl(PublishmentSystemInfo publishmentSystemInfo, AlbumInfo albumInfo, string wxOpenId)
        {
            var attributes = new NameValueCollection
            {
                {"publishmentSystemID", albumInfo.PublishmentSystemId.ToString()},
                {"albumID", albumInfo.Id.ToString()},
                {"wxOpenID", wxOpenId}
            };
            return PageUtils.AddQueryString(GetAlbumUrl(publishmentSystemInfo), attributes);
        }

        public static List<Article> Trigger(PublishmentSystemInfo publishmentSystemInfo, Model.KeywordInfo keywordInfo, string wxOpenId)
        {
            var articleList = new List<Article>();

            DataProviderWx.CountDao.AddCount(keywordInfo.PublishmentSystemId, ECountType.RequestNews);

            var albumInfoList = DataProviderWx.AlbumDao.GetAlbumInfoListByKeywordId(keywordInfo.PublishmentSystemId, keywordInfo.KeywordId);

            foreach (var albumInfo in albumInfoList)
            {
                Article article = null;

                if (albumInfo != null)
                {
                    var imageUrl = GetImageUrl(publishmentSystemInfo, albumInfo.ImageUrl);
                    var pageUrl = GetAlbumUrl(publishmentSystemInfo, albumInfo, wxOpenId);

                    article = new Article
                    {
                        Title = albumInfo.Title,
                        Description = albumInfo.Summary,
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
