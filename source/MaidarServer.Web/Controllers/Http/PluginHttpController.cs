﻿using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MaiDar.Core;
using MaiDarServer.CMS.Controllers.Http;
using MaiDarServer.CMS.Core;
using MaiDarServer.CMS.Plugin;
using MaiDarServer.Plugin.Features;

namespace MaiDarServer.API.Controllers.Http
{
    [RoutePrefix("api")]
    public class PluginHttpController : ApiController
    {
        [HttpGet, Route(PluginHttpApi.Route)]
        public HttpResponseMessage Get(string pluginId)
        {
            try
            {
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                return webApi?.HttpGet == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : webApi.HttpGet.Invoke(body);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet, Route(PluginHttpApi.RouteName)]
        public HttpResponseMessage Get(string pluginId, string name)
        {
            try
            {
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                return webApi?.HttpGetWithName == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : webApi.HttpGetWithName.Invoke(body, name);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet, Route(PluginHttpApi.RouteNameAndId)]
        public HttpResponseMessage Get(string pluginId, string name, string id)
        {
            try
            {
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                return webApi?.HttpGetWithNameAndId == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : webApi.HttpGetWithNameAndId.Invoke(body, name, id);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost, Route(PluginHttpApi.Route)]
        public HttpResponseMessage Post(string pluginId)
        {
            try
            {
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                return webApi?.HttpPost == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : webApi.HttpPost.Invoke(body);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost, Route(PluginHttpApi.RouteName)]
        public HttpResponseMessage Post(string pluginId, string name)
        {
            try
            {
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                return webApi?.HttpPostWithName == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : webApi.HttpPostWithName.Invoke(body, name);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost, Route(PluginHttpApi.RouteNameAndId)]
        public HttpResponseMessage Post(string pluginId, string name, string id)
        {
            try
            {
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                return webApi?.HttpPostWithNameAndId == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : webApi.HttpPostWithNameAndId.Invoke(body, name, id);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPut, Route(PluginHttpApi.Route)]
        public HttpResponseMessage Put(string pluginId)
        {
            try
            {
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                return webApi?.HttpPut == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : webApi.HttpPut.Invoke(body);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPut, Route(PluginHttpApi.RouteName)]
        public HttpResponseMessage Put(string pluginId, string name)
        {
            try
            {
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                return webApi?.HttpPutWithName == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : webApi.HttpPutWithName.Invoke(body, name);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPut, Route(PluginHttpApi.RouteNameAndId)]
        public HttpResponseMessage Put(string pluginId, string name, string id)
        {
            try
            {
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                return webApi?.HttpPutWithNameAndId == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : webApi.HttpPutWithNameAndId.Invoke(body, name, id);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpDelete, Route(PluginHttpApi.Route)]
        public HttpResponseMessage Delete(string pluginId)
        {
            try
            {
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                return webApi?.HttpDelete == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : webApi.HttpDelete.Invoke(body);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpDelete, Route(PluginHttpApi.RouteName)]
        public HttpResponseMessage Delete(string pluginId, string name)
        {
            try
            {
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                return webApi?.HttpDeleteWithName == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : webApi.HttpDeleteWithName.Invoke(body, name);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpDelete, Route(PluginHttpApi.RouteNameAndId)]
        public HttpResponseMessage Delete(string pluginId, string name, string id)
        {
            try
            {
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                return webApi?.HttpDeleteWithNameAndId == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : webApi.HttpDeleteWithNameAndId.Invoke(body, name, id);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPatch, Route(PluginHttpApi.Route)]
        public HttpResponseMessage Patch(string pluginId)
        {
            try
            {
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                return webApi?.HttpPatch == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : webApi.HttpPatch.Invoke(body);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPatch, Route(PluginHttpApi.RouteName)]
        public HttpResponseMessage Patch(string pluginId, string name)
        {
            try
            {
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                return webApi?.HttpPatchWithName == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : webApi.HttpPatchWithName.Invoke(body, name);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPatch, Route(PluginHttpApi.RouteNameAndId)]
        public HttpResponseMessage Patch(string pluginId, string name, string id)
        {
            try
            {
                var body = new RequestBody();
                var webApi = PluginCache.GetEnabledFeature<IWebApi>(pluginId);

                return webApi?.HttpPatchWithNameAndId == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound)
                    : webApi.HttpPatchWithNameAndId.Invoke(body, name, id);
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}
