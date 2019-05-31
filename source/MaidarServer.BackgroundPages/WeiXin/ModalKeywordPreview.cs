using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using MaiDar.Core;
using MaiDarServer.CMS.WeiXin.Data;

namespace MaiDarServer.BackgroundPages.WeiXin
{
    public class ModalKeywordPreview : BasePageCms
    {
        public TextBox TbWeiXin;

        private int _keywordId;

        public static string GetOpenWindowString(int publishmentSystemId, int keywordId)
        {
            return PageUtils.GetOpenWindowString("预览",
                PageUtils.GetWeiXinUrl(nameof(ModalKeywordPreview), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"keywordId", keywordId.ToString()}
                }), 400, 300);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _keywordId = Body.GetQueryInt("keywordID");

			if (!IsPostBack)
			{
                //this.tbWeiXin.Text = keywordInfo.Keywords;
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                var keywordInfo = DataProviderWx.KeywordDao.GetKeywordInfo(_keywordId);

                SuccessMessage("发送预览成功，请留意您的手机微信提醒");

                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "失败：" + ex.Message);
            }

            if (isChanged)
            {
                PageUtils.CloseModalPage(Page);
            }
		}
	}
}
