﻿using System.Collections.Generic;
using System.Data;
using MaiDar.Core;
using MaiDar.Core.Data;
using MaiDarServer.CMS.WeiXin.Model;

namespace MaiDarServer.CMS.WeiXin.Provider
{
    public class AccountDao : DataProviderBase
    {
        public override string TableName => "wx_Account";

        public int Insert(AccountInfo accountInfo)
        {
            int accountId;

            IDataParameter[] parms;

            var sqlInsert = MaiDarDataProvider.DatabaseDao.GetInsertSqlString(accountInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        accountId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return accountId;
        }

        public void Update(AccountInfo accountInfo)
        {
            IDataParameter[] parms;
            var sqlUpdate = MaiDarDataProvider.DatabaseDao.GetUpdateSqlString(accountInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }

        public AccountInfo GetAccountInfo(int publishmentSystemId)
        {
            AccountInfo accountInfo = null;

            string sqlWhere = $"WHERE {AccountAttribute.PublishmentSystemId} = {publishmentSystemId}";
            var sqlSelect = MaiDarDataProvider.DatabaseDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    accountInfo = new AccountInfo(rdr);
                }
                rdr.Close();
            }

            if (accountInfo == null)
            {
                accountInfo = new AccountInfo
                {
                    PublishmentSystemId = publishmentSystemId,
                    Token = StringUtils.GetShortGuid()
                };
                accountInfo.Id = Insert(accountInfo);
            }

            return accountInfo;
        }

        public List<AccountInfo> GetAccountInfoList(int publishmentSystemId)
        {
            var accountInfoList = new List<AccountInfo>();

            string sqlWhere = $"WHERE {AccountAttribute.PublishmentSystemId} = {publishmentSystemId}";

            var sqlSelect = MaiDarDataProvider.DatabaseDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var accountInfo = new AccountInfo(rdr);
                    accountInfoList.Add(accountInfo);
                }
                rdr.Close();
            }

            return accountInfoList;
        }
    }
}
