using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VirtMuseWeb.Models;
using Newtonsoft.Json;

public abstract class BaseRoomStyleRequest : IResourceRequest
{
    public virtual bool IsDone { get; protected set; }

    public string BaseURL { get; set; }

    public int ResourceLocator { get; protected set; }

    public GameObject Requestor { get; protected set; }

    public RoomStyleResource.TextureToApply TextureToApply { get; protected set; }

    public RoomStyleResource Response { get; set; }

    public BaseRoomStyleRequest(int resLocator,GameObject requestor, RoomStyleResource.TextureToApply neededTexture)
    {
        ResourceLocator = resLocator;
        Requestor = requestor;
        TextureToApply = neededTexture;
        IsDone = false;
    }

    public virtual IEnumerator StartWorkRequest()
    {
        if (RoomStyleManager.Instance.CheckIfDownloaded(ResourceLocator))
        {
            //already downloaded
            Response = RoomStyleManager.Instance.GetStyle(ResourceLocator);
        }
        else
        {
            //check if someone else is downloading
            if (RoomStyleManager.Instance.CheckIfDownloading(ResourceLocator))
            {
                //woke up
                Response =  RoomStyleManager.Instance.GetStyle(ResourceLocator);
            }
            else
            {
                //start downloading
                RoomStyleManager.Instance.NotifyDownloading(ResourceLocator);
                WWW reqRep = new WWW(BaseURL + $"{ResourceLocator}");

                while (!reqRep.isDone)
                    yield return null;

                ResourceModel m = JsonConvert.DeserializeObject<ResourceModel>(reqRep.text);

                Response =  (RoomStyleResource)m;
                RoomStyleManager.Instance.AddStyle(Response);
            }// end else not downloaded and noone downloading
        }// end else if downloaded
        IsDone = true;
    }

    public virtual void WhenDone()
    {
        Response.ApplyToGameobject(Requestor,TextureToApply);
    }

}
