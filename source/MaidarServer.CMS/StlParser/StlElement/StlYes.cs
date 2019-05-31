using System.Collections.Generic;
using MaiDarServer.CMS.StlParser.Model;

namespace MaiDarServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "�ɹ�ģ��", Description = "ͨ�� stl:yes ��ǩ��ģ������ʾ�ɹ�ģ��")]
    public sealed class StlYes
    {
        public const string ElementName = "stl:yes";
        public const string ElementName2 = "stl:successtemplate";

        public static SortedList<string, string> AttributeList => null;
    }
}
