﻿using MaiDarServer.CMS.WeiXin.WeiXinMP.Entities.JsonResult;

namespace MaiDarServer.CMS.WeiXin.WeiXinMP.AdvancedAPIs.OAuth
{
    /// <summary>
    /// 获取OAuth AccessToken的结果
    /// 如果错误，返回结果{"errcode":40029,"errmsg":"invalid code"}
    /// </summary>
    public class OAuthAccessTokenResult : WxJsonResult
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string openid { get; set; }
        public string scope { get; set; }
    }
}
