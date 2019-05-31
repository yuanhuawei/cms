using System;
using System.Web.UI.WebControls;
using MaiDar.Core;
using MaiDar.Core.Model.Enumerations;
using MaiDarServer.CMS.Core;

namespace MaiDarServer.BackgroundPages.Settings
{
	public class PageAuxiliaryTable : BasePageCms
    {
		public DataGrid DgContents;
        public Button BtnAdd;

        public static string GetRedirectUrl()
	    {
	        return PageUtils.GetSettingsUrl(nameof(PageAuxiliaryTable), null);
	    }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (Body.IsQueryExists("Delete"))
			{
                var enName = Body.GetQueryString("ENName");//辅助表
                var enNameArchive = enName + "_Archive";//辅助表归档
			
				try
				{
                    MaiDarDataProvider.TableCollectionDao.Delete(enName);//删除辅助表
                    MaiDarDataProvider.TableCollectionDao.Delete(enNameArchive);//删除辅助表归档

                    Body.AddAdminLog("删除辅助表", $"辅助表:{enName}");

					SuccessDeleteMessage();
				}
				catch(Exception ex)
				{
                    FailDeleteMessage(ex);
				}
			}

            if (IsPostBack) return;

            BreadCrumbSettings("辅助表管理", AppManager.Permissions.Settings.SiteManagement);

            DgContents.DataSource = MaiDarDataProvider.TableCollectionDao.GetDataSourceByAuxiliaryTableType();
            DgContents.DataBind();

            BtnAdd.OnClientClick = $"location.href='{PageUtils.GetLoadingUrl(PageAuxiliaryTableAdd.GetRedirectUrl())}';return false;";
        }

        public string GetYesOrNo(string isDefaultStr)
        {
            return StringUtils.GetBoolText(TranslateUtils.ToBool(isDefaultStr));
        }

        public int GetTableUsedNum(string tableEnName, string auxiliaryTableType)
        {
            var tableType = EAuxiliaryTableTypeUtils.GetEnumType(auxiliaryTableType);
            var usedNum = MaiDarDataProvider.TableCollectionDao.GetTableUsedNum(tableEnName, tableType);
            return usedNum;
        }

        public string GetAuxiliatyTableType(string auxiliaryTableType)
        {
            return EAuxiliaryTableTypeUtils.GetText(EAuxiliaryTableTypeUtils.GetEnumType(auxiliaryTableType));
        }

        public string GetIsChangedAfterCreatedInDb(string isCreatedInDb, string isChangedAfterCreatedInDb)
        {
            return TranslateUtils.ToBool(isCreatedInDb) == false ? "----" : StringUtils.GetBoolText(TranslateUtils.ToBool(isChangedAfterCreatedInDb));
        }

        public string GetFontColor(string isCreatedInDb, string isChangedAfterCreatedInDb)
        {
            if (EBooleanUtils.Equals(EBoolean.False, isCreatedInDb))
            {
                return string.Empty;
            }
            return EBooleanUtils.Equals(EBoolean.False, isChangedAfterCreatedInDb) ? string.Empty : "red";
        }
	}
}
