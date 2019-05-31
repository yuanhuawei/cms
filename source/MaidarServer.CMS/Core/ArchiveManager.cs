using MaiDar.Core;
using MaiDar.Core.AuxiliaryTable;
using MaiDarServer.CMS.Model;

namespace MaiDarServer.CMS.Core
{
	public class ArchiveManager
	{
        public static void CreateArchiveTableIfNotExists(PublishmentSystemInfo publishmentSystemInfo, string tableName)
        {
            if (!MaiDarDataProvider.DatabaseDao.IsTableExists(TableManager.GetTableNameOfArchive(tableName)))
            {
                try
                {
                    MaiDarDataProvider.TableMetadataDao.CreateAuxiliaryTableOfArchive(tableName);
                }
                catch
                {
                    // ignored
                }
            }
        }
	}
}
