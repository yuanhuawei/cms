using System.Collections.Generic;
using MaiDarServer.Plugin.Models;

namespace MaiDarServer.Plugin.Features
{
    public interface IContentTable: IPlugin
    {
        string ContentTableName { get; }
        List<PluginTableColumn> ContentTableColumns { get; }
    }
}
