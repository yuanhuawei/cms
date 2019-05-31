﻿using System.Collections.Generic;
using System.Data;
using System.Text;
using MaiDar.Core;
using MaiDar.Core.Data;
using MaiDarServer.CMS.WeiXin.Data;
using MaiDarServer.CMS.WeiXin.Manager;
using MaiDarServer.CMS.WeiXin.Model;
using MaiDarServer.CMS.WeiXin.Model.Enumerations;

namespace MaiDarServer.CMS.WeiXin.Provider
{
    public class CouponSnDao : DataProviderBase
    {
        public override string TableName => "wx_CouponSN";

        public int Insert(CouponSnInfo couponSnInfo)
        {
            var couponSnid = 0;

            IDataParameter[] parms = null;

            var sqlInsert = MaiDarDataProvider.DatabaseDao.GetInsertSqlString(couponSnInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);


            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        couponSnid = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return couponSnid;
        }

        public void Insert(int publishmentSystemId, int couponId, int totalNum)
        {
            var couponSnList = CouponManager.GetCouponSN(totalNum);
            foreach (var sn in couponSnList)
            {
                string sqlString =
                    $"INSERT INTO {TableName} (PublishmentSystemID, CouponID, SN, Status) VALUES ({publishmentSystemId}, {couponId}, '{sn}', '{ECouponStatusUtils.GetValue(ECouponStatus.Unused)}')";

                ExecuteNonQuery(sqlString);

            }

            DataProviderWx.CouponDao.UpdateTotalNum(couponId, DataProviderWx.CouponSnDao.GetTotalNum(publishmentSystemId, couponId));
        }

        public void Insert(int publishmentSystemId, int couponId, List<string> snList)
        {

            foreach (var sn in snList)
            {
                if (!string.IsNullOrEmpty(sn))
                {
                    string sqlString =
                        $"INSERT INTO {TableName} (PublishmentSystemID, CouponID, SN, Status) VALUES ({publishmentSystemId}, {couponId}, '{sn}', '{ECouponStatusUtils.GetValue(ECouponStatus.Unused)}')";
                    ExecuteNonQuery(sqlString);

                }
            }

            DataProviderWx.CouponDao.UpdateTotalNum(couponId, DataProviderWx.CouponSnDao.GetTotalNum(publishmentSystemId, couponId));
        }

        public void Update(CouponSnInfo couponSnInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = MaiDarDataProvider.DatabaseDao.GetUpdateSqlString(couponSnInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);

        }

        public void UpdateStatus(ECouponStatus status, List<int> snIdList)
        {
            string sqlString =
                $"UPDATE {TableName} SET {CouponSnAttribute.Status} = '{ECouponStatusUtils.GetValue(status)}' WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(snIdList)})";

            if (status == ECouponStatus.Cash)
            {
                sqlString =
                    $"UPDATE {TableName} SET {CouponSnAttribute.Status} = '{ECouponStatusUtils.GetValue(status)}', {CouponSnAttribute.HoldDate} = getdate(), {CouponSnAttribute.CashDate} = getdate() WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(snIdList)})";
            }
            else if (status == ECouponStatus.Hold)
            {
                sqlString =
                    $"UPDATE {TableName} SET {CouponSnAttribute.Status} = '{ECouponStatusUtils.GetValue(status)}', {CouponSnAttribute.HoldDate} = getdate() WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(snIdList)})";
            }

            ExecuteNonQuery(sqlString);
        }

        public int Hold(int publishmentSystemId, int actId, string cookieSn)
        {
            var snId = 0;

            string sqlString = $@"SELECT ID FROM wx_CouponSN WHERE 
PublishmentSystemID = {publishmentSystemId} AND 
CookieSN = '{cookieSn}' AND 
Status <> '{ECouponStatusUtils.GetValue(ECouponStatus.Unused)}' AND
CouponID IN (SELECT ID FROM wx_Coupon WHERE ActID = {actId})";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    snId = rdr.GetInt32(0);
                }
                rdr.Close();
            }

