using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VirtMuseWeb.Models;
using VirtMuseWeb.Utility;

namespace VirtMuseWeb.Services
{
    public interface IMuseumService
    {
        string GetMuseum(MuseumRequest Request);
    }

    public class MuseumService : IMuseumService
    {
        IHostingEnvironment _env;
        VirtMuseWebContext _context;
        ILogger<Program> _logger;

        public MuseumService(IHostingEnvironment env, VirtMuseWebContext context, ILogger<Program> logger)
        {
            _env = env;

            if (env == null)
                throw new Exception("Hosting enviroment was null");

            _context = context;

            if (context == null)
                throw new Exception("Context was null");

            if (_context.Database == null)
                throw new Exception("The Database is null");


            _logger = logger;
            if (logger == null)
                throw new Exception("Logger is null");

        }

        public string GetMuseum(MuseumRequest Request)
        {
            #region model shenaingans
            List<ResourceModel> model = _context.Resource.ToList();

            List<(int, ResourceType)> posRes = new List<(int, ResourceType)>();
            List<int> posStyle = new List<int>();

            foreach (ResourceModel item in model)
            {
                if (item.Type == ResourceType.RoomStyle)
                    posStyle.Add(item.ID);
                else
                    posRes.Add((item.ID,item.Type));
            }
            #endregion

            Museum m = new Museum((int)Request.Size);
            m.Initialize(new List<(RoomType, IRoomPlacableChecker)>()
            {
             (RoomType.Normal, new BaseRoomTypePlacableCheker(RoomType.Normal)),
             (RoomType.Long, new BaseRoomTypePlacableCheker(RoomType.Long)),
             ( RoomType.Big, new BaseRoomTypePlacableCheker(RoomType.Big)),
             (RoomType.L, new BaseRoomTypePlacableCheker(RoomType.L))
            },posRes,posStyle);
            m.Generate(DateTime.Now.ToLongTimeString());
            return m.Serialize();
        }
    }
}
