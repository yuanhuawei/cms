﻿using System.Collections.Generic;
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
	public class SearchManager
    {
        public static string GetImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, "weixin/search/img/start.jpg"));
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
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, "weixin/search/img/head_img.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
            }
        }

        public static string GetSearchUrl(PublishmentSystemInfo publishmentSystemInfo, SearchInfo searchInfo)
        {
            var attributes = new NameValueCollection();
            attributes.Add("publishmentSystemID", searchInfo.PublishmentSystemId.ToString());
            attributes.Add("searchID", searchInfo.Id.ToString());
            var url = PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, "weixin/search/index.html"));
            return PageUtils.AddQueryString(url, attributes);
        }

        //public static List<ContentInfo> GetContentInfoList(PublishmentSystemInfo publishmentSystemInfo, int nodeID, string keywords)
        //{
        //    List<ContentInfo> contentInfoList = new List<ContentInfo>();
        //    if (nodeID > 0)
        //    {
        //        contentInfoList = DataProvider.BackgroundContentDAO.
        //    }
        //    if (!string.IsNullOrEmpty(keyWords))
        //    {
        //        contentInfoList = DataProvider.ContentDAO.GetContentInfoList(ETableStyle.Site, publishmentSystemInfo.AuxiliaryTableForContent, publishmentSystemID, nodeID, keyWords);
        //    }

        //    return contentInfoList;
        //}

        public static List<Article> Trigger(PublishmentSystemInfo publishmentSystemInfo, Model.KeywordInfo keywordInfo, string wxOpenID)
        {
            var articleList = new List<Article>();

            DataProviderWx.CountDao.AddCount(keywordInfo.PublishmentSystemId, ECountType.RequestNews);

            var searchInfoList = DataProviderWx.SearchDao.GetSearchInfoListByKeywordId(keywordInfo.PublishmentSystemId, keywordInfo.KeywordId);

            foreach (var searchInfo in searchInfoList)
            {
                Article article = null;

                if (searchInfo != null)
                {
                    var imageUrl = GetImageUrl(publishmentSystemInfo, searchInfo.ImageUrl);
                    var pageUrl = GetSearchUrl(publishmentSystemInfo, searchInfo);

                    article = new Article
                    {
                        Title = searchInfo.Title,
                        Description = searchInfo.Summary,
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
