using System;
using MaiDar.Core;
using MaiDarServer.CMS.WeiXin.Data;

namespace MaiDarServer.BackgroundPages.WeiXin
{
    public class PageAppointmentItemDelete : BasePageCms
    {
        public void Page_Load(object sender, EventArgs e)
        { 
            var list = TranslateUtils.StringCollectionToIntList(Request["IDCollection"]);
            if (list.Count > 0)
            {
                try
                {
                    DataProviderWx.AppointmentItemDao.Delete(PublishmentSystemId, list);
                    Response.Write("success");
                    Response.End();
                }
                catch (Exception ex)
                {
                    Response.Write("failure");
                    Response.End();
                }
            }
        }
    }
}
