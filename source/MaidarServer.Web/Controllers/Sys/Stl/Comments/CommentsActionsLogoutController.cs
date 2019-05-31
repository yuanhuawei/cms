﻿using System;
using System.Web.Http;
using MaiDarServer.CMS.Controllers.Sys.Stl.Comments;
using MaiDarServer.CMS.Core;

namespace MaiDarServer.API.Controllers.Sys.Stl.Comments
{
    [RoutePrefix("api")]
    public class CommentsActionsLogoutController : ApiController
    {
        [HttpPost, Route(ActionsLogout.Route)]
        public IHttpActionResult Main()
        {
            var body = new RequestBody();

            try
            {
                body.UserLogout();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
