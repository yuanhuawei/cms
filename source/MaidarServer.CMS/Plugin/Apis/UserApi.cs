using System.Collections.Generic;
using MaiDar.Core;
using MaiDar.Core.Model;
using MaiDarServer.Plugin.Apis;
using MaiDarServer.Plugin.Models;

namespace MaiDarServer.CMS.Plugin.Apis
{
    public class UserApi : IUserApi
    {
        private UserApi() { }

        public static UserApi Instance { get; } = new UserApi();

        public IUserInfo GetUserInfoByUserId(int userId)
        {
            return MaiDarDataProvider.UserDao.GetUserInfo(userId);
        }

        public IUserInfo GetUserInfoByUserName(string userName)
        {
            return MaiDarDataProvider.UserDao.GetUserInfoByUserName(userName);
        }

        public IUserInfo GetUserInfoByEmail(string email)
        {
            return MaiDarDataProvider.UserDao.GetUserInfoByEmail(email);
        }

        public IUserInfo GetUserInfoByMobile(string mobile)
        {
            return MaiDarDataProvider.UserDao.GetUserInfoByMobile(mobile);
        }

        public string GetMobileByAccount(string account)
        {
            return MaiDarDataProvider.UserDao.GetMobileByAccount(account);
        }

        public bool IsUserNameExists(string userName)
        {
            return MaiDarDataProvider.UserDao.IsUserNameExists(userName);
        }

        public bool IsEmailExists(string email)
        {
            return MaiDarDataProvider.UserDao.IsEmailExists(email);
        }

        public bool IsMobileExists(string mobile)
        {
            return MaiDarDataProvider.UserDao.IsMobileExists(mobile);
        }

        public IUserInfo NewInstance()
        {
            return new UserInfo();
        }

        public bool Insert(IUserInfo userInfo, string password, out string errorMessage)
        {
            return MaiDarDataProvider.UserDao.Insert(userInfo, password, PageUtils.GetIpAddress(), out errorMessage);
        }

        public bool Validate(string account, string password, out string userName, out string errorMessage)
        {
            return MaiDarDataProvider.UserDao.Validate(account, password, out userName, out errorMessage);
        }

        public void UpdateLastActivityDateAndCountOfFailedLogin(string userName)
        {
            MaiDarDataProvider.UserDao.UpdateLastActivityDateAndCountOfFailedLogin(userName);
        }

        public void UpdateLastActivityDateAndCountOfLogin(string userName)
        {
            MaiDarDataProvider.UserDao.UpdateLastActivityDateAndCountOfLogin(userName);
        }

        public bool ChangePassword(string userName, string password, out string errorMessage)
        {
            return MaiDarDataProvider.UserDao.ChangePassword(userName, password, out errorMessage);
        }

        public void Update(IUserInfo userInfo)
        {
            MaiDarDataProvider.UserDao.Update(userInfo);
        }

        public bool IsPasswordCorrect(string password, out string errorMessage)
        {
            return MaiDarDataProvider.UserDao.IsPasswordCorrect(password, out errorMessage);
        }

        public void AddLog(string userName, string action, string summary)
        {
            LogUtils.AddUserLog(userName, action, summary);
        }

        public List<ILogInfo> GetLogs(string userName, int totalNum, string action = "")
        {
            return MaiDarDataProvider.UserLogDao.List(userName, totalNum, action);
        }
    }
}
