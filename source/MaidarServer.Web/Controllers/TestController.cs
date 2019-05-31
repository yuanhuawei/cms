using System;
using System.Web.Http;

namespace MaiDarServer.API.Controllers
{
    [RoutePrefix("api")]
    public class TestController : ApiController
    {
        [HttpGet, Route("test")]
        public IHttpActionResult Get()
        {
            return Ok(new
            {
                DateTime = DateTime.Now
            });
        }
    }
}
