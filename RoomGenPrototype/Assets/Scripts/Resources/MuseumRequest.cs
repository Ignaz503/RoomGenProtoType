using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json;

public class MuseumRequest : IResourceRequest
{
    public bool IsDone { get; private set; }

    public string BaseURL { get; set; }

    /// <summary>
    /// The museum request data sent to the server
    /// </summary>
    public MuseumRequestData RequestData { get; protected set; }
    /// <summary>
    /// the function that builds the museum from the data received from the server
    /// </summary>
    public Action<Museum> CallBack { get; protected set; }

    /// <summary>
    /// The response from the server
    /// </summary>
    public Museum Response { get; protected set; }

    public MuseumRequest(MuseumRequestData data, Action<Museum>callback)
    {
        RequestData = data;
        CallBack = callback;
        IsDone = false;
    }

    public IEnumerator StartWorkRequest()
    {
        WWW reqReply = new WWW(BaseURL + JsonConvert.SerializeObject(RequestData));

        while (!reqReply.isDone)
            yield return null;

        //TODO: FIX SERVER SIDE AS WELL
        string s = JsonConvert.DeserializeObject<string>(reqReply.text);
        Response = Museum.Deserialize(s);
        IsDone = true;
    }

    public void WhenDone()
    {
        CallBack?.Invoke(Response);
    }
}
