﻿using MaiDarServer.Plugin.Models;

namespace MaiDarServer.Plugin.Apis
{
    public interface IConfigApi
    {
        bool SetConfig(int publishmentSystemId, string name, object config);

        bool SetConfig(int publishmentSystemId, object config);

        T GetConfig<T>(int publishmentSystemId, string name = "");

        bool RemoveConfig(int publishmentSystemId, string name = "");

        string PhysicalApplicationPath { get; }

        string AdminDirectory { get; }

        ISystemConfigInfo SystemConfigInfo { get; }
    }
}
