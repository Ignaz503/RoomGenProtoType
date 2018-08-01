using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VirtMuseWeb.Models;


namespace VirtMuseWeb.Services
{
    public interface IResourceService
    {
        Resource GetResource(int ID);
        Creator GetCreator(int ID);
        MetaData GetMetaData(int ID);
        Source GetSource(int ID);
        void PostResource(Resource res);
    }

    public class ResourceService : IResourceService
    {
        IHostingEnvironment _env;
        VirtMuseWebContext _context;

        public ResourceService(IHostingEnvironment env, VirtMuseWebContext context)
        {
            _env = env;

            if (env == null)
                throw new Exception("Hosting enviroment was null");

            _context = context;

            if (context == null)
                throw new Exception("Context was null");
        }

        public Creator GetCreator(int ID)
        {
            throw new NotImplementedException();
        }

        public MetaData GetMetaData(int ID)
        {
            throw new NotImplementedException();
        }

        public Resource GetResource(int ID)
        {
            throw new NotImplementedException();
        }

        public Source GetSource(int ID)
        {
            throw new NotImplementedException();
        }

        public void PostResource(Resource res)
        {
            throw new NotImplementedException(res.Data.Length.ToString());
        }
    }
}
