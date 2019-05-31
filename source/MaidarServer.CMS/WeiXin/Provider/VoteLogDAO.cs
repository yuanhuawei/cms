﻿using System.Collections.Generic;
using System.Data;
using MaiDar.Core;
using MaiDar.Core.Data;
using MaiDarServer.CMS.WeiXin.Model;

namespace MaiDarServer.CMS.WeiXin.Provider
{
    public class VoteLogDao : DataProviderBase
    {
        public override string TableName => "wx_VoteLog";

        public void Insert(VoteLogInfo logInfo)
        {
            IDataParameter[] parms = null;

            var sqlInsert = MaiDarDataProvider.DatabaseDao.GetInsertSqlString(logInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlInsert, parms);
        }

        public void DeleteAll(int voteId)
        {
            if (voteId > 0)
            {
                string sqlString = $"DELETE FROM {TableName} WHERE {VoteLogAttribute.VoteId} = {voteId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(List<int> logIdList)
        {
            if (logIdList != null && logIdList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(logIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public int GetCount(int voteId)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE {VoteLogAttribute.VoteId} = {voteId}";

            return MaiDarDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public bool IsVoted(int voteId, string cookieSn, string wxOpenId)
        {
            var isVoted = false;
            string sqlString;
            if (string.IsNullOrEmpty(wxOpenId))
            {
                sqlString =
                    $"SELECT COUNT(*) FROM {TableName} WHERE {VoteLogAttribute.VoteId} = {voteId} AND {VoteLogAttribute.CookieSn} = '{cookieSn}' ";
            }
            else
            {
                sqlString =
                    $"SELECT COUNT(*) FROM {TableName} WHERE {VoteLogAttribute.VoteId} = {voteId} AND {VoteLogAttribute.WxOpenId} = '{wxOpenId}' ";
            } 

            isVoted = MaiDarDataProvider.DatabaseDao.GetIntResult(sqlString) > 0;

            return isVoted;
        }

        public string GetSelectString(int voteId)
        {
            string whereString = $"WHERE {VoteLogAttribute.VoteId} = {voteId}";
            return MaiDarDataProvider.DatabaseDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public List<VoteLogInfo> GetVoteLogInfoListByVoteId(int publishmentSystemId, int voteId)
        {
            var voteLogInfoList = new List<VoteLogInfo>();

            string sqlWhere =
                $"WHERE {VoteLogAttribute.PublishmentSystemId} = {publishmentSystemId} AND {VoteLogAttribute.VoteId} = '{voteId}'";

            var sqlSelect = MaiDarDataProvider.DatabaseDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var voteLogInfo = new VoteLogInfo(rdr);
                    voteLogInfoList.Add(voteLogInfo);
                }
                rdr.Close();
            }

            return voteLogInfoList;
        }

        public List<VoteLogInfo> GetVoteLogInfoList(int publishmentSystemId)
        {
            var voteLogInfoList = new List<VoteLogInfo>();

            string sqlWhere = $"WHERE {VoteLogAttribute.PublishmentSystemId} = {publishmentSystemId}";

            var sqlSelect = MaiDarDataProvider.DatabaseDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var voteLogInfo = new VoteLogInfo(rdr);
                    voteLogInfoList.Add(voteLogInfo);
                }
                rdr.Close();
            }

            return voteLogInfoList;
        }

        public int GetCount(int voteId, string iPAddress)
        {

            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE {VoteLogAttribute.VoteId} = {voteId} AND {VoteLogAttribute.IpAddress} = '{iPAddress}'";

            return MaiDarDataProvider.DatabaseDao.GetIntResult(sqlString);
        }
    }
}