            if (snId == 0)
            {
                sqlString = $@"SELECT TOP 1 ID FROM wx_CouponSN WHERE 
PublishmentSystemID = {publishmentSystemId} AND
Status = '{ECouponStatusUtils.GetValue(ECouponStatus.Unused)}' AND
CouponID IN (SELECT ID FROM wx_Coupon WHERE ActID = {actId})
ORDER BY ID";

                using (var rdr = ExecuteReader(sqlString))
                {
                    if (rdr.Read())
                    {
                        snId = rdr.GetInt32(0);
                    }
                    rdr.Close();
                }

            }

            return snId;
        }

        public void Delete(int snId)
        {
            if (snId > 0)
            {
                string sqlString = $"DELETE FROM {TableName} WHERE ID = {snId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(List<int> snIdList)
        {
            if (snIdList != null && snIdList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(snIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public CouponSnInfo GetSnInfo(int snId)
        {
            CouponSnInfo snInfo = null;

            string sqlWhere = $"WHERE ID = {snId}";
            var sqlSelect = MaiDarDataProvider.DatabaseDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    snInfo = new CouponSnInfo(rdr);
                }
                rdr.Close();
            }

            return snInfo;
        }

        public List<CouponSnInfo> GetSnInfoByCookieSn(int publishmentSystemId, string cookieSn, string uniqueId)
        {
            var list = new List<CouponSnInfo>();
            StringBuilder builder;
            if (string.IsNullOrEmpty(uniqueId))
            {
                builder = new StringBuilder(
                    $"WHERE {CouponSnAttribute.PublishmentSystemId} = {publishmentSystemId} AND {CouponSnAttribute.CookieSn} = '{PageUtils.FilterSql(cookieSn)}'");
            }
            else
            {
                builder = new StringBuilder(
                    $"WHERE {CouponSnAttribute.PublishmentSystemId} = {publishmentSystemId} AND {CouponSnAttribute.WxOpenId} = '{uniqueId}'");
            }
            var sqlSelect = MaiDarDataProvider.DatabaseDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY ID");

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var couponSnInfo = new CouponSnInfo(rdr);
                    list.Add(couponSnInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public int GetTotalNum(int publishmentSystemId, int couponId)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE {CouponSnAttribute.PublishmentSystemId} = {publishmentSystemId} AND {CouponSnAttribute.CouponId} = {couponId}";
            return MaiDarDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetHoldNum(int publishmentSystemId, int couponId)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE {CouponSnAttribute.PublishmentSystemId} = {publishmentSystemId} AND {CouponSnAttribute.CouponId} = {couponId} AND ({CouponSnAttribute.Status} = '{ECouponStatusUtils.GetValue(ECouponStatus.Hold)}')";
            return MaiDarDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetCashNum(int publishmentSystemId, int couponId)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE {CouponSnAttribute.PublishmentSystemId} = {publishmentSystemId} AND {CouponSnAttribute.CouponId} = {couponId} AND {CouponSnAttribute.Status} = '{ECouponStatusUtils.GetValue(ECouponStatus.Cash)}'";
            return MaiDarDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetSelectString(int publishmentSystemId, int couponId)
        {
            string whereString =
                $"WHERE {CouponSnAttribute.PublishmentSystemId} = {publishmentSystemId} AND {CouponSnAttribute.CouponId} = {couponId}";
            return MaiDarDataProvider.DatabaseDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public List<CouponSnInfo> GetCouponSnInfoList(int publishmentSystemId, int couponId)
        {
            var couponSnInfoList = new List<CouponSnInfo>();

            string sqlWhere =
                $"WHERE {CouponSnAttribute.PublishmentSystemId} = {publishmentSystemId} AND {CouponSnAttribute.CouponId} = {couponId}";

            var sqlSelect = MaiDarDataProvider.DatabaseDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var couponSnInfo = new CouponSnInfo(rdr);
                    couponSnInfoList.Add(couponSnInfo);
                }
                rdr.Close();
            }

            return couponSnInfoList;
        }

    }
}
