﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using VirtMuseWeb.Models;
using System.Net.Http;

public class DisplayResourceRequest<T> : IResourceRequest where T : BaseDisplayResource
{
    /// <summary>
    /// Resource ID of resource to get
    /// </summary>
    public int ResourceLocator { get; protected set; }
    /// <summary>
    /// Display that requested this resource
    /// </summary>
    public Display Requestor { get; protected set; }
    /// <summary>
    /// Resonse gotten from server, applicable to gamobject
    /// </summary>
    public T Response { get; protected set; }
    /// <summary>
    /// function to preprocess resource gotten
    /// </summary>
    public Func<T, PreProcessingGameObjectInformation> PreProcessing { get; set; }
    /// <summary>
    /// preprocessing information, that needs to be applied to the gameobject of the resource from the main thread
    /// </summary>
    public PreProcessingGameObjectInformation PreProcessedGameObjectInformation { get; set; }


    public string BaseURL { get; set; }
    public bool IsDone { get; private set; }

    public DisplayResourceRequest(int resourceLocator, Display requestor, Func<T, PreProcessingGameObjectInformation> preProcess)
    {
        ResourceLocator = resourceLocator;
        Requestor = requestor;
        PreProcessing = preProcess;
        IsDone = false;
    }

    public IEnumerator StartWorkRequest()
    {

        WWW reqReply = new WWW(BaseURL + $"{ResourceLocator}");

        while (!reqReply.isDone)
            yield return null;

        ResourceModel model = JsonConvert.DeserializeObject<ResourceModel>(reqReply.text);

        Response = (T)model;

        PreProcessedGameObjectInformation = PreProcessing?.Invoke(Response);
        IsDone = true;
    }

    public void WhenDone()
    {
        Requestor.ApplyResource(Response);
        if (PreProcessedGameObjectInformation != null)
        {
            Requestor.ApplyPreProcessingInformation(PreProcessedGameObjectInformation);
        }
    }

}
