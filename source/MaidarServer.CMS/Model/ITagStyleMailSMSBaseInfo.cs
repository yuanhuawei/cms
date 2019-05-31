using MaiDar.Core.Model.Enumerations;

namespace MaiDarServer.CMS.Model
{
    public interface ITagStyleMailSMSBaseInfo
    {
        bool IsSMS { get; set; }

        ETriState SMSReceiver { get; set; }

        string SMSTo { get; set; }

        string SMSFiledName { get; set; }

        bool IsSMSTemplate { get; set; }

        string SMSContent { get; set; }
    }
}
