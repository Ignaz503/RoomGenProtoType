using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Threading.Tasks;
using VirtMuseWeb.Utility;
using VirtMuseWeb.Models;
using Newtonsoft.Json;
//using WebSocketSharp;

/// <summary>
/// Manages the loading of resources for the client
/// WARNING NOT IMPLEMENTED CURRENTLY
/// SHOULD BE SEEN AS A WHAT TO EXPECT
/// </summary>
public class OldResourceLoader : MonoBehaviour
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

        public Func<RequestResult, RequestResult> preProcessing;
       
        /// <summary>
        /// The type of resource that is requested
        /// needed to build correct response
        /// </summary>
        public string ResourceType;

        public Request(Action<RequestResult> display,string resoureceLocator, string resourceType, Func<RequestResult,RequestResult> preProcess)
        {
            requestCallback = display;
            ResourceLocator = resoureceLocator;
            ResourceType = resourceType;
            preProcessing = preProcess;
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

        /// <summary>
        /// The resource that has been gotten from the server
        /// </summary>
        public BaseResource res;

        PreProcessingGameObjectInformation GameObjectInformation;

        public RequestResult(Action<RequestResult> display, BaseResource toApply, PreProcessingGameObjectInformation obj)
        {
            requestCallBack = display;
            res = toApply;
            GameObjectInformation = obj;
        }

    }

    public static OldResourceLoader Instance { get; protected set; }

    Queue<RequestResult> LoadedResources = new Queue<RequestResult>();

    Queue<Request> LoadRequests = new Queue<Request>();

    [SerializeField] string BaseURL;
    [SerializeField][Range(1,5)] int resID;

    public MeshFilter filter;
    public MeshRenderer debugrenderer;
    //public string url = "ws://localhost:#";
    //WebSocket ws;

    private void Awake()
    {
        if(Instance != null)
        {
            throw new Exception("there already exists a resource loader");
        }
        Instance = this;

        DontDestroyOnLoad(this);

    }

    private void Start()
    {
        //TestMuseum();
        TestResource();
    }

    /// <summary>
    /// sets a request for a resource
    /// </summary>
    public void RequestResource(Action<RequestResult> callback, string ResourceLocator, string resType,Func<RequestResult,RequestResult> preProcessing)
    {
        lock(LoadRequests)
        {
            LoadRequests.Enqueue(new Request(callback,ResourceLocator,resType, preProcessing));
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
        //lock (LoadRequests)
        //{
        //    if (LoadRequests.Count > 0)
        //    {
        //        Request req = LoadRequests.Dequeue();
        //        Thread t = new Thread(() => LoadResource(req));
        //        t.Start();
        //    }
        //}

        //lock (LoadedResources)
        //{
        //    if (LoadedResources.Count > 0)
        //    {
        //        RequestResult res = LoadedResources.Dequeue();
        //        res.requestCallBack?.Invoke(res);
        //    }
        //}
    }

    void LoadResource(Request req)
    {
        // TODO IMPLEMENT
        // IS Maybe RUN IN SEPERATE THREADS
        //IF SO DO NOT CALL THE CALLBACK FROM HERE
        string[] requests = req.ResourceLocator.Split(';');

        byte[][] retrivedData = new byte[requests.Length][];
        int i = 0;
        foreach(string reLoc in requests)
        {
            //download everything needed
            WWW res = new WWW(BaseURL + $"/api/resource/getres?id={reLoc}");

            while (!res.isDone)
                Thread.Sleep(50);//wait for download
            retrivedData[i++] = res.bytes;
        }

        i = 0;
        string[] retrivedJson = new string[retrivedData.Length];
        foreach(byte[] b in retrivedData)
        {
            //make jsons out of everything
            using (MemoryStream st = new MemoryStream(b))
            {
                DataContractJsonSerializer dt = new DataContractJsonSerializer(typeof(string));
                retrivedJson[i++] = dt.ReadObject(st) as string;
            }
        }
        
        //TODO Set Request result to retrived resource
        RequestResult result = new RequestResult(req.requestCallback, null, PreProcessingGameObjectInformation.Neutral());
        req.preProcessing?.Invoke(result);
    }

    void TestMuseum()
    {
        using (MemoryStream st = new MemoryStream())
        {
            DataContractJsonSerializer dt = new DataContractJsonSerializer(typeof(MuseumRequestData));
            dt.WriteObject(st, new MuseumRequestData() { MuseumType = "just anything really", Size = MuseumSize.Medium });

            string get= $"/api/museum/getmuseum?request={Encoding.Default.GetString(st.ToArray())}";
            Debug.Log(get);
            WWW museumreply = new WWW(BaseURL + get);
            StartCoroutine(LogMuseum(museumreply));
        }
    }

    void TestResource()
    {
        string get = $"/api/resource/getres?id={resID}";
        WWW resReply = new WWW(BaseURL + get);
        StartCoroutine(GetMesh(resReply));
    }

    IEnumerator LogMuseum(WWW museum)
    {
        while (!museum.isDone)
            yield return null;

        string s = museum.text;

        MemoryStream st = new MemoryStream(museum.bytes);

        DataContractJsonSerializer dt = new DataContractJsonSerializer(typeof(string));

        string deserialized = dt.ReadObject(st) as string;

        foreach (string split in deserialized.Split('\n'))
            Debug.Log(split);

    }

    IEnumerator GetMesh(WWW reply)
    {
        while (!reply.isDone)
            yield return null;

        ResourceModel m = JsonConvert.DeserializeObject<ResourceModel>(reply.text);

        UnityMeshData mD = UnityMeshData.Deserialize(m.Data);
        Debug.Log($"Vertices count: {mD.Vertices.Length}");
        Debug.Log($"UVs count: {mD.UVs.Length}");
        Debug.Log($"Triangles count: {mD.Triangles.Length}");
        Debug.Log($"Normals count: {mD.Normals.Length}");
        Debug.Log(mD.ToString());

        Mesh mesh = new Mesh();

        Vector3[] vert = new Vector3[mD.Vertices.Length];

        for (int i = 0; i < mD.Vertices.Length; i++)
        {
            Vec3 v = mD.Vertices[i];
            vert[i] = v;
        }

        Vector2[] uv = new Vector2[mD.UVs.Length];

        for (int i = 0; i < mD.UVs.Length; i++)
        {
            Vec2 v = mD.UVs[i];
            uv[i] = v;
        }

        Vector3[] normals = new Vector3[mD.Normals.Length];

        for (int i = 0; i < mD.Normals.Length; i++)
        {
            Vec3 v = mD.Normals[i];
            normals[i] = v;
        }

        mesh.vertices = vert;
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.triangles = mD.Triangles;
    
        debugrenderer.material.mainTexture = (Texture2D)mD.Texture;
        filter.mesh = mesh;

    }
}
