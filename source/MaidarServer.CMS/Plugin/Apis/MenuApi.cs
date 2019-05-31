using MaiDar.Core;
using MaiDarServer.CMS.Core;
using MaiDarServer.CMS.Core.Permissions;
using MaiDarServer.Plugin.Apis;
using MaiDarServer.Plugin.Models;

namespace MaiDarServer.CMS.Plugin.Apis
{
    public class MenuApi : IMenuApi
    {
        private readonly PluginMetadata _metadata;

        public MenuApi(PluginMetadata metadata)
        {
            _metadata = metadata;
        }

        public string GetMenuUrl(string relatedUrl)
        {
            return PageUtility.GetSiteFilesUrl(PageUtils.InnerApiUrl, PageUtils.Combine(DirectoryUtils.SiteFiles.Plugins, _metadata.Id, relatedUrl));
        }

        public bool IsPluginAuthorized
        {
            get
            {
                var body = new RequestBody();
                return PermissionsManager.HasAdministratorPermissions(body.AdminName, _metadata.Id);
            }
        }

        public bool IsSiteAuthorized(int publishmentSystemId)
        {
            var body = new RequestBody();
            return PermissionsManager.HasAdministratorPermissions(body.AdminName, _metadata.Id + publishmentSystemId);
        }
    }
}
