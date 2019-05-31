using System;
using MaiDar.Core;
using MaiDarServer.BackgroundPages.Settings;
using MaiDarServer.CMS.Core.Create;

namespace MaiDarServer.BackgroundPages.Cms
{
    public class PageCreateAll : BasePageCms
    {
        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (!IsPostBack)
            {
                CreateManager.CreateAll(PublishmentSystemId);
                PageCreateStatus.Redirect(PublishmentSystemId);
            }
        }
    }
}
