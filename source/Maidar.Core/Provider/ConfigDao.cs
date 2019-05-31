using System;
using System.Collections.Generic;
using System.Data;
using MaiDar.Core.Data;
using MaiDar.Core.Model;
using MaiDar.Core.Model.Enumerations;
using MaiDarServer.Plugin.Models;

namespace MaiDar.Core.Provider
{
    public class ConfigDao : DataProviderBase
	{
        public override string TableName => "bairong_Config";

        public override List<TableColumnInfo> TableColumns => new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = nameof(ConfigInfo.IsInitialized),
                DataType = DataType.VarChar,
                Length = 18
            },
            new TableColumnInfo
            {
                ColumnName = nameof(ConfigInfo.DatabaseVersion),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(ConfigInfo.UpdateDate),
                DataType = DataType.DateTime
            },
            new TableColumnInfo
            {
                ColumnName = nameof(ConfigInfo.SystemConfig),
                DataType = DataType.NText
            }
        };

        private const string SqlInsertConfig = "INSERT INTO bairong_Config (IsInitialized, DatabaseVersion, UpdateDate, SystemConfig) VALUES (@IsInitialized, @DatabaseVersion, @UpdateDate, @SystemConfig)";

        private const string SqlSelectConfig = "SELECT IsInitialized, DatabaseVersion, UpdateDate, SystemConfig FROM bairong_Config";

        private const string SqlSelectIsInitialized = "SELECT IsInitialized FROM bairong_Config";

		private const string SqlSelectDatabaseVersion = "SELECT DatabaseVersion FROM bairong_Config";

        private const string SqlUpdateConfig = "UPDATE bairong_Config SET IsInitialized = @IsInitialized, DatabaseVersion = @DatabaseVersion, UpdateDate = @UpdateDate, SystemConfig = @SystemConfig";

		private const string ParmIsInitialized = "@IsInitialized";
		private const string ParmDatabaseVersion = "@DatabaseVersion";
        private const string ParmUpdateDate = "@UpdateDate";
        private const string ParmSystemConfig = "@SystemConfig";

        public void Insert(ConfigInfo info) 
		{
			var insertParms = new IDataParameter[]
			{
				GetParameter(ParmIsInitialized, DataType.VarChar, 18, info.IsInitialized.ToString()),
				GetParameter(ParmDatabaseVersion, DataType.VarChar, 50, info.DatabaseVersion),
                GetParameter(ParmUpdateDate, DataType.DateTime, info.UpdateDate),
                GetParameter(ParmSystemConfig, DataType.NText, info.SystemConfigInfo.ToString())
            };

            ExecuteNonQuery(SqlInsertConfig, insertParms);
            ConfigManager.IsChanged = true;
		}

		public void Update(ConfigInfo info) 
		{
			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmIsInitialized, DataType.VarChar, 18, info.IsInitialized.ToString()),
				GetParameter(ParmDatabaseVersion, DataType.VarChar, 50, info.DatabaseVersion),
                GetParameter(ParmUpdateDate, DataType.DateTime, info.UpdateDate),
                GetParameter(ParmSystemConfig, DataType.NText, info.SystemConfigInfo.ToString())
            };

            ExecuteNonQuery(SqlUpdateConfig, updateParms);
            ConfigManager.IsChanged = true;
		}

		public bool IsInitialized()
		{
            var isInitialized = false;

			try
			{
                using (var rdr = ExecuteReader(SqlSelectIsInitialized)) 
				{
					if (rdr.Read()) 
					{
                        isInitialized = GetBool(rdr, 0);
					}
					rdr.Close();
				}
			}
		    catch
		    {
		        // ignored
		    }

		    return isInitialized;
		}

		public string GetDatabaseVersion()
		{
			var databaseVersion = string.Empty;

			try
			{
				using (var rdr = ExecuteReader(SqlSelectDatabaseVersion)) 
				{
					if (rdr.Read()) 
					{
                        databaseVersion = GetString(rdr, 0);
					}
					rdr.Close();
				}
			}
		    catch
		    {
		        // ignored
		    }

		    return databaseVersion;
		}

		public ConfigInfo GetConfigInfo()
		{
		    var info = new ConfigInfo
		    {
		        IsInitialized = false
		    };

		    using (var rdr = ExecuteReader(SqlSelectConfig))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    info = new ConfigInfo(GetBool(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

			return info;
		}

        public string GetGuid(string key)
        {
            key = "guid_" + key;

            var guid = ConfigManager.SystemConfigInfo.GetExtendedAttribute(key);
            if (!string.IsNullOrEmpty(guid)) return guid;

            guid = StringUtils.GetShortGuid();
            ConfigManager.SystemConfigInfo.SetExtendedAttribute(key, guid);
            MaiDarDataProvider.ConfigDao.Update(ConfigManager.Instance);
            return guid;
        }

        public void InitializeConfig()
        {
            var configInfo = new ConfigInfo(true, AppManager.Version, DateTime.Now, string.Empty);
            Insert(configInfo);
        }

        public void InitializeUserRole(string userName, string password)
        {
            RoleManager.CreatePredefinedRoles();

            var administratorInfo = new AdministratorInfo
            {
                UserName = userName,
                Password = password
            };

            string errorMessage;
            AdminManager.CreateAdministrator(administratorInfo, out errorMessage);
            MaiDarDataProvider.AdministratorsInRolesDao.AddUserToRole(userName, EPredefinedRoleUtils.GetValue(EPredefinedRole.ConsoleAdministrator));
        }
    }
}
