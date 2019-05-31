using System;
using MaiDar.Core;
using MaiDarServer.BackgroundPages.Settings;
using MaiDarServer.CMS.Core.Create;

namespace MaiDarServer.BackgroundPages.Cms
{
    public class PageCreateIndex : BasePageCms
    {
        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID"); // 检测参数PublishmentSystemID是否合法(不能为空)

            if (!IsPostBack)
            {
                CreateManager.CreateChannel(PublishmentSystemId, PublishmentSystemId); // 创建任务
                PageCreateStatus.Redirect(PublishmentSystemId); // 转到查询任务进度页面
            }
        }
    }
}
