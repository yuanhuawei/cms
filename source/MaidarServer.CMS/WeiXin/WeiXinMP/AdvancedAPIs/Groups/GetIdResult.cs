﻿using MaiDarServer.CMS.WeiXin.WeiXinMP.Entities.JsonResult;

namespace MaiDarServer.CMS.WeiXin.WeiXinMP.AdvancedAPIs.Groups
{
    /// <summary>
    /// 获取用户分组ID返回结果
    /// </summary>
    public class GetGroupIdResult : WxJsonResult
    {
        public int groupid { get; set; }
    }
}
