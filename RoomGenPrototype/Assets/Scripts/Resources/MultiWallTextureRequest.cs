using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VirtMuseWeb.Models;
using VirtMuseWeb.Utility;
using Newtonsoft.Json;

public class MultiWallTextureRequest : BaseRoomStyleRequest
{
    /// <summary>
    /// ID of second style to get
    /// </summary>
    public int SecondResourceLocator { get; protected set; }

    /// <summary>
    /// second style response
    /// </summary>
    public RoomStyleResource SecondResponse { get; protected set; }

    /// <summary>
    /// merged texure of style one and two
    /// </summary>
    public Texture2D MergedTexture { get; protected set; }

    /// <summary>
    /// Wall that requested the texture
    /// </summary>
    public Wall RequestingWall { get; protected set; }

    /// <summary>
    /// ctor for request
    /// </summary>
    /// <param name="resourceLocator">ID of first style to get</param>
    /// <param name="resourceLocator2">ID of second style to get</param>
    /// <param name="requestor">requessting gameobject</param>
    /// <param name="resLocPositionMod">position modifer of texture info for first style</param>
    /// <param name="resLoc2PositionMod">position mod for second style from texture info</param>
    public MultiWallTextureRequest(Wall w, GameObject requestor )
        : base(w.TextureInfos[0].AssociatedResourceLocators, requestor, RoomStyleResource.TextureToApply.Wall)
    {
        SecondResourceLocator = w.TextureInfos[1].AssociatedResourceLocators;

        RequestingWall = w;
    }

    public override IEnumerator StartWorkRequest()
    {
        #region First Style
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
                Response = RoomStyleManager.Instance.GetStyle(ResourceLocator);
            }
            else
            {
                //start downloading
                RoomStyleManager.Instance.NotifyDownloading(ResourceLocator);
                WWW reqRep = new WWW(BaseURL + $"{ResourceLocator}");

                while (!reqRep.isDone)
                    yield return null;

                ResourceModel m = JsonConvert.DeserializeObject<ResourceModel>(reqRep.text);

                Response = (RoomStyleResource)m;
                RoomStyleManager.Instance.AddStyle(Response);
            }// end else not downloaded and noone downloading
        }// end else if downloaded
        #endregion
        #region SecondStyle
        if (RoomStyleManager.Instance.CheckIfDownloaded(SecondResourceLocator))
        {
            //already downloaded
            SecondResponse = RoomStyleManager.Instance.GetStyle(SecondResourceLocator);
        }
        else
        {
            //check if someone else is downloading
            if (RoomStyleManager.Instance.CheckIfDownloading(SecondResourceLocator))
            {
                //woke up
                SecondResponse = RoomStyleManager.Instance.GetStyle(SecondResourceLocator);
            }
            else
            {
                //start downloading
                RoomStyleManager.Instance.NotifyDownloading(SecondResourceLocator);
                WWW reqRep = new WWW(BaseURL + $"{SecondResourceLocator}");

                while (!reqRep.isDone)
                    yield return null;

                ResourceModel m = JsonConvert.DeserializeObject<ResourceModel>(reqRep.text);

                SecondResponse = (RoomStyleResource)m;
                RoomStyleManager.Instance.AddStyle(SecondResponse);
            }// end else not downloaded and noone downloading
        }// end else if downloaded
        #endregion
                     
       MergeWallTextures();

        IsDone = true;
    }

    public override void WhenDone()
    {
        MeshRenderer re = Requestor.GetComponent<MeshRenderer>();
        if (re == null)
        {
            re = Requestor.AddComponent<MeshRenderer>();
            re.material = new Material(Shader.Find("Standard"));
        }
        re.material.mainTexture = MergedTexture;
    }

    /// <summary>
    /// Merges the two wall textures of Response and SecondResponse
    /// </summary>
    void MergeWallTextures()
    { 
        //place wall textures into array depending on pos mod to merge correctly
        Texture2D[] textures = new Texture2D[2];
        textures[RequestingWall.TextureInfos[0].PositionModifier] = Response.Wall;
        textures[RequestingWall.TextureInfos[1].PositionModifier] = SecondResponse.Wall;


        MergedTexture = new Texture2D(2 * Response.Wall.width, Response.Wall.height); // create new texture

        //loop over texture and set pixels
        for (int x = 0; x < MergedTexture.width / 2; x++)
        {
            for (int y = 0; y < MergedTexture.height; y++)
            {
                Color t1 = textures[0].GetPixel(x, y);
                Color t2 = textures[1].GetPixel(x, y);

                MergedTexture.SetPixel(x, y, t1);
                MergedTexture.SetPixel(x + (MergedTexture.width / 2), y, t2);
            }
        }

        MergedTexture.Apply();
    }
}
