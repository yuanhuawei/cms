using System.Text;
using MaiDar.Core;
using MaiDarServer.CMS.Core;
using MaiDarServer.CMS.Model;

namespace MaiDarServer.CMS.WeiXin.Manager
{
    public class ComponentsManager
    {
        public static string GetBackgroundImageUrl(PublishmentSystemInfo publishmentSystemInfo, string backgroundImageUrl)
        {
            if (string.IsNullOrEmpty(backgroundImageUrl))
            {
                backgroundImageUrl = "1.jpg";
            }
            if (!backgroundImageUrl.StartsWith("@"))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, $"weixin/components/background/{backgroundImageUrl}"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, backgroundImageUrl));
            }
        }

        public static string GetBackgroundImageSelectHtml(PublishmentSystemInfo publishmentSystemInfo, string backgroundImageUrl)
        {
            var builder = new StringBuilder(@"<select id=""backgroundSelect"" onchange=""if ($(this).val()){$('#preview_backgroundImageUrl').attr('src', $(this).val());$('#backgroundImageUrl').val($(this).find('option:selected').attr('url'));}""><option value="""">选择预设背景</option>");
            if (string.IsNullOrEmpty(backgroundImageUrl))
            {
                backgroundImageUrl = "0.jpg";
            }
            for (var i = 0; i <= 20; i++)
            {
                var fileName = i + ".jpg";
                var selected = string.Empty;
                if (fileName == backgroundImageUrl)
                {
                    selected = "selected";
                }
                builder.AppendFormat(@"<option value=""{0}"" url=""{1}"" {2}>{3}</option>", PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, $"weixin/components/background/{i}.jpg")), fileName, selected, "预设背景" + (i + 1));
            }
            builder.Append("</select>");
            return builder.ToString();
        }
	}
}
