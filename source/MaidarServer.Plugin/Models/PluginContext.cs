using MaiDarServer.Plugin.Apis;

namespace MaiDarServer.Plugin.Models
{
    public class PluginContext
    {
        public PluginEnvironment Environment { get;  set; }

        public PluginMetadata Metadata { get; set; }

        public IAdminApi AdminApi { get;  set; }

        public IConfigApi ConfigApi { get;  set; }

        public IContentApi ContentApi { get;  set; }

        public IDataApi DataApi { get;  set; }

        public IFilesApi FilesApi { get;  set; }

        public IMenuApi MenuApi { get;  set; }

        public INodeApi NodeApi { get;  set; }

        public IParseApi ParseApi { get;  set; }

        public IPaymentApi PaymentApi { get;  set; }

        public IPublishmentSystemApi PublishmentSystemApi { get;  set; }

        public ISmsApi SmsApi { get;  set; }

        public IUserApi UserApi { get;  set; }
    }
}
