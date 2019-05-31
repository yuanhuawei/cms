﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using MaiDar.Core.Data;
using MaiDar.Core.Model;
using MaiDar.Core.Model.Enumerations;
using MySql.Data.MySqlClient;
using MaiDarServer.Plugin.Models;

namespace MaiDar.Core.Provider
{
    public class DatabaseDao : DataProviderBase
    {
        public virtual void DeleteDbLog()
        {
            if (WebConfigUtils.DatabaseType == EDatabaseType.MySql)
            {
                ExecuteSql("PURGE MASTER LOGS BEFORE DATE_SUB( NOW( ), INTERVAL 3 DAY)");
                return;
            }

            var databaseName = SqlUtils.GetDatabaseNameFormConnectionString(WebConfigUtils.ConnectionString);
            //检测数据库版本
            const string sqlCheck = "SELECT SERVERPROPERTY('productversion')";
            var versions = ExecuteScalar(sqlCheck).ToString();
            //MM.nn.bbbb.rr
            //8 -- 2000
            //9 -- 2005
            //10 -- 2008
            var version = 8;
            var arr = versions.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length > 0)
            {
                version = TranslateUtils.ToInt(arr[0], 8);
            }
            if (version < 10)
            {
                //2000,2005
                string sql = $"BACKUP LOG [{databaseName}] WITH NO_LOG";
                ExecuteNonQuery(sql);
            }
            else
            {
                //2008+
                string sql =
                    $@"ALTER DATABASE [{databaseName}] SET RECOVERY SIMPLE;DBCC shrinkfile ([{databaseName}_log], 1); ALTER DATABASE [{databaseName}] SET RECOVERY FULL; ";
                ExecuteNonQuery(sql);
            }
        }

        public void ExecuteSql(string sqlString)
        {
            if (string.IsNullOrEmpty(sqlString)) return;

            using (var conn = GetConnection())
            {
                conn.Open();
                ExecuteNonQuery(conn, sqlString);
            }
        }

        public void ExecuteSql(List<string> sqlList)
        {
            if (sqlList == null || sqlList.Count <= 0) return;

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var sql in sqlList)
                        {
                            ExecuteNonQuery(trans, sql);
                        }

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

        public void ExecuteSqlInFile(string pathToScriptFile, StringBuilder errorBuilder)
        {
            IDbConnection connection;

            if (false == File.Exists(pathToScriptFile))
            {
                throw new Exception("File " + pathToScriptFile + " does not exists");
            }
            using (Stream stream = File.OpenRead(pathToScriptFile))
            {
                var reader = new StreamReader(stream, Encoding.UTF8);

                connection = GetConnection();

                var command = SqlUtils.GetIDbCommand();

                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;

                string sqlString;
                while (null != (sqlString = SqlUtils.ReadNextSqlString(reader)))
                {
                    command.CommandText = sqlString;
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        errorBuilder.Append($@"
                    sql:{sqlString}
                    message:{ex.Message}
                    ");
                    }
                }

                reader.Close();
            }
            connection.Close();
        }

        public void ExecuteSqlInFile(string pathToScriptFile, string tableName, StringBuilder errorBuilder)
        {
            IDbConnection connection;

            if (false == File.Exists(pathToScriptFile))
            {
                throw new Exception("File " + pathToScriptFile + " does not exists");
            }
            using (Stream stream = File.OpenRead(pathToScriptFile))
            {
                var reader = new StreamReader(stream, Encoding.Default);

                connection = GetConnection();

                var command = SqlUtils.GetIDbCommand();

                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;

                string sqlString;
                while (null != (sqlString = SqlUtils.ReadNextSqlString(reader)))
                {
                    sqlString = string.Format(sqlString, tableName);
                    command.CommandText = sqlString;
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        errorBuilder.Append($@"
                    sql:{sqlString}
                    message:{ex.Message}
                    ");
                    }
                }

                reader.Close();
            }
            connection.Close();
        }

