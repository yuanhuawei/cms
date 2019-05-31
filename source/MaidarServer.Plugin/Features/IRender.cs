using System;
using MaiDarServer.Plugin.Models;

namespace MaiDarServer.Plugin.Features
{
    public interface IRender: IPlugin
    {
        Func<PluginRenderContext, string> Render { get; }


    }
}
