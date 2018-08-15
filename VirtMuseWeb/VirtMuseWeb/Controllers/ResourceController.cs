using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGeneration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtMuseWeb.Models;
using VirtMuseWeb.Services;

namespace VirtMuseWeb.Controllers
{
    [Produces("application/json")]
    [Route("api/resource")]
    public class ResourceController : Controller
    {
        private readonly IResourceService _resourceService;

        public ResourceController(IResourceService service)
        {
            _resourceService = service;
        }

        [HttpGet, Route("getres")]
        public ResourceModel GetResource(int id)
        {
            return _resourceService.GetResource(id);
        }

        [HttpPost, Route("postresource")]
        public async Task<StatusCodeResult> PostResource([FromBody]JObject res)
        {
            if(res== null)
            {
                return BadRequest() ;
            }

            await _resourceService.PostResource(res.ToObject<Resource<byte>>());
            return Ok();
        }
    }
}