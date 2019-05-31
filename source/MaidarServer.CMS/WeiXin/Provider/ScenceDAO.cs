﻿using MaiDar.Core;
using MaiDar.Core.Data;
using MaiDarServer.CMS.WeiXin.Model;

namespace MaiDarServer.CMS.WeiXin.Provider
{
    public class ScenceDao : DataProviderBase
    {
        public override string TableName => "wx_Scence";

        public ScenceInfo GetScenceInfo(int scenceId)
        {
            ScenceInfo scenceInfo = null;

            string sqlWhere = $"WHERE ID = {scenceId}";
            var sqlSelect = MaiDarDataProvider.DatabaseDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    scenceInfo = new ScenceInfo(rdr);
                }
                rdr.Close();
            }

            return scenceInfo;
        }

        // Mr.wu begin
        public void UpdateClickNum(int scenceId, int publishmentSystemId)
        {
            if (scenceId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} set ClickNum= ClickNum+1 WHERE ID = {scenceId} AND publishmentSystemID = {publishmentSystemId}";
                ExecuteNonQuery(sqlString);
            }
        }
        // Mr.wu end
    }
}
