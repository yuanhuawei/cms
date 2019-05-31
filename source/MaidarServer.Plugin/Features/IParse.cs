using System;
using System.Collections.Generic;
using MaiDarServer.Plugin.Models;

namespace MaiDarServer.Plugin.Features
{
    public interface IParse: IPlugin
    {
        Dictionary<string, Func<PluginParseContext, string>> ElementsToParse { get; }
    }
}
