using MaiDar.Core;

namespace MaiDarServer.CMS.StlParser.Cache
{
    public class TableStructure
    {
        private static readonly object LockObject = new object();

        public static string GetSelectSqlStringByQueryString(string connectionString, string queryString, int startNum,
            int totalNum, string orderByString)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(TableStructure),
                       nameof(GetSelectSqlStringByQueryString), connectionString, queryString, startNum.ToString(),
                       totalNum.ToString(), orderByString);
            var retval = StlCacheUtils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<string>(cacheKey);
                if (retval == null)
                {
                    retval = MaiDarDataProvider.DatabaseDao.GetSelectSqlStringByQueryString(connectionString,
                    queryString, startNum, totalNum, orderByString);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
