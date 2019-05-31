﻿using System.Collections.Generic;
using System.Data;
using MaiDar.Core;
using MaiDar.Core.Data;
using MaiDarServer.CMS.WeiXin.Data;
using MaiDarServer.CMS.WeiXin.Model;

namespace MaiDarServer.CMS.WeiXin.Provider
{
    public class ConferenceDao : DataProviderBase
    {
        public override string TableName => "wx_Conference";

        public int Insert(ConferenceInfo conferenceInfo)
        {
            var conferenceId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = MaiDarDataProvider.DatabaseDao.GetInsertSqlString(conferenceInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);


            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        conferenceId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return conferenceId;
        }

        public void Update(ConferenceInfo conferenceInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = MaiDarDataProvider.DatabaseDao.GetUpdateSqlString(conferenceInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }

        public void AddUserCount(int conferenceId)
        {
            if (conferenceId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {ConferenceAttribute.UserCount} = {ConferenceAttribute.UserCount} + 1 WHERE ID = {conferenceId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void AddPvCount(int conferenceId)
        {
            if (conferenceId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {ConferenceAttribute.PvCount} = {ConferenceAttribute.PvCount} + 1 WHERE ID = {conferenceId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, int conferenceId)
        {
            if (conferenceId > 0)
            {
                var conferenceIdList = new List<int>();
                conferenceIdList.Add(conferenceId);
                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(conferenceIdList));

                DataProviderWx.ConferenceContentDao.DeleteAll(conferenceId);

                string sqlString = $"DELETE FROM {TableName} WHERE ID = {conferenceId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, List<int> conferenceIdList)
        {
            if (conferenceIdList != null && conferenceIdList.Count > 0)
            {
                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(conferenceIdList));

                foreach (var conferenceId in conferenceIdList)
                {
                    DataProviderWx.ConferenceContentDao.DeleteAll(conferenceId);
                }

                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(conferenceIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetConferenceIdList(int publishmentSystemId)
        {
            var conferenceIdList = new List<int>();

            string sqlWhere = $"WHERE {ConferenceAttribute.PublishmentSystemId} = {publishmentSystemId}";
            var sqlSelect = MaiDarDataProvider.DatabaseDao.GetSelectSqlString(ConnectionString, TableName, 0, ConferenceAttribute.Id, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    conferenceIdList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return conferenceIdList;
        }

        public void UpdateUserCount(int publishmentSystemId, Dictionary<int, int> conferenceIdWithCount)
        {
            if (conferenceIdWithCount.Count == 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {ConferenceAttribute.UserCount} = 0 WHERE {ConferenceAttribute.PublishmentSystemId} = {publishmentSystemId}";
                ExecuteNonQuery(sqlString);
            }
            else
            {
                var conferenceIdList = GetConferenceIdList(publishmentSystemId);
                foreach (var conferenceId in conferenceIdList)
                {
                    if (conferenceIdWithCount.ContainsKey(conferenceId))
                    {
                        string sqlString =
                            $"UPDATE {TableName} SET {ConferenceAttribute.UserCount} = {conferenceIdWithCount[conferenceId]} WHERE ID = {conferenceId}";
                        ExecuteNonQuery(sqlString);
                    }
                    else
                    {
                        string sqlString =
                            $"UPDATE {TableName} SET {ConferenceAttribute.UserCount} = 0 WHERE ID = {conferenceId}";
                        ExecuteNonQuery(sqlString);
                    }
                }
            }
        }

        private List<int> GetKeywordIdList(List<int> conferenceIdList)
        {
            var keywordIdList = new List<int>();

            string sqlString =
                $"SELECT {ConferenceAttribute.KeywordId} FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(conferenceIdList)})";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    keywordIdList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return keywordIdList;
        }

        public ConferenceInfo GetConferenceInfo(int conferenceId)
        {
            ConferenceInfo conferenceInfo = null;

            string sqlWhere = $"WHERE ID = {conferenceId}";
            var sqlSelect = MaiDarDataProvider.DatabaseDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    conferenceInfo = new ConferenceInfo(rdr);
                }
                rdr.Close();
            }

            return conferenceInfo;
        }

        public List<ConferenceInfo> GetConferenceInfoListByKeywordId(int publishmentSystemId, int keywordId)
        {
            var conferenceInfoList = new List<ConferenceInfo>();

            string sqlWhere =
                $"WHERE {ConferenceAttribute.PublishmentSystemId} = {publishmentSystemId} AND {ConferenceAttribute.IsDisabled} <> '{true}'";
            if (keywordId > 0)
            {
                sqlWhere += $" AND {ConferenceAttribute.KeywordId} = {keywordId}";
            }

            var sqlSelect = MaiDarDataProvider.DatabaseDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var conferenceInfo = new ConferenceInfo(rdr);
                    conferenceInfoList.Add(conferenceInfo);
                }
                rdr.Close();
            }

            return conferenceInfoList;
        }

        public int GetFirstIdByKeywordId(int publishmentSystemId, int keywordId)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TableName} WHERE {ConferenceAttribute.PublishmentSystemId} = {publishmentSystemId} AND {ConferenceAttribute.IsDisabled} <> '{true}' AND {ConferenceAttribute.KeywordId} = {keywordId}";

            return MaiDarDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetTitle(int conferenceId)
        {
            var title = string.Empty;

            string sqlWhere = $"WHERE ID = {conferenceId}";
            var sqlSelect = MaiDarDataProvider.DatabaseDao.GetSelectSqlString(ConnectionString, TableName, 0, ConferenceAttribute.Title, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    title = rdr.GetValue(0).ToString();
                }
                rdr.Close();
            }

            return title;
        }

        public string GetSelectString(int publishmentSystemId)
        {
            string whereString = $"WHERE {ConferenceAttribute.PublishmentSystemId} = {publishmentSystemId}";
            return MaiDarDataProvider.DatabaseDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public List<ConferenceInfo> GetConferenceInfoList(int publishmentSystemId)
        {
            var conferenceInfoList = new List<ConferenceInfo>();

            string sqlWhere = $" AND {ConferenceAttribute.PublishmentSystemId} = {publishmentSystemId}";

            var sqlSelect = MaiDarDataProvider.DatabaseDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var conferenceInfo = new ConferenceInfo(rdr);
                    conferenceInfoList.Add(conferenceInfo);
                }
                rdr.Close();
            }

            return conferenceInfoList;
        }

    }
}
