using MaiDar.Core;
using MaiDarServer.Plugin.Apis;
namespace MaiDarServer.CMS.Plugin.Apis
{
    public class AdminApi : IAdminApi
    {
        private AdminApi() { }

        public static AdminApi Instance { get; } = new AdminApi();

        public bool IsUserNameExists(string userName)
        {
            return MaiDarDataProvider.AdministratorDao.IsUserNameExists(userName);
        }
    }
}
