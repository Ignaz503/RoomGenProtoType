using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtMuseWeb.Models;
using VirtMuseWeb.Services;

namespace KnockoutTS.Controllers
{
    [Route("api/[controller]")]
    public class MailController : Controller
    {
        private readonly IMailService _mailService;

        public MailController(IMailService mailService)
        {
            _mailService = mailService;
        }

        [HttpGet, Route("getfolder")]
        public Folder Get(string folder)
        {
            return _mailService.GetMailByFolder(folder);
        }

        [HttpGet("{mailId:int}"), Route("getmail")]
        public Mail Get(int mailId)
        {
            return _mailService.GetMail(mailId);
        }

        [HttpPost,Route("postmail")]
        public string PostMail(Mail m)
        {
            if (m == null)
                throw new Exception("No Data");

            return "success";
        }
    }
}
