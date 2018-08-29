using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityStandardAssets.Characters.FirstPerson;

/// <summary>
/// class that tracks the build progress of the museum
/// and sees if player can teleport into museum
/// </summary>
public class MuseumBuildObserver : MonoBehaviour {
    /// <summary>
    /// the player transform 
    /// </summary>
    [SerializeField] FirstPersonController firstPersonController;

    /// <summary>
    /// status display for floors built
    /// </summary>
    [SerializeField]TextMeshProUGUI floorProgressDisplay;
    /// <summary>
    /// status display fpr walls built
    /// </summary>
    [SerializeField]TextMeshProUGUI wallProgressDispalys;
    /// <summary>
    /// status display for resources requested
    /// </summary>
    [SerializeField]TextMeshProUGUI resourceProgressDisplay;

    /// <summary>
    /// Test display if entrance closed or open
    /// </summary>
    [SerializeField]TextMeshProUGUI entranceClosedOpenText;

    /// <summary>
    /// Material of Door mesh, will have color changed
    /// when done
    /// </summary>
    [SerializeField] Material doortMaterial;

    /// <summary>
    /// Door that is the entrace into the museum
    /// </summary>
    [SerializeField] BaseDoor door;

    /// <summary>
    /// trigger for the entrance door
    /// </summary>
    [SerializeField] DoorTrigger doorTrigger;

    /// <summary>
    /// Camera that renders redner texute, behind entracne door
    /// </summary>
    [SerializeField] Camera portalCamera;

    /// <summary>
    /// max number of floors to buil
    /// </summary>
    int maxFloorsAndCeilingsToBuild;
    /// <summary>
    /// max number of walls to build
    /// </summary>
    int maxWallsToBuild;
    /// <summary>
    /// max number of resources to request
    /// </summary>
    int maxResourceToRequest;

    int _currentFloorsAndCeilingsBuilt = 0;
    /// <summary>
    /// current number of floors built
    /// </summary>
    int currentFloorsAndCeilingsBuilt
    {
        get { return _currentFloorsAndCeilingsBuilt; }
        set
        {
            _currentFloorsAndCeilingsBuilt = value;
            floorProgressDisplay.text = $"Floors built: {_currentFloorsAndCeilingsBuilt}/{maxFloorsAndCeilingsToBuild}";
        }
    }

    int _currentWallsBuilt = 0;
    /// <summary>
    /// current number of walls built
    /// </summary>
    int currentWallsBuilt
    {
        get { return _currentWallsBuilt; }
        set
        {
            _currentWallsBuilt = value;
            wallProgressDispalys.text = $"Walls built: {_currentWallsBuilt}/{maxWallsToBuild}";
        }
    }

    int _currentResourcesRequested = 0;
    /// <summary>
    /// current number of resources requested
    /// </summary>
    int currentResourcesRequested
    {
        get { return _currentResourcesRequested; }
        set
        {
            _currentResourcesRequested = value;
            resourceProgressDisplay.text = $"Resources requested: {_currentResourcesRequested}/{maxResourceToRequest}";
        }
    }

    // Use this for initialization
    void Start () {
        MuseumBuilder.Instance.OnMuseumGotten += (m) =>
        {
            maxFloorsAndCeilingsToBuild = m.Rooms.Count;
            maxWallsToBuild = m.Walls.Count;


            //each wall is a single resource request
            maxResourceToRequest += m.Walls.Count;//for room style

            foreach(Room r in m.Rooms)
            {
                //every floor and ceiling seperate request
                //for every room tile
                maxResourceToRequest += 2 * r.RoomTiles.Count;
                //every resource
                maxResourceToRequest += r.CenterDisplayInfos.Length;
            }

            foreach(Wall w in m.Walls)
            {
                //every resource
                maxResourceToRequest += w.DisplayInfos.Count;
            }
            
            currentFloorsAndCeilingsBuilt = 0;
            currentWallsBuilt = 0;
            currentResourcesRequested = 0;
        };

        MuseumBuilder.Instance.OnWallBuilt += (w) => 
        {
            currentWallsBuilt++;
            UpdateEntrance();
        };
        MuseumBuilder.Instance.OnRoomBuilt += (r) =>
        {
            currentFloorsAndCeilingsBuilt++;
            UpdateEntrance();
            if (!portalCamera.gameObject.activeSelf)
            {
                MuseumBuilder.Instance.SetToStartPosition(portalCamera.transform);
                portalCamera.transform.position = new Vector3(portalCamera.transform.position.x, 2.5f, portalCamera.transform.position.z);
            }
        };
        ResourceLoader.Instance.OnResourceDownloaded += (t) =>
        {
            currentResourcesRequested++;
            UpdateEntrance();
        };

        door.OnOpen += (d) => TeleportIfPossible();

        doorTrigger.OnColliderEnter += (other, door) =>
        {
            if(CheckDone())
                firstPersonController.enabled = false;
        };
        doorTrigger.OnColliderLeave += (other, door) => firstPersonController.enabled = true;

        door.Lock("");
    }

    /// <summary>
    /// checks if museum finised building
    /// WARNING: currently temp version
    /// </summary>
    /// <returns>true if done building, false otherwise</returns>
    public bool CheckDone()
    {
        return maxFloorsAndCeilingsToBuild <= currentFloorsAndCeilingsBuilt && maxWallsToBuild <= currentWallsBuilt && maxResourceToRequest <= currentResourcesRequested;
    }

    public void UpdateEntrance()
    {
        if (CheckDone())
        {
            doortMaterial.color = Color.green;
            entranceClosedOpenText.text = "OPEN";
            entranceClosedOpenText.color = Color.green;
            door.Unlock("");
        }
    }

    public void TeleportIfPossible()
    {
        if (CheckDone())
        {
            firstPersonController.enabled = true;
            MuseumBuilder.Instance.SetToStartPosition(firstPersonController.transform);
        }
    }

    private void OnEnable()
    {
        doortMaterial.color = Color.red;
        entranceClosedOpenText.text = "CLOSED";
        entranceClosedOpenText.color = Color.red;
    }
    private void OnDestroy()
    {
        doortMaterial.color = Color.red;
    }
}
