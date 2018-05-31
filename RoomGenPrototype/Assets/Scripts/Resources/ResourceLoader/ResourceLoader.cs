using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Manages the loading of resources for the client
/// WARNING NOT IMPLEMENTED CURRENTLY
/// SHOULD BE SEEN AS A WHAT TO EXPECT
/// </summary>
public class ResourceLoader : MonoBehaviour
{
    public struct Request
    {
        public Display disp;
        public string ResourceLocator;

        public Request(Display display,string resoureceLocator)
        {
            disp = display;
            ResourceLocator = resoureceLocator;
        }
    }

    public struct RequestResult
    {
        public Display disp;
        public Resource res;

        public RequestResult(Display display, Resource toApply)
        {
            disp = display;
            res = toApply;
        }

    }

    Queue<RequestResult> LoadedResources;

    Queue<Request> LoadRequests;

    public void RequestResource(Display disp, string ResourceLocator)
    {
        lock(LoadRequests)
        {
            LoadRequests.Enqueue(new Request(disp,ResourceLocator));
        };
    }

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
            if(LoadRequests.Count > 0)
            {
                Request req = LoadRequests.Dequeue();
                Thread t = new Thread(()=> LoadResource(req));
                t.Start();
            }
        }

        lock (LoadedResources)
        {
            if (LoadedResources.Count > 0)
            {
                RequestResult res = LoadedResources.Dequeue();
                res.disp.ApplyResource(res.res);
            }
        }

    }

    void LoadResource(Request req)
    {
        // TODO IMPLEMENT
        // IS RUN IN SEPERATE THREADS
    }
}
