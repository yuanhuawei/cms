﻿using System;
using System.Web.UI.WebControls;
using MaiDar.Core;
using MaiDar.Core.Model.Enumerations;

namespace MaiDarServer.BackgroundPages.Settings
{
    public class PageLogConfiguration : BasePage
    {
        protected RadioButtonList RblIsTimeThreshold;
        public PlaceHolder PhTimeThreshold;
        protected TextBox TbTime;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            BreadCrumbSettings("日志阈值设置", AppManager.Permissions.Settings.Log);

            EBooleanUtils.AddListItems(RblIsTimeThreshold, "启用", "不启用");
            ControlUtils.SelectListItemsIgnoreCase(RblIsTimeThreshold, ConfigManager.SystemConfigInfo.IsTimeThreshold.ToString());
            TbTime.Text = ConfigManager.SystemConfigInfo.TimeThreshold.ToString();

            PhTimeThreshold.Visible = TranslateUtils.ToBool(RblIsTimeThreshold.SelectedValue);
        }

        public void RblIsTimeThreshold_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhTimeThreshold.Visible = TranslateUtils.ToBool(RblIsTimeThreshold.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                ConfigManager.SystemConfigInfo.IsTimeThreshold = TranslateUtils.ToBool(RblIsTimeThreshold.SelectedValue);
                if (ConfigManager.SystemConfigInfo.IsTimeThreshold)
                {
                    ConfigManager.SystemConfigInfo.TimeThreshold = TranslateUtils.ToInt(TbTime.Text);
                }

                MaiDarDataProvider.ConfigDao.Update(ConfigManager.Instance);

                Body.AddAdminLog("设置日志阈值参数");
                SuccessMessage("日志阈值参数设置成功");
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }
        }
    }
}
