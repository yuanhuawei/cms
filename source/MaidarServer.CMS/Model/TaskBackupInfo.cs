using MaiDar.Core;
using MaiDar.Core.Model;
using MaiDarServer.CMS.Model.Enumerations;
using MaiDarServer.Plugin.Models;

namespace MaiDarServer.CMS.Model
{
	public class TaskBackupInfo : ExtendedAttributes
	{
        public TaskBackupInfo(string serviceParameters)
        {
            var nameValueCollection = TranslateUtils.ToNameValueCollection(serviceParameters);
            SetExtendedAttribute(nameValueCollection);
        }

        public EBackupType BackupType
		{
            get { return EBackupTypeUtils.GetEnumType(GetExtendedAttribute("BackupType")); }
            set { SetExtendedAttribute("BackupType", EBackupTypeUtils.GetValue(value)); }
		}

        public string PublishmentSystemIDCollection
        {
            get { return GetExtendedAttribute("PublishmentSystemIDCollection"); }
            set { SetExtendedAttribute("PublishmentSystemIDCollection", value); }
        }

        public bool IsBackupAll
        {
            get { return GetBool("IsBackupAll", false); }
            set { SetExtendedAttribute("IsBackupAll", value.ToString()); }
        }
	}
}
