using System.Collections.Generic;
using MaiDarServer.CMS.StlParser.Model;

namespace MaiDarServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "SQL��ѯ���", Description = "ͨ�� stl:queryString ��ǩ��ģ���ж���SQL��ѯ���")]
    public class StlQueryString
	{
        public const string ElementName = "stl:queryString";

        public static SortedList<string, string> AttributeList => null;
    }
}