        public int GetIntResult(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            var count = 0;

            using (var conn = GetConnection(connectionString))
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, sqlString))
                {
                    if (rdr.Read())
                    {
                        count = GetInt(rdr, 0);
                    }
                    rdr.Close();
                }
            }
            return count;
        }

        public int GetIntResult(string sqlString)
        {
            var count = 0;

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, sqlString))
                {
                    if (rdr.Read())
                    {
                        count = GetInt(rdr, 0);
                    }
                    rdr.Close();
                }
            }
            return count;
        }

        public int GetIntResult(string sqlString, IDataParameter[] parms)
        {
            var count = 0;

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, sqlString, parms))
                {
                    if (rdr.Read())
                    {
                        count = GetInt(rdr, 0);
                    }
                    rdr.Close();
                }
            }
            return count;
        }

        public List<int> GetIntList(string sqlString)
        {
            var list = new List<int>();

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, sqlString))
                {
                    while (rdr.Read())
                    {
                        list.Add(GetInt(rdr, 0));
                    }
                    rdr.Close();
                }
            }
            return list;
        }

        public List<int> GetIntList(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            var list = new List<int>();

            using (var conn = GetConnection(connectionString))
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, sqlString))
                {
                    while (rdr.Read())
                    {
                        list.Add(GetInt(rdr, 0));
                    }
                    rdr.Close();
                }
            }
            return list;
        }

        public string GetString(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            var retval = string.Empty;

            using (var conn = GetConnection(connectionString))
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, sqlString))
                {
                    if (rdr.Read())
                    {
                        retval = GetString(rdr, 0);
                    }
                    rdr.Close();
                }
            }
            return retval;
        }

        public string GetString(string sqlString)
        {
            var value = string.Empty;
            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    value = GetString(rdr, 0);
                }
                rdr.Close();
            }

            return value;
        }

        public List<string> GetStringList(string sqlString)
        {
            var list = new List<string>();

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, sqlString))
                {
                    while (rdr.Read())
                    {
                        list.Add(GetString(rdr, 0));
                    }
                    rdr.Close();
                }
            }
            return list;
        }

        public DateTime GetDateTime(string sqlString)
        {
            var datetime = DateTime.MinValue;
            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    datetime = GetDateTime(rdr, 0);
                }
                rdr.Close();
            }

            return datetime;
        }

        public DateTime GetDateTime(string sqlString, IDataParameter[] parms)
        {
            var datetime = DateTime.MinValue;
            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    datetime = GetDateTime(rdr, 0);
                }
                rdr.Close();
            }

            return datetime;
        }

        public DataSet GetDataSetByWhereString(string tableEnName, string whereString)
        {
            var sqlSelect = GetSelectSqlString(tableEnName, SqlUtils.Asterisk, whereString);
            var dataset = ExecuteDataset(sqlSelect);
            return dataset;
        }

        public IDataReader GetDataReader(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            return string.IsNullOrEmpty(sqlString) ? null : ExecuteReader(connectionString, sqlString);
        }

        public IEnumerable GetDataSource(string sqlString)
        {
            if (string.IsNullOrEmpty(sqlString)) return null;
            var enumerable = (IEnumerable)ExecuteReader(sqlString);
            return enumerable;
        }

        public IEnumerable GetDataSource(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            if (string.IsNullOrEmpty(sqlString)) return null;
            var enumerable = (IEnumerable)ExecuteReader(connectionString, sqlString);
            return enumerable;
        }

        public DataSet GetDataSet(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            if (string.IsNullOrEmpty(sqlString)) return null;
            var dataset = ExecuteDataset(connectionString, sqlString);
            return dataset;
        }

        public DataSet GetDataSet(string sqlString)
        {
            if (string.IsNullOrEmpty(sqlString)) return null;
            var dataset = ExecuteDataset(sqlString);
            return dataset;
        }

        public void ReadResultsToExtendedAttributes(IDataReader rdr, ExtendedAttributes attributes)
        {
            for (var i = 0; i < rdr.FieldCount; i++)
            {
                var columnName = rdr.GetName(i);
                var value = Convert.ToString(rdr.GetValue(i));
                if (!string.IsNullOrEmpty(value))
                {
                    value = PageUtils.UnFilterSql(value);
                }
                attributes.SetExtendedAttribute(columnName, value);
            }
        }

        public void ReadResultsToNameValueCollection(IDataReader rdr, NameValueCollection attributes)
        {
            for (var i = 0; i < rdr.FieldCount; i++)
            {
                var columnName = rdr.GetName(i);
                var value = Convert.ToString(rdr.GetValue(i));
                if (!string.IsNullOrEmpty(value))
                {
                    value = PageUtils.UnFilterSql(value);
                }
                attributes.Set(columnName, value);
            }
        }

        public int GetPageTotalCount(string sqlString)
        {
            var temp = sqlString.ToLower();
            var pos = temp.LastIndexOf("order by", StringComparison.Ordinal);
            if (pos > -1)
                sqlString = sqlString.Substring(0, pos);

            // Add new ORDER BY info if SortKeyField is specified
            //if (!string.IsNullOrEmpty(sortField) && addCustomSortInfo)
            //    SelectCommand += " ORDER BY " + SortField;

            string cmdText = $"SELECT COUNT(*) FROM ({sqlString}) AS t0";
            return GetIntResult(cmdText);
        }

        public string GetStlPageSqlString(string sqlString, string orderByString, int recordCount, int itemsPerPage, int currentPageIndex)
        {
            var temp = sqlString.ToLower();
            var pos = temp.LastIndexOf("order by", StringComparison.Ordinal);
            if (pos > -1)
                sqlString = sqlString.Substring(0, pos);

            var recordsInLastPage = itemsPerPage;

            // Calculate the correspondent number of pages
            var lastPage = recordCount / itemsPerPage;
            var remainder = recordCount % itemsPerPage;
            if (remainder > 0)
                lastPage++;
            var pageCount = lastPage;

            if (remainder > 0)
                recordsInLastPage = remainder;

            var recsToRetrieve = itemsPerPage;
            if (currentPageIndex == pageCount - 1)
                recsToRetrieve = recordsInLastPage;

            var orderByString2 = orderByString.Replace(" DESC", " DESC2");
            orderByString2 = orderByString2.Replace(" ASC", " DESC");
            orderByString2 = orderByString2.Replace(" DESC2", " ASC");

            if (WebConfigUtils.DatabaseType == EDatabaseType.MySql)
            {
                return $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) AS t0 {orderByString} LIMIT {itemsPerPage * (currentPageIndex + 1)}
    ) AS t1 {orderByString2} LIMIT {recsToRetrieve}
) AS t2 {orderByString}";
            }
            else
            {
                return $@"
SELECT * FROM (
    SELECT TOP {recsToRetrieve} * FROM (
        SELECT TOP {itemsPerPage * (currentPageIndex + 1)} * FROM ({sqlString}) AS t0 {orderByString}
    ) AS t1 {orderByString2}
) AS t2 {orderByString}";
            }
        }

        public void Install(StringBuilder errorBuilder)
        {
            var sqlPath = PathUtils.GetInstallSqlFilePath(WebConfigUtils.DatabaseType);
            MaiDarDataProvider.DatabaseDao.ExecuteSqlInFile(sqlPath, errorBuilder);
            MaiDarDataProvider.TableCollectionDao.CreateAllAuxiliaryTableIfNotExists();
        }

        public void Upgrade(EDatabaseType databaseType, StringBuilder errorBuilder)
        {
            var filePathUpgrade = PathUtils.GetUpgradeSqlFilePath(databaseType, false);
            var filePathUpgradeTable = PathUtils.GetUpgradeSqlFilePath(databaseType, true);

            MaiDarDataProvider.DatabaseDao.ExecuteSqlInFile(filePathUpgrade, errorBuilder);

            if (FileUtils.IsFileExists(filePathUpgradeTable))
            {
                try
                {
                    var tableList = MaiDarDataProvider.TableCollectionDao.GetAuxiliaryTableListCreatedInDb();
                    foreach (var table in tableList)
                    {
                        MaiDarDataProvider.DatabaseDao.ExecuteSqlInFile(filePathUpgradeTable, table.TableEnName, errorBuilder);
                    }
                }
                catch
                {
                    // ignored
                }
            }

            MaiDarDataProvider.TableCollectionDao.CreateAllAuxiliaryTableIfNotExists();
        }

        public bool ConnectToServer(bool isMySql, string connectionStringWithoutDatabaseName, out List<string> databaseNameList, out string errorMessage)
        {
            databaseNameList = new List<string>();
            try
            {
                if (isMySql)
                {
                    var connection = new MySqlConnection(connectionStringWithoutDatabaseName);
                    var command = new MySqlCommand("show databases", connection);

                    connection.Open();

                    var rdr = command.ExecuteReader();

                    while (rdr.Read())
                    {
                        var dbName = rdr.GetString(0);
                        if (dbName == null) continue;
                        if (dbName != "information_schema" &&
                            dbName != "mysql" &&
                            dbName != "performance_schema" &&
                            dbName != "sakila" &&
                            dbName != "sys" &&
                            dbName != "world")
                        {
                            databaseNameList.Add(dbName);
                        }
                    }

                    connection.Close();
                }
                else
                {
                    var connection = new SqlConnection(connectionStringWithoutDatabaseName);
                    var command = new SqlCommand("select name from master..sysdatabases order by name asc", connection);

                    connection.Open();

                    connection.ChangeDatabase("master");

                    var dr = command.ExecuteReader();

                    while (dr.Read())
                    {
                        var dbName = dr["name"] as string;
                        if (dbName == null) continue;
                        if (dbName != "master" &&
                            dbName != "msdb" &&
                            dbName != "tempdb" &&
                            dbName != "model")
                        {
                            databaseNameList.Add(dbName);
                        }
                    }

                    connection.Close();
                }

                errorMessage = "";

                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
        }

        public bool IsTableExists(string tableName)
        {
            bool exists;

            try
            {
                // ANSI SQL way.  Works in PostgreSQL, MSSQL, MySQL.  
                exists = (int)ExecuteScalar($"select case when exists((select * from information_schema.tables where table_name = '{tableName}')) then 1 else 0 end") == 1;
            }
            catch
            {
                try
                {
                    if (WebConfigUtils.DatabaseType == EDatabaseType.MySql)
                    {
                        tableName = $"`{tableName}`";
                    }
                    // Other RDBMS.  Graceful degradation
                    exists = true;
                    ExecuteNonQuery($"select 1 from {tableName} where 1 = 0");
                }
                catch
                {
                    exists = false;
                }
            }

            return exists;
        }

        public void CreatePluginTable(string pluginId, string tableName, List<PluginTableColumn> tableColumns)
        {
            var sqlBuilder = new StringBuilder();

            try
            {
                sqlBuilder.Append($@"CREATE TABLE {tableName} (").AppendLine();

                if (WebConfigUtils.DatabaseType == EDatabaseType.MySql)
                {
                    sqlBuilder.Append(@"Id INT AUTO_INCREMENT,").AppendLine();
                }
                else
                {
                    sqlBuilder.Append(@"Id int IDENTITY (1, 1),").AppendLine();
                }

                foreach (var tableColumn in tableColumns)
                {
                    if (string.IsNullOrEmpty(tableColumn.AttributeName) ||
                        StringUtils.EqualsIgnoreCase(tableColumn.AttributeName, "Id")) continue;

                    var columnSql = SqlUtils.GetColumnSqlString(DataTypeUtils.GetEnumType(tableColumn.DataType), tableColumn.AttributeName,
                        tableColumn.DataLength);
                    if (!string.IsNullOrEmpty(columnSql))
                    {
                        sqlBuilder.Append(columnSql).Append(",").AppendLine();
                    }
                }

                //添加主键及索引
                sqlBuilder.Append(WebConfigUtils.DatabaseType == EDatabaseType.MySql
                    ? @"PRIMARY KEY (Id)"
                    : $@"CONSTRAINT PK_{tableName} PRIMARY KEY (Id)").AppendLine();

                sqlBuilder.Append(WebConfigUtils.DatabaseType == EDatabaseType.MySql
                    ? ") ENGINE=InnoDB DEFAULT CHARSET=utf8"
                    : ")");

                ExecuteNonQuery(sqlBuilder.ToString());
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex, sqlBuilder.ToString());
            }
        }

        public void AlterPluginTable(string pluginId, string tableName, List<PluginTableColumn> tableColumns)
        {
            var columnNameList = GetColumnNameList(tableName, true);
            foreach (var tableColumn in tableColumns)
            {
                if (columnNameList.Contains(tableColumn.AttributeName.ToLower())) continue;

                var columnSqlString = SqlUtils.GetColumnSqlString(DataTypeUtils.GetEnumType(tableColumn.DataType), tableColumn.AttributeName, tableColumn.DataLength);
                var sqlString = SqlUtils.GetAddColumnsSqlString(tableName, columnSqlString);

                try
                {
                    MaiDarDataProvider.DatabaseDao.ExecuteSql(sqlString);
                }
                catch (Exception ex)
                {
                    LogUtils.AddPluginErrorLog(pluginId, ex, sqlString);
                }
            }
        }

        public string GetCreateSystemTableSqlString(string tableName, List<TableColumnInfo> tableColumns)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append($@"CREATE TABLE {tableName} (").AppendLine();

            var primaryKeyColumns = new List<TableColumnInfo>();
            foreach (var tableColumn in tableColumns)
            {
                if (string.IsNullOrEmpty(tableColumn.ColumnName)) continue;

                if (tableColumn.IsIdentity || StringUtils.EqualsIgnoreCase(tableColumn.ColumnName, "Id"))
                {
                    primaryKeyColumns.Add(tableColumn);
                    if (WebConfigUtils.DatabaseType == EDatabaseType.MySql)
                    {
                        sqlBuilder.Append($@"{tableColumn.ColumnName} INT AUTO_INCREMENT,").AppendLine();
                    }
                    else
                    {
                        sqlBuilder.Append($@"{tableColumn.ColumnName} int IDENTITY (1, 1),").AppendLine();
                    }
                }
                else
                {
                    if (tableColumn.IsPrimaryKey)
                    {
                        primaryKeyColumns.Add(tableColumn);
                    }

                    var columnSql = SqlUtils.GetColumnSqlString(tableColumn.DataType, tableColumn.ColumnName,
                    tableColumn.Length);
                    if (!string.IsNullOrEmpty(columnSql))
                    {
                        sqlBuilder.Append(columnSql).Append(",").AppendLine();
                    }
                }
            }

            foreach (var tableColumn in primaryKeyColumns)
            {
                sqlBuilder.Append(WebConfigUtils.DatabaseType == EDatabaseType.MySql
                ? $@"PRIMARY KEY ({tableColumn.ColumnName})"
                : $@"CONSTRAINT PK_{tableName}_{tableColumn.ColumnName} PRIMARY KEY ({tableColumn.ColumnName})").AppendLine();
            }

            sqlBuilder.Append(WebConfigUtils.DatabaseType == EDatabaseType.MySql
                ? ") ENGINE=InnoDB DEFAULT CHARSET=utf8"
                : ")");

            return sqlBuilder.ToString();
        }

        public void CreateSystemTable(string tableName, List<TableColumnInfo> tableColumns)
        {
            try
            {
                var sqlString = GetCreateSystemTableSqlString(tableName, tableColumns);

                ExecuteNonQuery(sqlString);
            }
            catch (Exception ex)
            {
                LogUtils.AddSystemErrorLog(ex, tableName);
            }
        }

        public List<string> GetAlterSystemTableSqlString(string tableName, List<TableColumnInfo> tableColumns)
        {
            var list = new List<string>();

            var columnNameList = GetColumnNameList(tableName, true);
            foreach (var tableColumn in tableColumns)
            {
                if (columnNameList.Contains(tableColumn.ColumnName.ToLower())) continue;

                list.Add(SqlUtils.GetAddColumnsSqlString(tableName, SqlUtils.GetColumnSqlString(tableColumn.DataType, tableColumn.ColumnName, tableColumn.Length)));
            }

            return list;
        }

        public void AlterSystemTable(string tableName, List<TableColumnInfo> tableColumns)
        {
            var alterList = GetAlterSystemTableSqlString(tableName, tableColumns);
            foreach (var sqlString in alterList)
            {
                try
                {
                    MaiDarDataProvider.DatabaseDao.ExecuteSql(sqlString);
                }
                catch (Exception ex)
                {
                    LogUtils.AddSystemErrorLog(ex, sqlString);
                }
            }
        }

        public List<string> GetDatabaseNameList(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            var list = new List<string>();

            if (WebConfigUtils.DatabaseType == EDatabaseType.MySql)
            {
                var connection = new MySqlConnection(connectionString);
                var command = new MySqlCommand("show databases", connection);

                connection.Open();

                var rdr = command.ExecuteReader();

                while (rdr.Read())
                {
                    var dbName = rdr.GetString(0);
                    if (dbName == null) continue;
                    if (dbName != "information_schema" &&
                        dbName != "mysql" &&
                        dbName != "performance_schema" &&
                        dbName != "sakila" &&
                        dbName != "sys" &&
                        dbName != "world")
                    {
                        list.Add(dbName);
                    }
                }

                connection.Close();
            }
            else
            {
                var connection = new SqlConnection(connectionString);
                var command = new SqlCommand("select name from master..sysdatabases order by name asc", connection);

                connection.Open();

                connection.ChangeDatabase("master");

                var dr = command.ExecuteReader();

                while (dr.Read())
                {
                    var dbName = dr["name"] as string;
                    if (dbName == null) continue;
                    if (dbName != "master" &&
                        dbName != "msdb" &&
                        dbName != "tempdb" &&
                        dbName != "model")
                    {
                        list.Add(dbName);
                    }
                }

                connection.Close();
            }

            return list;
        }

        public IDictionary GetTablesAndViewsDictionary(string connectionString, string databaseName)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            string sqlString =
                $"select name, id from [{databaseName}].dbo.sysobjects where type in ('U','V') and category<>2 Order By Name";

            var sortedlist = new SortedList();

            using (var rdr = ExecuteReader(connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    sortedlist.Add(GetString(rdr, 0), GetInt(rdr, 1));
                }
                rdr.Close();
            }
            return sortedlist;
        }

        public string GetTableId(string connectionString, string databaseName, string tableName)
        {
            if (WebConfigUtils.DatabaseType == EDatabaseType.MySql) return tableName;

            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            var tableId = SqlUtils.Cache_GetTableIDCache(databaseName, tableName);

            if (string.IsNullOrEmpty(tableId))
            {
                string sqlString =
                    $"select id from [{databaseName}].dbo.sysobjects where type in ('U','V') and category<>2 and name='{tableName}'";

                using (var rdr = ExecuteReader(connectionString, sqlString))
                {
                    if (rdr.Read())
                    {
                        tableId = GetString(rdr, 0);
                        SqlUtils.Cache_CacheTableID(databaseName, tableName, tableId);
                    }
                    rdr.Close();
                }
            }

            return tableId;
        }

        public string GetTableName(string databaseName, string tableId)
        {
            if (WebConfigUtils.DatabaseType == EDatabaseType.MySql) return tableId;

            var tableName = string.Empty;
            string cmd =
                $"select O.name from [{databaseName}].dbo.sysobjects O, [{databaseName}].dbo.sysusers U where O.id={tableId} and U.uid=O.uid";

            using (var rdr = ExecuteReader(cmd))
            {
                if (rdr.Read())
                {
                    tableName = GetString(rdr, 0);
                }
                rdr.Close();
            }
            return tableName;
        }

        public string GetTableName(string connectionString, string databaseName, string tableId)
        {
            if (WebConfigUtils.DatabaseType == EDatabaseType.MySql) return tableId;

            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            var tableName = string.Empty;
            string sqlString =
                $"select O.name from [{databaseName}].dbo.sysobjects O, [{databaseName}].dbo.sysusers U where O.id={tableId} and U.uid=O.uid";

            using (var rdr = ExecuteReader(connectionString, sqlString))
            {
                if (rdr.Read())
                {
                    tableName = GetString(rdr, 0);
                }
                rdr.Close();
            }
            return tableName;
        }

        public string GetDefaultConstraintName(string tableName, string columnName)
        {
            if (WebConfigUtils.DatabaseType != EDatabaseType.MySql) return string.Empty;

            var defaultConstraintName = string.Empty;
            string sqlString =
                $"select b.name from syscolumns a,sysobjects b where a.id=object_id('{tableName}') and b.id=a.cdefault and a.name='{columnName}' and b.name like 'DF%'";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    defaultConstraintName = GetString(rdr, 0);
                }
                rdr.Close();
            }
            return defaultConstraintName;
        }

        public List<string> GetColumnNameList(string tableName, bool isLower)
        {
            var databaseName = SqlUtils.GetDatabaseNameFormConnectionString(WebConfigUtils.ConnectionString);
            var tableId = GetTableId(WebConfigUtils.ConnectionString, databaseName, tableName);
            var allTableColumnInfoList = GetTableColumnInfoList(databaseName, tableName, tableId);

            var columnNameList = new List<string>();

            foreach (var tableColumnInfo in allTableColumnInfoList)
            {
                columnNameList.Add(isLower ? tableColumnInfo.ColumnName.ToLower() : tableColumnInfo.ColumnName);
            }

            return columnNameList;
        }

        public List<TableColumnInfo> GetTableColumnInfoList(string databaseName, string tableName, string tableId)
        {
            return GetTableColumnInfoList(WebConfigUtils.ConnectionString, databaseName, tableName, tableId);
        }

        public List<TableColumnInfo> GetTableColumnInfoList(string connectionString, string databaseName, string tableName, string tableId)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            var cacheList = SqlUtils.Cache_GetTableColumnInfoListCache(connectionString, databaseName, tableId);
            if (cacheList != null && cacheList.Count > 0)
            {
                return cacheList;
            }

            var list = new List<TableColumnInfo>();
            var isIdentityExist = false;

            if (WebConfigUtils.DatabaseType == EDatabaseType.MySql)
            {
                string sqlString = $"select COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, COLUMN_KEY from information_schema.columns where table_schema = '{databaseName}' and table_name = '{tableName}' order by table_name,ordinal_position; ";
                using (var rdr = ExecuteReader(connectionString, sqlString))
                {
                    while (rdr.Read())
                    {
                        var columnName = Convert.ToString(rdr.GetValue(0));
                        var dataType = DataTypeUtils.FromMySql(Convert.ToString(rdr.GetValue(1)));
                        var length = rdr.IsDBNull(2) || dataType == DataType.NText || dataType == DataType.Text ? 0 : Convert.ToInt32(rdr.GetValue(2));
                        var isPrimaryKey = Convert.ToString(rdr.GetValue(3)) == "PRI";

                        var isIdentity = isPrimaryKey && StringUtils.EqualsIgnoreCase(columnName, "Id");

                        var info = new TableColumnInfo(columnName, dataType, length, isPrimaryKey, isIdentity);
                        list.Add(info);
                    }
                    rdr.Close();
                }
            }
            else
            {
                string sqlString =
                $"select C.name, T.name, C.length, C.colstat, case when C.autoval is null then 0 else 1 end, SC.text, (select CForgin.name from [{databaseName}].dbo.sysreferences Sr,[{databaseName}].dbo.sysobjects O,[{databaseName}].dbo.syscolumns CForgin where Sr.fkeyid={tableId} and Sr.fkey1=C.colid and Sr.rkeyid=O.id and CForgin.id=O.id and CForgin.colid=Sr.rkey1), (select O.name from [{databaseName}].dbo.sysreferences Sr,[{databaseName}].dbo.sysobjects O,[{databaseName}].dbo.syscolumns CForgin where Sr.fkeyid={tableId} and Sr.fkey1=C.colid and Sr.rkeyid=O.id and CForgin.id=O.id and CForgin.colid=Sr.rkey1), (select Sr.rkeyid from [{databaseName}].dbo.sysreferences Sr,[{databaseName}].dbo.sysobjects O,[{databaseName}].dbo.syscolumns CForgin where Sr.fkeyid={tableId} and Sr.fkey1=C.colid and Sr.rkeyid=O.id and CForgin.id=O.id and CForgin.colid=Sr.rkey1) from [{databaseName}].dbo.systypes T, [{databaseName}].dbo.syscolumns C left join [{databaseName}].dbo.syscomments SC on C.cdefault=SC.id where C.id={tableId} and C.xtype=T.xusertype order by C.colid";

                using (var rdr = ExecuteReader(connectionString, sqlString))
                {
                    while (rdr.Read())
                    {
                        var columnName = Convert.ToString(rdr.GetValue(0));
                        if (columnName == "msrepl_tran_version")//sqlserver 发布订阅字段，忽略
                        {
                            continue;
                        }
                        var dataType = DataTypeUtils.FromSqlServer(Convert.ToString(rdr.GetValue(1)));
                        var length = GetDataLength(dataType, Convert.ToInt32(rdr.GetValue(2)));
                        var isPrimaryKeyInt = Convert.ToInt32(rdr.GetValue(3));
                        var isIdentityInt = Convert.ToInt32(rdr.GetValue(4));

                        var isPrimaryKey = isPrimaryKeyInt == 1;
                        var isIdentity = isIdentityInt == 1 || StringUtils.EqualsIgnoreCase(columnName, "Id");
                        //sqlserver 2005 返回isIdentity结果不正确,so 在此假设所有ID字段为Idenity字段
                        if (isIdentity)
                        {
                            isIdentityExist = true;
                        }

                        var info = new TableColumnInfo(columnName, dataType, length, isPrimaryKey, isIdentity);
                        list.Add(info);
                    }
                    rdr.Close();
                }

                if (!isIdentityExist)
                {
                    var sqlIdentity = "select name from syscolumns where id = object_id(N'" + tableName + "') and COLUMNPROPERTY(id, name,'IsIdentity')= 1";
                    var clName = "";
                    using (var rdr = ExecuteReader(sqlIdentity))
                    {
                        if (rdr.Read())
                        {
                            clName = GetString(rdr, 0);
                        }
                        rdr.Close();
                    }

                    foreach (var info in list)
                    {
                        if (clName == info.ColumnName)
                        {
                            info.IsIdentity = true;
                        }
                    }
                }
            }

            SqlUtils.Cache_CacheTableColumnInfoList(connectionString, databaseName, tableId, list);

            return list;
        }

        //lengthFromDb:数据库元数据查询获取的长度
        protected int GetDataLength(DataType dataType, int lengthFromDb)
        {
            if (dataType == DataType.NChar || dataType == DataType.NVarChar)
            {
                return Convert.ToInt32(lengthFromDb / 2);
            }

            return lengthFromDb;
        }

        public bool IsColumnEquals(TableMetadataInfo metadataInfo, TableColumnInfo columnInfo)
        {
            if (!StringUtils.EqualsIgnoreCase(metadataInfo.AttributeName, columnInfo.ColumnName)) return false;
            if (metadataInfo.DataType != columnInfo.DataType) return false;
            if (metadataInfo.DataLength != columnInfo.Length) return false;
            return true;
        }

        public string GetInsertSqlString(NameValueCollection attributes, string tableName, out IDataParameter[] parms)
        {
            return GetInsertSqlString(attributes, ConnectionString, tableName, out parms);
        }

        public string GetInsertSqlString(NameValueCollection attributes, string connectionString, string tableName, out IDataParameter[] parms)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }
            //by 20151030 sofuny 获取自动增长列

            var databaseName = SqlUtils.GetDatabaseNameFormConnectionString(connectionString);
            var tableId = GetTableId(connectionString, databaseName, tableName);
            var allTableColumnInfoList = GetTableColumnInfoList(connectionString, databaseName, tableName, tableId);

            var columnNameList = new List<string>();

            var parameterList = new List<IDataParameter>();

            foreach (var tableColumnInfo in allTableColumnInfoList)
            {
                if (tableColumnInfo.IsIdentity || attributes[tableColumnInfo.ColumnName] == null) continue;

                columnNameList.Add(tableColumnInfo.ColumnName);
                var valueStr = attributes[tableColumnInfo.ColumnName];

                if (tableColumnInfo.DataType == DataType.DateTime)
                {
                    parameterList.Add(GetParameter("@" + tableColumnInfo.ColumnName, tableColumnInfo.DataType, TranslateUtils.ToDateTime(valueStr)));
                }
                else if (tableColumnInfo.DataType == DataType.Integer)
                {
                    parameterList.Add(GetParameter("@" + tableColumnInfo.ColumnName, tableColumnInfo.DataType, TranslateUtils.ToIntWithNagetive(valueStr)));
                }
                else
                {
                    parameterList.Add(GetParameter("@" + tableColumnInfo.ColumnName, tableColumnInfo.DataType, valueStr));
                }
            }

            parms = parameterList.ToArray();

            string returnSqlString =
                $"INSERT INTO {SqlUtils.GetTableName(tableName)} ({TranslateUtils.ObjectCollectionToString(columnNameList, " ,", "[", "]")}) VALUES ({TranslateUtils.ObjectCollectionToString(columnNameList, " ,", "@")})";

            return returnSqlString;
        }

        public string GetUpdateSqlString(NameValueCollection attributes, string tableName, out IDataParameter[] parms)
        {
            return GetUpdateSqlString(attributes, ConnectionString, tableName, out parms);
        }

        public string GetUpdateSqlString(NameValueCollection attributes, string connectionString, string tableName, out IDataParameter[] parms)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            parms = null;
            var databaseName = SqlUtils.GetDatabaseNameFormConnectionString(connectionString);
            var tableId = GetTableId(connectionString, databaseName, tableName);
            var allTableColumnInfoList = GetTableColumnInfoList(connectionString, databaseName, tableName, tableId);

            var setList = new List<string>();
            var whereList = new List<string>();

            var parmsList = new List<IDataParameter>();

            foreach (var tableColumnInfo in allTableColumnInfoList)
            {
                if (attributes[tableColumnInfo.ColumnName] != null)
                {
                    if (!tableColumnInfo.IsPrimaryKey && tableColumnInfo.ColumnName != "ID")
                    {
                        var valueStr = attributes[tableColumnInfo.ColumnName];
                        var sqlValue = SqlUtils.Parse(tableColumnInfo.DataType, valueStr, tableColumnInfo.Length);
                        if (!string.IsNullOrEmpty(sqlValue))
                        {
                            setList.Add($"{tableColumnInfo.ColumnName} = {"@" + tableColumnInfo.ColumnName}");

                            if (tableColumnInfo.DataType == DataType.DateTime)
                            {
                                parmsList.Add(GetParameter("@" + tableColumnInfo.ColumnName, tableColumnInfo.DataType, TranslateUtils.ToDateTime(valueStr)));
                            }
                            else if (tableColumnInfo.DataType == DataType.Integer)
                            {
                                parmsList.Add(GetParameter("@" + tableColumnInfo.ColumnName, tableColumnInfo.DataType, TranslateUtils.ToInt(valueStr)));
                            }
                            else
                            {
                                parmsList.Add(GetParameter("@" + tableColumnInfo.ColumnName, tableColumnInfo.DataType, valueStr));
                            }
                        }

                    }
                    else
                    {
                        var valueStr = attributes[tableColumnInfo.ColumnName];
                        whereList.Add($"{tableColumnInfo.ColumnName} = {"@" + tableColumnInfo.ColumnName}");
                        parmsList.Add(GetParameter("@" + tableColumnInfo.ColumnName, tableColumnInfo.DataType, valueStr));
                    }
                }
            }

            if (whereList.Count == 0 && !string.IsNullOrEmpty(attributes["ID"]))
            {
                whereList.Add("ID = @ID");
            }

            if (whereList.Count == 0)
            {
                throw new MissingPrimaryKeyException();
            }
            if (setList.Count == 0)
            {
                throw new SyntaxErrorException();
            }

            parms = parmsList.ToArray();

            string returnSqlString =
                $"UPDATE {SqlUtils.GetTableName(tableName)} SET {TranslateUtils.ObjectCollectionToString(setList, " ,")} WHERE {TranslateUtils.ObjectCollectionToString(whereList, " AND ")}";

            return returnSqlString;
        }

        public string GetSelectSqlString(string tableName, string columns, string whereString)
        {
            return GetSelectSqlString(tableName, 0, columns, whereString, null);
        }

        public string GetSelectSqlString(string tableName, string columns, string whereString, string orderByString)
        {
            return GetSelectSqlString(tableName, 0, columns, whereString, orderByString);
        }

        public string GetSelectSqlString(string tableName, int totalNum, string columns, string whereString, string orderByString)
        {
            return GetSelectSqlString(ConnectionString, tableName, totalNum, columns, whereString, orderByString);
        }

        public string GetSelectSqlString(string connectionString, string tableName, int totalNum, string columns, string whereString, string orderByString)
        {
            return GetSelectSqlString(ConnectionString, tableName, totalNum, columns, whereString, orderByString, string.Empty);
        }

        public string GetSelectSqlString(string connectionString, string tableName, int totalNum, string columns, string whereString, string orderByString, string joinString)
        {
            if (!string.IsNullOrEmpty(whereString))
            {
                whereString = StringUtils.ReplaceStartsWith(whereString.Trim(), "AND", string.Empty);
                if (!StringUtils.StartsWithIgnoreCase(whereString, "WHERE "))
                {
                    whereString = "WHERE " + whereString;
                }
            }

            if (!string.IsNullOrEmpty(joinString))
            {
                whereString = joinString + " " + whereString;
            }

            return SqlUtils.GetTopSqlString(tableName, columns, whereString + " " + orderByString, totalNum);
        }

        public string GetSelectSqlString(string tableName, int startNum, int totalNum, string columns, string whereString, string orderByString)
        {
            return GetSelectSqlString(ConnectionString, tableName, startNum, totalNum, columns, whereString, orderByString);
        }

        public string GetSelectSqlString(string connectionString, string tableName, int startNum, int totalNum, string columns, string whereString, string orderByString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            if (startNum <= 1)
            {
                return GetSelectSqlString(connectionString, tableName, totalNum, columns, whereString, orderByString);
            }

            string countSqlString = $"SELECT Count(*) FROM {SqlUtils.GetTableName(tableName)} {whereString}";
            var count = MaiDarDataProvider.DatabaseDao.GetIntResult(connectionString, countSqlString);
            if (totalNum == 0)
            {
                totalNum = count;
            }

            if (startNum > count) return string.Empty;

            var topNum = startNum + totalNum - 1;

            if (count < topNum)
            {
                totalNum = count - startNum + 1;
                if (totalNum < 1)
                {
                    return GetSelectSqlString(connectionString, tableName, totalNum, columns, whereString, orderByString);
                }
            }

            var orderByStringOpposite = GetOrderByStringOpposite(orderByString);

            if (WebConfigUtils.DatabaseType == EDatabaseType.MySql)
            {
                return $@"
SELECT {columns} FROM (
    SELECT {columns} FROM (
        SELECT {columns} FROM {SqlUtils.GetTableName(tableName)} {whereString} {orderByString} LIMIT {topNum}
    ) AS tmp {orderByStringOpposite} LIMIT {totalNum}
) AS tmp {orderByString}
";
            }
            return $@"
SELECT {columns}
FROM (SELECT TOP {totalNum} {columns}
        FROM (SELECT TOP {topNum} {columns}
                FROM {SqlUtils.GetTableName(tableName)} {whereString} {orderByString}) tmp
        {orderByStringOpposite}) tmp
{orderByString}
";
        }

        public string GetSelectSqlStringByQueryString(string connectionString, string queryString, int totalNum, string orderByString)
        {
            if (totalNum == 0 && string.IsNullOrEmpty(orderByString))
            {
                return queryString;
            }
            string sqlString;
            if (totalNum > 0)
            {
                //TODO: 当queryString包含top 2语句时排序有问题
                sqlString = WebConfigUtils.DatabaseType == EDatabaseType.MySql ? $"SELECT * FROM ({queryString}) AS tmp {orderByString} LIMIT {totalNum}" : $"SELECT TOP {totalNum} * FROM ({queryString}) tmp {orderByString}";
            }
            else
            {
                sqlString = string.IsNullOrEmpty(orderByString) ? queryString : $"SELECT * FROM ({queryString}) tmp {orderByString}";
            }
            return sqlString;
        }


        public string GetSelectSqlStringByQueryString(string connectionString, string queryString, int startNum, int totalNum, string orderByString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            if (startNum == 1 && totalNum == 0 && string.IsNullOrEmpty(orderByString))
            {
                return queryString;
            }
            //queryString = queryString.Trim().ToUpper();
            if (queryString.LastIndexOf(" ORDER ", StringComparison.Ordinal) != -1)
            {
                if (string.IsNullOrEmpty(orderByString))
                {
                    orderByString = queryString.Substring(queryString.LastIndexOf(" ORDER ", StringComparison.Ordinal) + 1);
                }
                queryString = queryString.Substring(0, queryString.LastIndexOf(" ORDER ", StringComparison.Ordinal));
            }
            orderByString = ParseOrderByString(orderByString);

            if (startNum <= 1)
            {
                return GetSelectSqlStringByQueryString(connectionString, queryString, totalNum, orderByString);
            }

            string countSqlString = $"SELECT Count(*) FROM ({queryString}) tmp";
            var count = MaiDarDataProvider.DatabaseDao.GetIntResult(connectionString, countSqlString);
            if (totalNum == 0)
            {
                totalNum = count;
            }

            if (startNum > count) return string.Empty;

            var topNum = startNum + totalNum - 1;

            if (count < topNum)
            {
                totalNum = count - startNum + 1;
                if (totalNum < 1)
                {
                    return GetSelectSqlStringByQueryString(connectionString, queryString, totalNum, orderByString);
                }
            }

            var orderByStringOpposite = GetOrderByStringOpposite(orderByString);

            if (WebConfigUtils.DatabaseType == EDatabaseType.MySql)
            {
                return $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({queryString}) tmp {orderByString} LIMIT {topNum}
    ) AS tmp {orderByStringOpposite} LIMIT {totalNum}
) AS tmp {orderByString}";
            }
            else
            {
                return $@"
SELECT *
FROM (SELECT TOP {totalNum} *
        FROM (SELECT TOP {topNum} *
                FROM ({queryString}) tmp {orderByString}) tmp
        {orderByStringOpposite}) tmp
{orderByString}
";
            }
        }

        private static string ParseOrderByString(string orderByString)
        {
            if (string.IsNullOrEmpty(orderByString)) return orderByString;

            orderByString = orderByString.ToUpper().Trim();
            if (!orderByString.StartsWith("ORDER BY"))
            {
                orderByString = "ORDER BY " + orderByString;
            }
            if (!orderByString.EndsWith("DESC") && !orderByString.EndsWith("ASC"))
            {
                orderByString = orderByString + " ASC";
            }
            return orderByString;
        }

        private string GetOrderByStringOpposite(string orderByString)
        {
            var retval = string.Empty;
            if (!string.IsNullOrEmpty(orderByString))
            {
                retval = orderByString.Replace(" DESC", " DESC_OPPOSITE").Replace(" ASC", " DESC").Replace(" DESC_OPPOSITE", " ASC");
            }
            return retval;
        }
    }
}
