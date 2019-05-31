using MaiDar.Core.Model.Enumerations;

namespace MaiDar.Core
{
	public class CountManager
	{
		private CountManager()
		{
		}

		public static void AddCount(string relatedTableName, string relatedIdentity, ECountType countType)
		{
            if (MaiDarDataProvider.CountDao.IsExists(relatedTableName, relatedIdentity, countType))
			{
				MaiDarDataProvider.CountDao.AddCountNum(relatedTableName, relatedIdentity, countType);
			}
			else
			{
                MaiDarDataProvider.CountDao.Insert(relatedTableName, relatedIdentity, countType, 1);
			}
		}

		public static void DeleteByRelatedTableName(string relatedTableName)
		{
            MaiDarDataProvider.CountDao.DeleteByRelatedTableName(relatedTableName);
		}

		public static void DeleteByIdentity(string relatedTableName, string relatedIdentity)
		{
            MaiDarDataProvider.CountDao.DeleteByIdentity(relatedTableName, relatedIdentity);
		}

        public static int GetCount(string relatedTableName, string relatedIdentity, ECountType countType)
        {
            return MaiDarDataProvider.CountDao.GetCountNum(relatedTableName, relatedIdentity, countType);
        }

        public static int GetCount(string relatedTableName, int publishmentSystemId, ECountType countType)
        {
            return MaiDarDataProvider.CountDao.GetCountNum(relatedTableName, publishmentSystemId, countType);
        }
	}
}
