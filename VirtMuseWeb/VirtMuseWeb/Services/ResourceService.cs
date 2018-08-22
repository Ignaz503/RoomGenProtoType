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
using UnityEngine;
using System.Drawing;

namespace VirtMuseWeb.Services
{
    public interface IResourceService
    {
        ResourceModel GetResource(int ID);
        Task PostResource(Resource<byte> res);
    }

    public class ResourceService : IResourceService
    {
        IHostingEnvironment _env;
        VirtMuseWebContext _context;
        ILogger<Program> _logger;

        public ResourceService(IHostingEnvironment env, VirtMuseWebContext context, ILogger<Program> logger)
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

        public ResourceModel GetResource(int ID)
        {
            return _context.Resource.First(r =>  r.ID == ID);
        }
       
        /// <summary>
        /// creates task that transforms float resource to binary resource
        /// and adds it to the DB
        /// </summary>
        /// <param name="bRes"></param>
        public async Task PostResource(Resource<byte> bRes)
        {
            FastObjImporter imp = new FastObjImporter();
            ResourceModel resModel = new ResourceModel()
            {
                ID = bRes.ID,
                MetaDataJSON =JsonConvert.SerializeObject(bRes.MetaData),
                Type = bRes.Type
            };

            if (bRes.Type == ResourceType.Mesh)
            {
                //make unity mesh if type mesh
                string mesh = System.Text.Encoding.Default.GetString(bRes.Data[0]);
                UnityMeshData mData = imp.BuildMesh(mesh);

                if (bRes.Data.Length == 3)
                {
                    _logger.LogInformation("Texture from sent data");
                    mData.Texture = ImageHelper.GetImage(bRes.Data[2]);
                }
                else
                    mData.Texture = Utility.Image.White(1, 1);// set white texture if no texture given

                //TODO: resource model not bResData 0
                resModel.Data = mData.Serialize();
            }
            else if (bRes.Type == ResourceType.RoomStyle)
            {
                Utility.RoomStyle rS = new Utility.RoomStyle(bRes.Data);
                resModel.Data = rS.Serialize();
            }
            else
            {
                //image 
                //just set resource model to byte array
                resModel.Data = ImageHelper.GetImage(bRes.Data[0]).Serialize();
            }
            try
            {
                resModel.ID = 0;
                var entry = _context.Resource.Add(new ResourceModel());
                entry.CurrentValues.SetValues(resModel);
                await _context.SaveChangesAsync();
            }catch(Exception e){
                _logger.LogError("Something went wrong whilst adding to the DB\n" + e);
            }
        }
    }
}
