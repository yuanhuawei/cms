using System.Collections.Generic;
using MaiDarServer.Plugin.Models;

namespace MaiDarServer.Plugin.Features
{
    public interface ITable : IPlugin
    {
        Dictionary<string, List<PluginTableColumn>> Tables { get; }
    }
}
