using MaiDar.Core;
using MaiDar.Core.Model;
using MaiDarServer.CMS.Core;
using MaiDarServer.CMS.Model;
using MaiDarServer.CMS.WeiXin.Manager;

namespace MaiDarServer.CMS.WeiXin.IO
{
    public class PageUtilityWX
    {
        private PageUtilityWX()
        {
        }

        public static string GetWeiXinTemplateDirectoryUrl(PublishmentSystemInfo publishmentSystemInfo)
        {
            return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, "weixin/components/templates"));
        }

        public static string GetWeiXinFileUrl(PublishmentSystemInfo publishmentSystemInfo, int keywordID, int resourceID)
        {
            return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo,
                $"@/weixin-files/{keywordID}-{resourceID}.html"));
        }

        public static string GetContentUrl(PublishmentSystemInfo publishmentSystemInfo, ContentInfo contentInfo)
        {
            return PageUtils.AddProtocolToUrl(PageUtility.GetContentUrl(publishmentSystemInfo, contentInfo));
        }

        public static string GetContentUrl(PublishmentSystemInfo publishmentSystemInfo, int channelID, int contentID, bool isFromBackground)
        {
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, channelID);
            return PageUtils.AddProtocolToUrl(PageUtility.GetContentUrl(publishmentSystemInfo, nodeInfo, contentID, isFromBackground));
        }

        public static string GetChannelUrl(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo)
        {
            return PageUtils.AddProtocolToUrl(PageUtility.GetChannelUrl(publishmentSystemInfo, nodeInfo, false));
        }

        public static string GetChannelUrl(PublishmentSystemInfo publishmentSystemInfo, int channelID)
        {
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, channelID);
            return GetChannelUrl(publishmentSystemInfo, nodeInfo);
        }

        #region Vote

        public static string GetVoteTemplateDirectoryUrl(PublishmentSystemInfo publishmentSystemInfo)
        {
            return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, "weixin/vote"));
        }

        public static string GetVoteUrl(PublishmentSystemInfo publishmentSystemInfo, int keywordID, int voteID)
        {
            return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo,
                $"@/weixin-files/{keywordID}-{voteID}.html"));
        }

        #endregion

        public class API
        {
            public static string GetMPUrl(int publishmentSystemID)
            {
                var url = PageUtils.AddProtocolToUrl(PageUtils.Combine(PageUtils.GetHost(), "api/mp/url/", publishmentSystemID.ToString()));

                return PageUtils.RemovePortFromUrl(url);
            }

            public static string GetMPToken(int publishmentSystemID)
            {
                return WeiXinManager.GetAccountInfo(publishmentSystemID).Token;
            }
        }
    }
}
