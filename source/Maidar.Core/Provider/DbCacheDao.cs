using System;
using System.Collections.Generic;
using System.Data;
using MaiDar.Core.Data;
using MaiDar.Core.Model;
using MaiDarServer.Plugin.Models;

namespace MaiDar.Core.Provider
{
    public class DbCacheDao : DataProviderBase
    {
        public override string TableName => "bairong_DbCache";

        public override List<TableColumnInfo> TableColumns => new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = "CacheKey",
                DataType = DataType.VarChar,
                Length = 200,
                IsPrimaryKey = true
            },
            new TableColumnInfo
            {
                ColumnName = "CacheValue",
                DataType = DataType.NVarChar,
                Length = 500
            },
            new TableColumnInfo
            {
                ColumnName = "AddDate",
                DataType = DataType.DateTime
            }
        };

        private const string SqlSelectValue = "SELECT CacheValue FROM bairong_DbCache WHERE CacheKey = @CacheKey";

        private const string SqlSelectCount = "SELECT COUNT(*) FROM bairong_DbCache";

        private const string SqlInsert = "INSERT INTO bairong_DbCache (CacheKey, CacheValue, AddDate) VALUES (@CacheKey, @CacheValue, @AddDate)";

        private const string SqlDelete = "DELETE FROM bairong_DbCache WHERE CacheKey = @CacheKey";

        private const string SqlDeleteAll = "DELETE FROM bairong_DbCache";

        private const string ParmCacheKey = "@CacheKey";
        private const string ParmCacheValue = "@CacheValue";
        private const string ParmAddDate = "@AddDate";

        public void RemoveAndInsert(string cacheKey, string cacheValue)
        {
            if (string.IsNullOrEmpty(cacheKey)) return;

            DeleteExcess90Days();

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var removeParams = new IDataParameter[]
                        {
                            GetParameter(ParmCacheKey, DataType.VarChar, 200, cacheKey)
                        };

                        ExecuteNonQuery(trans, SqlDelete, removeParams);

                        var insertParms = new IDataParameter[]
                        {
                            GetParameter(ParmCacheKey, DataType.VarChar, 200, cacheKey),
                            GetParameter(ParmCacheValue, DataType.NVarChar, 500, cacheValue),
                            GetParameter(ParmAddDate, DataType.DateTime, DateTime.Now)
                        };

                        ExecuteNonQuery(trans, SqlInsert, insertParms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Clear()
        {
            ExecuteNonQuery(SqlDeleteAll);
        }

        public bool IsExists(string cacheKey)
        {
            var retval = false;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmCacheKey, DataType.VarChar, 200, cacheKey)
			};

            using (var rdr = ExecuteReader(SqlSelectValue, parms))
            {
                if (rdr.Read())
                {
                    retval = true;
                }
                rdr.Close();
            }
            return retval;
        }

        public string GetValue(string cacheKey)
        {
            var retval = string.Empty;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmCacheKey, DataType.VarChar, 200, cacheKey)
			};

            using (var rdr = ExecuteReader(SqlSelectValue, parms))
            {
                if (rdr.Read())
                {
                    retval = GetString(rdr, 0);
                }
                rdr.Close();
            }
            return retval;
        }

        public string GetValueAndRemove(string cacheKey)
        {
            var retval = string.Empty;

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var parms = new IDataParameter[]
                        {
                            GetParameter(ParmCacheKey, DataType.VarChar, 200, cacheKey)
                        };

                        using (var rdr = ExecuteReader(trans, SqlSelectValue, parms))
                        {
                            if (rdr.Read())
                            {
                                retval = GetString(rdr, 0);
                            }
                            rdr.Close();
                        }

                        var removeParams = new IDataParameter[]
                        {
                            GetParameter(ParmCacheKey, DataType.VarChar, 200, cacheKey)
                        };

                        ExecuteNonQuery(trans, SqlDelete, removeParams);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return retval;
        }

        public int GetCount()
        {
            var count = 0;
            using (var rdr = ExecuteReader(SqlSelectCount))
            {
                if (rdr.Read())
                {
                    count = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return count;
        }

        public void DeleteExcess90Days()
        {
            ExecuteNonQuery("DELETE FROM bairong_DbCache WHERE " + SqlUtils.GetDateDiffGreatThanDays("AddDate", 90.ToString()));
        }
    }
}
