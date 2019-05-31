using System;
using System.Web.Http;
using MaiDar.Core;
using MaiDarServer.CMS.Controllers.Sys.Administrators;
using MaiDarServer.CMS.Core;

namespace MaiDarServer.API.Controllers.Sys.Administrators
{
    [RoutePrefix("api")]
    public class AdministratorsActionsLoginController : ApiController
    {
        [HttpPost, Route(ActionsLogin.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var body = new RequestBody();
                var account = body.GetPostString("account");
                var password = body.GetPostString("password");
                if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
                {
                    return Unauthorized();
                }

                string userName;
                string errorMessage;
                if (!MaiDarDataProvider.AdministratorDao.ValidateAccount(account, password, out userName, out errorMessage))
                {
                    LogUtils.AddAdminLog(userName, "后台管理员登录失败");
                    MaiDarDataProvider.AdministratorDao.UpdateLastActivityDateAndCountOfFailedLogin(userName);
                    return Unauthorized();
                }

                MaiDarDataProvider.AdministratorDao.UpdateLastActivityDateAndCountOfLogin(userName);
                body.AdminLogin(userName);
                return Ok(new
                {
                    UserName = userName
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
