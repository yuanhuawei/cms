﻿using System;
using System.Web.UI.WebControls;
using MaiDar.Core;
using MaiDarServer.CMS.Controllers.Sys.Administrators;

namespace MaiDarServer.BackgroundPages
{
    public class PageRight : BasePage
    {
        public Literal LtlWelcome;
        public Literal LtlVersionInfo;
        public Literal LtlUpdateDate;
        public Literal LtlLastLoginDate;

        public string ApiUrl => SiteCheckList.GetUrl(PageUtils.InnerApiUrl, Body.AdminName);

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            LtlWelcome.Text = "欢迎使用 SiteServer 管理后台";

            LtlVersionInfo.Text = AppManager.GetFullVersion();

            if (Body.AdministratorInfo.LastActivityDate != DateTime.MinValue)
            {
                LtlLastLoginDate.Text = DateUtils.GetDateAndTimeString(Body.AdministratorInfo.LastActivityDate);
            }

            LtlUpdateDate.Text = DateUtils.GetDateAndTimeString(ConfigManager.Instance.UpdateDate);
        }
	}
}
