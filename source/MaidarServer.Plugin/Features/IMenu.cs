using MaiDarServer.Plugin.Models;
using System;

namespace MaiDarServer.Plugin.Features
{
    public interface IMenu : IPlugin
    {
        PluginMenu PluginMenu { get; }

        Func<int, PluginMenu> SiteMenu { get; }
    }
}
