namespace MaiDarServer.Plugin.Apis
{
    public interface IMenuApi
    {
        string GetMenuUrl(string relatedUrl);

        bool IsPluginAuthorized { get; }

        bool IsSiteAuthorized(int publishmentSystemId);
    }
}
