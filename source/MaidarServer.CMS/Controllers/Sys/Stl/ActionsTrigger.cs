using System.Collections.Specialized;
using MaiDar.Core;

namespace MaiDarServer.CMS.Controllers.Sys.Stl
{
    public class ActionsTrigger
    {
        public const string Route = "sys/stl/actions/trigger";

        public static string GetUrl(string apiUrl, int publishmentSystemId, int channelId, int contentId,
            int fileTemplateId, bool isRedirect)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(apiUrl, Route), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"channelId", channelId.ToString()},
                {"contentId", contentId.ToString()},
                {"fileTemplateId", fileTemplateId.ToString()},
                {"isRedirect", isRedirect.ToString()}
            });
        }
    }
}