using System.Collections.Generic;

namespace MaiDarServer.Plugin.Models
{
    public class PluginMenu
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string IconClass { get; set; }
        public string Href { get; set; }
        public string Target { get; set; }
        public List<PluginMenu> Menus { get; set; }
    }
}
