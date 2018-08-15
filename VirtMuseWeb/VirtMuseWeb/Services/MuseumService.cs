using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtMuseWeb.Utility;

namespace VirtMuseWeb.Services
{
    public interface IMuseumService
    {
        string GetMuseum(MuseumRequest Request);
    }

    public class MuseumService : IMuseumService
    {
        public string GetMuseum(MuseumRequest Request)
        {
            Museum m = new Museum((int)Request.Size);
            m.Generate(DateTime.Now.ToLongTimeString());
            return m.Serialize();
        }
    }
}
