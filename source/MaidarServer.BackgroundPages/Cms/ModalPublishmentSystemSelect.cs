using System;
using System.Text;
using System.Web.UI.WebControls;
using MaiDar.Core;
using MaiDar.Core.Model.Enumerations;
using MaiDarServer.CMS.Core;
using MaiDarServer.CMS.Core.Security;

namespace MaiDarServer.BackgroundPages.Cms
{
    public class ModalPublishmentSystemSelect : BasePageCms
    {
        public Literal ltlHtml;

        public static string GetOpenLayerString()
        {
            return PageUtils.GetOpenLayerString("选择站点", PageUtils.GetCmsUrl(nameof(ModalPublishmentSystemSelect), null));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                var builder = new StringBuilder();

                var publishmentSystemIdList = ProductPermissionsManager.Current.PublishmentSystemIdList;
                foreach (var publishmentSystemId in publishmentSystemIdList)
                {
                    var loadingUrl = PageUtils.GetLoadingUrl(PageMain.GetRedirectUrl(publishmentSystemId));
                    var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                    builder.Append($@"
<span class=""icon-span"">
    <a href=""{loadingUrl}"" target=""_top"">
      <h5>
        {publishmentSystemInfo.PublishmentSystemName}
        <br>
        <small>{publishmentSystemInfo.PublishmentSystemDir}</small>
      </h5>
    </a>
  </span>");
                }

                ltlHtml.Text = builder.ToString();
            }
        }
    }
}
