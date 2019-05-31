namespace MaiDar.Core
{
	public class CacheDbUtils
	{
		private CacheDbUtils()
		{
		}

		public static void RemoveAndInsert(string cacheKey, string cacheValue)
		{
			if (!string.IsNullOrEmpty(cacheKey))
			{
				MaiDarDataProvider.DbCacheDao.RemoveAndInsert(cacheKey, cacheValue);
			}
		}

        public static void Clear()
        {
            MaiDarDataProvider.DbCacheDao.Clear();
        }

		public static bool IsExists(string cacheKey)
		{
            return MaiDarDataProvider.DbCacheDao.IsExists(cacheKey);
		}

        public static string GetValue(string cacheKey)
        {
            return MaiDarDataProvider.DbCacheDao.GetValue(cacheKey);
        }

		public static string GetValueAndRemove(string cacheKey)
		{
            return MaiDarDataProvider.DbCacheDao.GetValueAndRemove(cacheKey);
		}

        public static int GetCount()
        {
            return MaiDarDataProvider.DbCacheDao.GetCount();
        }

	}
}
