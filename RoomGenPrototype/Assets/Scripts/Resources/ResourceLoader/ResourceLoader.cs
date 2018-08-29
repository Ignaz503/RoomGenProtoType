using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ResourceLoader : MonoBehaviour
{
    public enum RequestType
    {
        MuseumRequest,
        Other
    }

    #region Debugging
    //public Display ImageDisplay;
    //public Display MeshDisplay;
    //public GameObject Wall1;
    //public GameObject Wall2;

    //public int[] ResourceIDs;

    //void TestImageRequest()
    //{
    //DisplayResourceRequest<DisplayImageResource> req = new DisplayResourceRequest<DisplayImageResource>(ResourceIDs[0], ImageDisplay, null);

    //PostRequest(req, RequestType.Other);

    //}

    //void TestMeshRequest()
    //{
    //    DisplayResourceRequest<DisplayMeshResource> req = new DisplayResourceRequest<DisplayMeshResource>(4, MeshDisplay,
    //        (res) =>
    //        {
    //            BoundingSphere boundsChild = BoundingSphere.Calculate(res.Mesh.vertices);
    //            BoundingSphere boundsParent = BoundingSphere.Calculate((MeshDisplay as MeshDisplay).ParentMesh.sharedMesh.vertices);

    //            float avg = (MeshDisplay.transform.localScale.x + MeshDisplay.transform.localScale.y + MeshDisplay.transform.localScale.z) / 3f;
    //            float scale = ((boundsParent.radius / boundsChild.radius) * avg);

    //            return new PreProcessingGameObjectInformation() { Scale = new Vector3(scale, scale, scale) };
    //        }

    //        );

    //    PostRequest(req, RequestType.Other);
    //}

    //void TestWallSingle()
    //{
    //    SingleWallTextureRequest req = new SingleWallTextureRequest(ResourceIDs[3], Wall1);
    //    PostRequest(req, RequestType.Other);
    //}

    //void TestMultiWall1()
    //{
    //    Wall w = new Wall(Wall.WallType.Solid, new Vector2[] { Vector2.zero, Vector2.zero }, -1f, Vector2Int.zero, 0, Wall.WallRotation.Vertical, null);

    //    w.TextureInfos.Add(new MuseumTextureInfo() { AssociatedResourceLocators = ResourceIDs[2], PositionModifier = 0 });
    //    w.TextureInfos.Add(new MuseumTextureInfo() { AssociatedResourceLocators = ResourceIDs[3], PositionModifier = 1 });


    //    MultiWallTextureRequest req = new MultiWallTextureRequest(w, Wall1);
    //    PostRequest(req, RequestType.Other);
    //}

    //void TestMultiWall2()
    //{
    //    Wall w = new Wall(Wall.WallType.Solid, new Vector2[] { Vector2.zero, Vector2.zero }, -1f, Vector2Int.zero, 0, Wall.WallRotation.Vertical, null);

    //    w.TextureInfos.Add(new MuseumTextureInfo() { AssociatedResourceLocators = ResourceIDs[3], PositionModifier = 0 });
    //    w.TextureInfos.Add(new MuseumTextureInfo() { AssociatedResourceLocators = ResourceIDs[2], PositionModifier = 1 });


    //    MultiWallTextureRequest req = new MultiWallTextureRequest(w, Wall2);
    //    PostRequest(req, RequestType.Other);
    //}

    //void TestSameTexture()
    //{
    //    TestMultiWall2();
    //    Action<Type> a = null;
    //    a = (t) => { TestWallSingle(); OnResourceDownloaded -= a; };
    //    OnResourceDownloaded += a;
    //}

    /// <summary>
    /// Debug start
    /// </summary>
    //public void Start()
    //{
    //    //TestMeshRequest();
    //    //TestImageRequest();
    //    //TestWallSingle();
    //    //TestMultiWall1();
    //    //TestMultiWall2();
    //    //TestSameTexture();
    //}

    #endregion


    /// <summary>
    /// Instance of resource loader
    /// </summary>
    public static ResourceLoader Instance { get; protected set; }
    /// <summary>
    /// number of request it is possible to work on at the same time
    /// </summary>
    [SerializeField] [Range(1, 20)] int numWorkableRequestsSameTime;
    /// <summary>
    /// base url to server
    /// eg: http://localhost:52536
    /// </summary>
    [SerializeField] string baseURL;

    /// <summary>
    /// sub route to resource eg /api/resource/getres?id=
    /// </summary>
    [SerializeField] string[] resourceRoute;

    /// <summary>
    /// event invoked when a resource was downloaded
    /// informs about what type the request was
    /// DONT send actuall IResourceRequest someone might unwatingly might tameper with things
    /// should be seen as info for observers
    /// </summary>
    public event Action<Type> OnResourceDownloaded;

    /// <summary>
    /// number of currently active requests
    /// </summary>
    int numCurrentRequestsWorkingOn = 0;

    /// <summary>
    /// queue of request posted
    /// </summary>
    Queue<IResourceRequest> requestQueue;

    /// <summary>
    /// queue of requests that are currently being worked on
    /// </summary>
    Queue<IResourceRequest> requestsWorkingOn;

    /// <summary>
    /// special request for the museum
    /// stored for easy storage between scenes 
    /// </summary>
    public Museum MuseumToBuild { get; set; }

    public void Awake()
    {
        if (Instance != null)
            throw new Exception("There already exists a ResourceLoader");

        DontDestroyOnLoad(this);
        Instance = this;
        requestQueue = new Queue<IResourceRequest>();
        requestsWorkingOn = new Queue<IResourceRequest>();
    }

/// <summary>
    /// first checks if we can start a new request, when there is space
    /// and we have request posted
    /// after that, checks if any request finished
    /// at the end checks if it can go to sleep
    /// when there are no requests worked on and no requests posted
    /// </summary>
    private void Update()
    {
        //Debug.Log($"Num requests: {requestQueue.Count}");
                
        //see if possible to start work on new request
        if(numCurrentRequestsWorkingOn < numWorkableRequestsSameTime)
        {
            //can start new request
            if(requestQueue.Count >0)
            {
                IResourceRequest req = requestQueue.Dequeue();
                numCurrentRequestsWorkingOn++;//increas number of worked on request

                requestsWorkingOn.Enqueue(req);//staart tracking worked on request
                StartCoroutine(req.StartWorkRequest());//start working request
            }
        }

        Queue<IResourceRequest> unfinishedRequests = new Queue<IResourceRequest>();
        while(requestsWorkingOn.Count > 0)
        {
            IResourceRequest req = requestsWorkingOn.Dequeue();
            //check if requst done, if so complete
            // else reenqueue
            if (req.IsDone)
            {
                OnResourceDownloaded?.Invoke(req.GetType());
                req.WhenDone();
                numCurrentRequestsWorkingOn--;//decrease num working on
            }
            else
            {
                unfinishedRequests.Enqueue(req);
            }
        }
        requestsWorkingOn = unfinishedRequests;

        //check if we can sleep
        // when no requests worked on or no request in queue
        if (requestsWorkingOn.Count == 0 && requestQueue.Count == 0)
            gameObject.SetActive(false);//goto sleep

    }

    /// <summary>
    /// posts request to request queue
    /// and wakes up gameobject
    /// </summary>
    /// <param name="req">the request that is posted</param>
    /// <param name="reqType">reqest type, to figure out routing on server</param>
    public void PostRequest(IResourceRequest req,RequestType reqType)
    {
        req.BaseURL = baseURL + resourceRoute[(int)reqType];//set url for request to download

        requestQueue.Enqueue(req);
        //wake up
        //needed cause loader goes to sleep when no requests in queue or worked on
        gameObject.SetActive(true);
    }
    
}
