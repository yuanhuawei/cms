using System.Collections.Generic;
using MaiDarServer.CMS.StlParser.Model;

namespace MaiDarServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "����ģ��", Description = "ͨ�� stl:loading ��ǩ��ģ���д�����������ʾ������")]
    public sealed class StlLoading
    {
        public const string ElementName = "stl:loading";

        public static SortedList<string, string> AttributeList => null;
    }
}
