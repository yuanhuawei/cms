using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using MaiDar.Core;
using MaiDarServer.CMS.Core;

namespace MaiDarServer.BackgroundPages.Cms
{
	public class ModalTemplateView : BasePageCms
    {
        public TextBox TbContent;

        public static string GetOpenLayerString(int publishmentSystemId, int templateLogId)
        {
            return PageUtils.GetOpenLayerString("查看修订内容", PageUtils.GetCmsUrl(nameof(ModalTemplateView), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"templateLogID", templateLogId.ToString()}
            }));
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
           
			if (!IsPostBack)
			{
                var templateLogId = Body.GetQueryInt("templateLogID");
                TbContent.Text = DataProvider.TemplateLogDao.GetTemplateContent(templateLogId);
			}
		}
	}
}
