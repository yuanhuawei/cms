﻿using System.Collections.Generic;
using MaiDar.Core;
using MaiDarServer.CMS.Core;
using MaiDarServer.Plugin.Apis;
using MaiDarServer.Plugin.Models;

namespace MaiDarServer.CMS.Plugin.Apis
{
    public class FilesApi : IFilesApi
    {
        private readonly PluginMetadata _metadata;

        public FilesApi(PluginMetadata metadata)
        {
            _metadata = metadata;
        }

        public void MoveFiles(int sourcePublishmentSystemId, int targetPublishmentSystemId, List<string> relatedUrls)
        {
            if (sourcePublishmentSystemId == targetPublishmentSystemId) return;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(sourcePublishmentSystemId);
            var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);
            if (publishmentSystemInfo == null || targetPublishmentSystemInfo == null) return;

            foreach (var relatedUrl in relatedUrls)
            {
                if (!string.IsNullOrEmpty(relatedUrl) && !PageUtils.IsProtocolUrl(relatedUrl))
                {
                    FileUtility.MoveFile(publishmentSystemInfo, targetPublishmentSystemInfo, relatedUrl);
                }
            }
        }

        public void AddWaterMark(int publishmentSystemId, string filePath)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            FileUtility.AddWaterMark(publishmentSystemInfo, filePath);
        }

        public string GetUploadFilePath(int publishmentSystemId, string relatedPath)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var localDirectoryPath = PathUtility.GetUploadDirectoryPath(publishmentSystemInfo, PathUtils.GetExtension(relatedPath));
            var localFileName = PathUtility.GetUploadFileName(publishmentSystemInfo, relatedPath);
            return PathUtils.Combine(localDirectoryPath, localFileName);
        }

        public string GetTemporaryFilesPath(string relatedPath)
        {
            return PathUtils.GetTemporaryFilesPath(relatedPath);
        }

        public string GetPluginPath(string relatedPath)
        {
            return PathUtils.Combine(_metadata.DirectoryPath, relatedPath);
        }

        public string GetPluginUrl(string relatedUrl)
        {
            return PageUtility.GetSiteFilesUrl(PageUtils.OuterApiUrl, PageUtils.Combine(DirectoryUtils.SiteFiles.Plugins, _metadata.Id, relatedUrl));
        }

        public string GetApiJsonUrl(string name = "", string id = "")
        {
            return Controllers.Json.PluginJsonApi.GetUrl(PageUtils.OuterApiUrl, _metadata.Id, name, id);
        }

        public string GetApiHttpUrl(string name = "", string id = "")
        {
            return Controllers.Http.PluginHttpApi.GetUrl(PageUtils.OuterApiUrl, _metadata.Id, name, id);
        }

        public string GetHomeUrl(string relatedUrl = "")
        {
            return HomeUtils.GetUrl(PageUtils.OuterApiUrl, relatedUrl);
        }

        public string GetHomeLoginUrl(string returnUrl)
        {
            return HomeUtils.GetLoginUrl(PageUtils.OuterApiUrl, returnUrl);
        }

        public string GetHomeLogoutUrl(string returnUrl)
        {
            return HomeUtils.GetLogoutUrl(PageUtils.OuterApiUrl, returnUrl);
        }

        public string GetHomeRegisterUrl(string returnUrl)
        {
            return HomeUtils.GetRegisterUrl(PageUtils.OuterApiUrl, returnUrl);
        }

        public string GetPublishmentSystemUrl(int publishmentSystemId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            return PageUtility.GetPublishmentSystemUrl(publishmentSystemInfo, string.Empty, false);
        }

        public string GetPublishmentSystemUrlByFilePath(string filePath)
        {
            var publishmentSystemId = PublishmentSystemApi.Instance.GetPublishmentSystemIdByFilePath(filePath);
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            return PageUtility.GetPublishmentSystemUrlByPhysicalPath(publishmentSystemInfo, filePath);
        }

        public string GetRootUrl(string relatedUrl)
        {
            return PageUtils.GetRootUrl(relatedUrl);
        }

        public string GetChannelUrl(int publishmentSystemId, int channelId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            return PageUtility.GetChannelUrl(publishmentSystemInfo, NodeManager.GetNodeInfo(publishmentSystemId, channelId), false);
        }

        public string GetContentUrl(int publishmentSystemId, int channelId, int contentId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            return PageUtility.GetContentUrl(publishmentSystemInfo, NodeManager.GetNodeInfo(publishmentSystemId, channelId), contentId, false);
        }
    }
}
