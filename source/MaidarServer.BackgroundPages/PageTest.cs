using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using MaiDar.Core;

namespace MaiDarServer.BackgroundPages
{
    public class PageTest : Page
    {
        public Literal LtlContent;

        public void Page_Load(object sender, EventArgs e)
        {
            LtlContent.Text = string.Empty;

            var sqlCreate = MaiDarDataProvider.DatabaseDao.GetCreateSystemTableSqlString(MaiDarDataProvider.ContentCheckDao.TableName, MaiDarDataProvider.ContentCheckDao.TableColumns);

            var sqlAlert = MaiDarDataProvider.DatabaseDao.GetAlterSystemTableSqlString(MaiDarDataProvider.ContentCheckDao.TableName, MaiDarDataProvider.ContentCheckDao.TableColumns);

            LtlContent.Text = sqlCreate + "<br /><hr /></br />" + string.Join("<br />", sqlAlert);
        }
    }
}
