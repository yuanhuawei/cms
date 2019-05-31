using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using MaiDar.Core;
using MaiDar.Core.Data;
using MaiDar.Core.Model.Enumerations;
using MaiDarServer.BackgroundPages.Controls;
using MaiDarServer.BackgroundPages.Core;

namespace MaiDarServer.BackgroundPages.Settings
{
	public class PageRecord : BasePage
    {
        public TextBox TbKeyword;
        public DateTimeTextBox TbDateFrom;
        public DateTimeTextBox TbDateTo;

        public Repeater RptContents;
        public SqlPager SpContents;

		public Button BtnDelete;
		public Button BtnDeleteAll;

        protected override bool IsSinglePage => true;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (!MaiDarDataProvider.RecordDao.IsRecord()) return;

            if (Body.IsQueryExists("Delete"))
            {
                var list = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("IDCollection"));
                try
                {
                    MaiDarDataProvider.RecordDao.Delete(list);
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }
            else if (Body.IsQueryExists("DeleteAll"))
            {
                try
                {
                    MaiDarDataProvider.RecordDao.DeleteAll();
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 100;

            SpContents.SelectCommand = !Body.IsQueryExists("Keyword") ? MaiDarDataProvider.RecordDao.GetSelectCommend() : MaiDarDataProvider.RecordDao.GetSelectCommend(Body.GetQueryString("Keyword"), Body.GetQueryString("DateFrom"), Body.GetQueryString("DateTo"));

            SpContents.SortField = "Id";
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

			if(!IsPostBack)
			{
                if (Body.IsQueryExists("Keyword"))
                {
                    TbKeyword.Text = Body.GetQueryString("Keyword");
                    TbDateFrom.Text = Body.GetQueryString("DateFrom");
                    TbDateTo.Text = Body.GetQueryString("DateTo");
                }

                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(PageUtils.GetSettingsUrl(nameof(PageRecord), new NameValueCollection
                {
                    {"Delete", "True" }
                }), "IDCollection", "IDCollection", "请选择需要删除的记录！", "此操作将删除所选记录，确认吗？"));

                BtnDeleteAll.Attributes.Add("onclick", PageUtils.GetRedirectStringWithConfirm(PageUtils.GetSettingsUrl(nameof(PageRecord), new NameValueCollection
                {
                    {"DeleteAll", "True" }
                }), "此操作将删除所有记录信息，确定吗？"));

                SpContents.DataBind();
			}
		}

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                
                var ltlText = (Literal)e.Item.FindControl("ltlText");
                var ltlSummary = (Literal)e.Item.FindControl("ltlSummary");
                var ltlSource = (Literal)e.Item.FindControl("ltlSource");
                var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");

                ltlText.Text = SqlUtils.EvalString(e.Item.DataItem, "Text");
                ltlSummary.Text = SqlUtils.EvalString(e.Item.DataItem, "Summary");
                ltlSource.Text = SqlUtils.EvalString(e.Item.DataItem, "Source");
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(SqlUtils.EvalDateTime(e.Item.DataItem, "AddDate"), EDateFormatType.Day, ETimeFormatType.LongTime);
            }
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(PageUrl, true);
        }

	    private string PageUrl => PageUtils.GetSettingsUrl(nameof(PageRecord), new NameValueCollection
	    {
	        {"Keyword", TbKeyword.Text},
	        {"DateFrom", TbDateFrom.Text},
	        {"DateTo", TbDateTo.Text}
	    });
	}
}
