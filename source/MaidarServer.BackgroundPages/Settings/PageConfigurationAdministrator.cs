﻿using System;
using System.Web.UI.WebControls;
using MaiDar.Core;
using MaiDar.Core.Model.Enumerations;

namespace MaiDarServer.BackgroundPages.Settings
{
    public class PageConfigurationAdministrator : BasePage
    {
        public TextBox TbLoginUserNameMinLength;
        public TextBox TbLoginPasswordMinLength;
        public DropDownList DdlLoginPasswordRestriction;

        public RadioButtonList RblIsLoginFailToLock;
        public PlaceHolder PhFailToLock;
        public TextBox TbLoginFailToLockCount;
        public DropDownList DdlLoginLockingType;
        public PlaceHolder PhLoginLockingHours;
        public TextBox TbLoginLockingHours;

        public RadioButtonList RblIsFindPassword;
        public PlaceHolder PhFindPassword;
        public TextBox TbFindPasswordSmsTplId;

        public RadioButtonList RblIsViewContentOnlySelf;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            BreadCrumbSettings("管理员设置", AppManager.Permissions.Settings.AdminManagement);

            TbLoginUserNameMinLength.Text = ConfigManager.SystemConfigInfo.AdminUserNameMinLength.ToString();
            TbLoginPasswordMinLength.Text = ConfigManager.SystemConfigInfo.AdminPasswordMinLength.ToString();
            EUserPasswordRestrictionUtils.AddListItems(DdlLoginPasswordRestriction);
            ControlUtils.SelectListItemsIgnoreCase(DdlLoginPasswordRestriction, ConfigManager.SystemConfigInfo.AdminPasswordRestriction);

            EBooleanUtils.AddListItems(RblIsLoginFailToLock, "是", "否");
            ControlUtils.SelectListItemsIgnoreCase(RblIsLoginFailToLock, ConfigManager.SystemConfigInfo.IsAdminLockLogin.ToString());

            PhFailToLock.Visible = ConfigManager.SystemConfigInfo.IsAdminLockLogin;

            TbLoginFailToLockCount.Text = ConfigManager.SystemConfigInfo.AdminLockLoginCount.ToString();

            DdlLoginLockingType.Items.Add(new ListItem("按小时锁定", EUserLockTypeUtils.GetValue(EUserLockType.Hours)));
            DdlLoginLockingType.Items.Add(new ListItem("永久锁定", EUserLockTypeUtils.GetValue(EUserLockType.Forever)));
            ControlUtils.SelectListItemsIgnoreCase(DdlLoginLockingType, ConfigManager.SystemConfigInfo.AdminLockLoginType);

            PhLoginLockingHours.Visible = false;
            if (!EUserLockTypeUtils.Equals(ConfigManager.SystemConfigInfo.AdminLockLoginType, EUserLockType.Forever))
            {
                PhLoginLockingHours.Visible = true;
                TbLoginLockingHours.Text = ConfigManager.SystemConfigInfo.AdminLockLoginHours.ToString();
            }

            EBooleanUtils.AddListItems(RblIsFindPassword, "启用", "禁用");
            ControlUtils.SelectListItemsIgnoreCase(RblIsFindPassword, ConfigManager.SystemConfigInfo.IsAdminFindPassword.ToString());
            PhFindPassword.Visible = ConfigManager.SystemConfigInfo.IsAdminFindPassword;
            TbFindPasswordSmsTplId.Text = ConfigManager.SystemConfigInfo.AdminFindPasswordSmsTplId;

            EBooleanUtils.AddListItems(RblIsViewContentOnlySelf, "不可以", "可以");
            ControlUtils.SelectListItemsIgnoreCase(RblIsViewContentOnlySelf, ConfigManager.SystemConfigInfo.IsViewContentOnlySelf.ToString());
        }

        public void RblIsLoginFailToLock_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhFailToLock.Visible = TranslateUtils.ToBool(RblIsLoginFailToLock.SelectedValue);
        }

        public void DdlLoginLockingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhLoginLockingHours.Visible = !EUserLockTypeUtils.Equals(EUserLockType.Forever, DdlLoginLockingType.SelectedValue);
        }

        public void RblIsFindPassword_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhFindPassword.Visible = TranslateUtils.ToBool(RblIsFindPassword.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                ConfigManager.SystemConfigInfo.AdminUserNameMinLength = TranslateUtils.ToInt(TbLoginUserNameMinLength.Text);
                ConfigManager.SystemConfigInfo.AdminPasswordMinLength = TranslateUtils.ToInt(TbLoginPasswordMinLength.Text);
                ConfigManager.SystemConfigInfo.AdminPasswordRestriction = DdlLoginPasswordRestriction.SelectedValue;

                ConfigManager.SystemConfigInfo.IsAdminLockLogin = TranslateUtils.ToBool(RblIsLoginFailToLock.SelectedValue);
                ConfigManager.SystemConfigInfo.AdminLockLoginCount = TranslateUtils.ToInt(TbLoginFailToLockCount.Text, 3);
                ConfigManager.SystemConfigInfo.AdminLockLoginType = DdlLoginLockingType.SelectedValue;
                ConfigManager.SystemConfigInfo.AdminLockLoginHours = TranslateUtils.ToInt(TbLoginLockingHours.Text);

                ConfigManager.SystemConfigInfo.IsAdminFindPassword = TranslateUtils.ToBool(RblIsFindPassword.SelectedValue);
                ConfigManager.SystemConfigInfo.AdminFindPasswordSmsTplId = TbFindPasswordSmsTplId.Text;

                ConfigManager.SystemConfigInfo.IsViewContentOnlySelf = TranslateUtils.ToBool(RblIsViewContentOnlySelf.SelectedValue);

                MaiDarDataProvider.ConfigDao.Update(ConfigManager.Instance);

                Body.AddAdminLog("管理员设置");
                SuccessMessage("管理员设置成功");
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }
        }
    }
}
