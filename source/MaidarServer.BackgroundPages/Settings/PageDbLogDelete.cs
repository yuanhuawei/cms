﻿using System;
using MaiDar.Core;

namespace MaiDarServer.BackgroundPages.Settings
{
    public class PageDbLogDelete : BasePage
    {
        protected override bool IsAccessable => true;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                BreadCrumbSettings("清空数据库日志", AppManager.Permissions.Settings.Utility);
            }
        }

        public string GetLastExecuteDate()
        {
            var dt = MaiDarDataProvider.LogDao.GetLastRemoveLogDate(Body.AdminName);
            return dt == DateTime.MinValue ? "无记录" : DateUtils.GetDateAndTimeString(dt);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                try
                {
                    MaiDarDataProvider.DatabaseDao.DeleteDbLog();

                    Body.AddAdminLog("清空数据库日志");

                    SuccessMessage("清空日志成功！");
                }
                catch (Exception ex)
                {
                    PageUtils.RedirectToErrorPage(ex.Message);
                }
            }
        }

    }
}
