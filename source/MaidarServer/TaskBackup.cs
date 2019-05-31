using System;
using System.Collections.Generic;
using MaiDar.Core;
using MaiDarServer.CMS.Core;
using MaiDarServer.CMS.ImportExport;
using MaiDarServer.CMS.Model;
using MaiDarServer.CMS.Model.Enumerations;

namespace MaiDarServer
{
    public class TaskBackup
    {
        public static bool Execute(TaskInfo taskInfo)
        {
            var taskBackupInfo = new TaskBackupInfo(taskInfo.ServiceParameters);

            if (taskInfo.PublishmentSystemID != 0)
            {
                return BackupByPublishmentSystemID(taskInfo, taskInfo.PublishmentSystemID, taskBackupInfo.BackupType);
            }
            else
            {
                List<int> publishmentSystemIDArrayList = null;
                if (taskBackupInfo.IsBackupAll)
                {
                    publishmentSystemIDArrayList = PublishmentSystemManager.GetPublishmentSystemIdList();
                }
                else
                {
                    publishmentSystemIDArrayList = TranslateUtils.StringCollectionToIntList(taskBackupInfo.PublishmentSystemIDCollection);
                }
                foreach (int publishmentSystemID in publishmentSystemIDArrayList)
                {
                    BackupByPublishmentSystemID(taskInfo, publishmentSystemID, taskBackupInfo.BackupType);
                }
            }

            return true;
        }

        private static bool BackupByPublishmentSystemID(TaskInfo taskInfo, int publishmentSystemID, EBackupType backupType)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemID);
            if (publishmentSystemInfo == null)
            {
                ExecutionUtils.LogError(taskInfo, new Exception("无法找到对应站点"));
                return false;
            }

            var filePath = PathUtility.GetBackupFilePath(publishmentSystemInfo, backupType);
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            FileUtils.DeleteFileIfExists(filePath);

            if (backupType == EBackupType.Templates)
            {
                BackupUtility.BackupTemplates(publishmentSystemInfo.PublishmentSystemId, filePath);
            }
            else if (backupType == EBackupType.ChannelsAndContents)
            {
                BackupUtility.BackupChannelsAndContents(publishmentSystemInfo.PublishmentSystemId, filePath);
            }
            else if (backupType == EBackupType.Files)
            {
                BackupUtility.BackupFiles(publishmentSystemInfo.PublishmentSystemId, filePath);
            }
            else if (backupType == EBackupType.Site)
            {
                BackupUtility.BackupSite(publishmentSystemInfo.PublishmentSystemId, filePath);
            }

            return true;
        }
    }
}
