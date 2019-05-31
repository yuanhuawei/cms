using MaiDarServer.CMS.WeiXin.WeiXinMP.Entities.JsonResult;

namespace MaiDarServer.CMS.WeiXin.WeiXinMP.AdvancedAPIs.Groups
{
    /// <summary>
    /// 创建分组返回结果
    /// </summary>
    public class CreateGroupResult : WxJsonResult
    {
        public GroupsJson_Group group { get; set; }
    }
}
