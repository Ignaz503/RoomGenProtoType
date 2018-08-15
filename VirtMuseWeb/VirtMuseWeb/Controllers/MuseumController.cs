using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtMuseWeb.Services;
using VirtMuseWeb.Utility;

namespace VirtMuseWeb.Controllers
{
    [Produces("application/json")]
    [Route("api/museum")]
    public class Museumcontroller : Controller
    {
        private readonly IMuseumService _museumservice;

        public Museumcontroller(IMuseumService _serv)
        {
            _museumservice = _serv;
        }

        [HttpGet, Route("getmuseum")]
        public string getmuseum(string request)
        {
            return _museumservice.GetMuseum(JsonConvert.DeserializeObject<MuseumRequest>(request));
        }
    }
}