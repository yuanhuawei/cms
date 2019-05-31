using System;
using System.Web.Http;
using MaiDar.Core;
using MaiDarServer.CMS.Controllers.Sys.Stl;
using MaiDarServer.CMS.Core;
using MaiDarServer.CMS.StlParser.Model;
using MaiDarServer.CMS.StlParser.StlElement;

namespace MaiDarServer.API.Controllers.Sys.Stl
{
    [RoutePrefix("api")]
    public class StlActionsPageContentsController : ApiController
    {
        [HttpPost, Route(ActionsPageContents.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var body = new RequestBody();

                var publishmentSystemId = body.GetPostInt("publishmentSystemId");
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                var pageNodeId = body.GetPostInt("pageNodeId");
                var templateId = body.GetPostInt("templateId");
                var totalNum = body.GetPostInt("totalNum");
                var pageCount = body.GetPostInt("pageCount");
                var currentPageIndex = body.GetPostInt("currentPageIndex", 0);
                var stlPageContentsElement = TranslateUtils.DecryptStringBySecretKey(body.GetPostString("stlPageContentsElement"));

                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, pageNodeId);
                var templateInfo = TemplateManager.GetTemplateInfo(publishmentSystemId, templateId);
                var pageInfo = new PageInfo(nodeInfo.NodeId, 0, publishmentSystemInfo, templateInfo, body.UserInfo);
                var contextInfo = new ContextInfo(pageInfo);

                var stlPageContents = new StlPageContents(stlPageContentsElement, pageInfo, contextInfo, false);

                var pageHtml = stlPageContents.Parse(totalNum, currentPageIndex, pageCount, false);

                return Ok(pageHtml);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
