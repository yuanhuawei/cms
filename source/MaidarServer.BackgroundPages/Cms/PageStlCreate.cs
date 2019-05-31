using System;

namespace MaiDarServer.BackgroundPages.Cms
{
	public class PageStlCreate : BasePageCms
    {
		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (!IsPostBack)
			{
				
			}
		}
	}
}
