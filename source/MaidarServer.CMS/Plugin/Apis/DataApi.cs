﻿using MaiDar.Core;
using MaiDar.Core.Data;
using MaiDar.Core.Model.Enumerations;
using MaiDarServer.Plugin.Apis;
using MaiDarServer.Plugin.Models;

namespace MaiDarServer.CMS.Plugin.Apis
{
    public class DataApi : IDataApi
    {
        private readonly PluginMetadata _metadata;

        public DataApi(PluginMetadata metadata)
        {
            _metadata = metadata;
        }

        private string _databaseType;
        public string DatabaseType
        {
            get
            {
                if (_databaseType != null) return _databaseType;

                _databaseType = EDatabaseTypeUtils.GetValue(WebConfigUtils.DatabaseType);
                if (string.IsNullOrEmpty(_metadata.DatabaseType)) return _databaseType;

                _databaseType = _metadata.DatabaseType;
                if (WebConfigUtils.IsProtectData)
                {
                    _databaseType = TranslateUtils.DecryptStringBySecretKey(_databaseType);
                }
                return _databaseType;
            }
        }

        private string _connectionString;
        public string ConnectionString
        {
            get
            {
                if (_connectionString != null) return _connectionString;

                _connectionString = WebConfigUtils.ConnectionString;
                if (string.IsNullOrEmpty(_metadata.ConnectionString)) return _connectionString;

                _connectionString = _metadata.ConnectionString;
                if (WebConfigUtils.IsProtectData)
                {
                    _connectionString = TranslateUtils.DecryptStringBySecretKey(_connectionString);
                }
                return _connectionString;
            }
        }

        private IDbHelper _dbHelper;
        public IDbHelper DbHelper
        {
            get
            {
                if (_dbHelper != null) return _dbHelper;

                if (EDatabaseTypeUtils.Equals(DatabaseType, EDatabaseType.MySql))
                {
                    _dbHelper = new MySql();
                }
                else
                {
                    _dbHelper = new SqlServer();
                }
                return _dbHelper;
            }
        }

        public string Encrypt(string inputString)
        {
            return TranslateUtils.EncryptStringBySecretKey(inputString);
        }

        public string Decrypt(string inputString)
        {
            return TranslateUtils.DecryptStringBySecretKey(inputString);
        }

        public string FilterXss(string html)
        {
            return PageUtils.FilterXss(html);
        }

        public string FilterSql(string sql)
        {
            return PageUtils.FilterSql(sql);
        }
    }
}
