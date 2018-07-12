using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using UnityEngine;
//using WebSocketSharp;

/// <summary>
/// Manages the loading of resources for the client
/// WARNING NOT IMPLEMENTED CURRENTLY
/// SHOULD BE SEEN AS A WHAT TO EXPECT
/// </summary>
public class ResourceLoader : MonoBehaviour
{

    /// <summary>
    /// struct for a request from the museum builder to get a certain resource from
    /// the server
    /// </summary>
    public struct Request
    {
        /// <summary>
        /// callback to whoever  created request
        /// </summary>
        public Action<RequestResult> requestCallback;

        /// <summary>
        /// resource locator for server to find resource
        /// </summary>
        public string ResourceLocator;

        /// <summary>
        /// The type of resource that is requested
        /// needed to build correct response
        /// </summary>
        public string ResourceType;

        public Request(Action<RequestResult> display,string resoureceLocator, string resourceType)
        {
            requestCallback = display;
            ResourceLocator = resoureceLocator;
            ResourceType = resourceType;
        }
    }

    /// <summary>
    /// the result of a request after it was revieved from the server
    /// </summary>
    public struct RequestResult
    {
        /// <summary>
        /// callback to resource requester, called by main unity thread
        /// eg safe that callback touches gamobjects(sensually ofc)
        /// </summary>
        public Action<RequestResult> requestCallBack;
        public BaseResource res;

        public RequestResult(Action<RequestResult> display, BaseResource toApply)
        {
            requestCallBack = display;
            res = toApply;
        }

    }

    public static ResourceLoader Instance { get; protected set; }

    Queue<RequestResult> LoadedResources = new Queue<RequestResult>();

    Queue<Request> LoadRequests = new Queue<Request>();

    //public string url = "ws://localhost:#";
    //WebSocket ws;

    public string responestring = "";

    private void Awake()
    {
        if(Instance != null)
        {
            throw new Exception("there already exists a resource loader");
        }
        Instance = this;

    //    ws = new WebSocket(url);

    //    ws.OnMessage += (sender, e) =>
    //    {
    //        Debug.Log("Recieved message");
    //        responestring = e.Data;
    //    };

    //    ws.OnOpen += (sender, e) =>
    //    {
    //        Debug.Log("open");
    //        ws.SendAsync("Test", OIWSA);
    //    };

    //    ws.ConnectAsync();
    }

    //void OIWSA(bool b)
    //{
    //    Debug.Log(b);
    //}

    //private void OnDestroy()
    //{
    //    //ws.Close();
    //}


    /// <summary>
    /// sets a request for a resource
    /// </summary>
    public void RequestResource(Action<RequestResult> callback, string ResourceLocator, string resType)
    {
        lock(LoadRequests)
        {
            LoadRequests.Enqueue(new Request(callback,ResourceLocator,resType));
        };
    }

    /// <summary>
    /// function to retrieve a result of a request
    /// </summary>
    /// <returns></returns>
    public RequestResult RetriveResult()
    {
        RequestResult res;
        lock (LoadedResources)
        {
            res = LoadedResources.Dequeue();
        }
        return res;
    }

    private void Update()
    {
        lock (LoadRequests)
        {
            if (LoadRequests.Count > 0)
            {
                Request req = LoadRequests.Dequeue();
                Thread t = new Thread(() => LoadResource(req));
                t.Start();
            }
        }

        lock (LoadedResources)
        {
            if (LoadedResources.Count > 0)
            {
                RequestResult res = LoadedResources.Dequeue();
                res.requestCallBack?.Invoke(res);
            }
        }

        Debug.Log(responestring);
    }

    void LoadResource(Request req)
    {
        // TODO IMPLEMENT
        // IS Maybe RUN IN SEPERATE THREADS
        //IF SO DO NOT CALL THE CALLBACK FROM HERE
    }

    void TestRequest()
    {

    }

}
