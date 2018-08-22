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
    public Display ImageDisplay;
    public Display MeshDisplay;
    public GameObject Wall;

    public int[] ResourceIDs;
    #endregion


    /// <summary>
    /// Instance of resource loader
    /// </summary>
    public static ResourceLoader Instance { get; protected set; }
    /// <summary>
    /// number of request it is possible to work on at the same time
    /// </summary>
    [SerializeField] [Range(1, 20)] int NumWorkableRequestsSameTime;
    /// <summary>
    /// base url to server
    /// eg: http://localhost:52536
    /// </summary>
    [SerializeField] string BaseURL;

    /// <summary>
    /// sub route to resource eg /api/resource/getres?id=
    /// </summary>
    [SerializeField] string[] ResourceRoute;

    int numCurrentRequestsWorkingOn = 0;
    Queue<IResourceRequest> requestQueue;
    Queue<IResourceRequest> requestsWorkingOn;

    public void Awake()
    {
        if (Instance != null)
            throw new Exception("There already exists a ResourceLoader");

        DontDestroyOnLoad(this);
        Instance = this;
        RoomStyleManager.Instantiate();
        requestQueue = new Queue<IResourceRequest>();
        requestsWorkingOn = new Queue<IResourceRequest>();
    }


    public void Start()
    {
        //TestMeshRequest();
        TestImageRequest();
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
        //see if possible to start work on new request
        if(numCurrentRequestsWorkingOn < NumWorkableRequestsSameTime)
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
        req.BaseURL = BaseURL + ResourceRoute[(int)reqType];//set url for request to download
        requestQueue.Enqueue(req);
        gameObject.SetActive(true);//wake up
    }
    

    void TestImageRequest()
    {
        DisplayResourceRequest<DisplayImageResource> req = new DisplayResourceRequest<DisplayImageResource>(ResourceIDs[0], ImageDisplay, null);

        PostRequest(req, RequestType.Other);

    }

    void TestMeshRequest()
    {
        DisplayResourceRequest<DisplayMeshResource> req = new DisplayResourceRequest<DisplayMeshResource>(4, MeshDisplay,
            (res) => 
            {
                BoundingSphere boundsChild = BoundingSphere.Calculate(res.Mesh.vertices);
                BoundingSphere boundsParent = BoundingSphere.Calculate((MeshDisplay as MeshDisplay).ParentMesh.sharedMesh.vertices);
                
                float avg = (MeshDisplay.transform.localScale.x + MeshDisplay.transform.localScale.y + MeshDisplay.transform.localScale.z) / 3f;
                float scale = ((boundsParent.radius / boundsChild.radius) * avg);

                return new PreProcessingGameObjectInformation() { Scale = new Vector3(scale, scale, scale) };
            }          
            
            );

        PostRequest(req, RequestType.Other);
    }

}
