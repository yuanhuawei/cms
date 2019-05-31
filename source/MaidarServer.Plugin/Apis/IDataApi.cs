﻿
using MaiDarServer.Plugin.Models;

namespace MaiDarServer.Plugin.Apis
{
    public interface IDataApi
    {
        string DatabaseType { get; }

        string ConnectionString { get; }

        IDbHelper DbHelper { get; }

        string Encrypt(string inputString);

        string Decrypt(string inputString);

        string FilterXss(string html);

        string FilterSql(string sql);
    }
}
