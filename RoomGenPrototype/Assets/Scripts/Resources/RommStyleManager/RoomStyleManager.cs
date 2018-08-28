using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Manages downloaded roomstyles, as to not download them multiple times
/// is thread safeish
/// </summary>
public sealed class RoomStyleManager
{
    public static RoomStyleManager _Instance;
    /// <summary>
    /// Instance of the manager
    /// </summary>
    public static RoomStyleManager Instance
    {
        get
        {
            if (_Instance == null)//possibly not thread safe
                _Instance = new RoomStyleManager();
            return _Instance;
        }
    }

/// <summary>
    /// thread safe dictionary of all roomstyles, maps from
    /// resource ID to style
    /// </summary>
    ConcurrentDictionary<int, RoomStyleResource> downloadedRoomStyles;
        /// <summary>
    /// thread safe dictionary, that tracks all styles that are currently
    /// being downloaded as to not start downloading it multiple times
    /// is dictinary cause of:
    /// https://stackoverflow.com/questions/18922985/concurrent-hashsett-in-net-framework
    /// </summary>
    ConcurrentDictionary<int, byte> currentlyDownloadingStyles;

    /// <summary>
    /// ctor that creates a manager with empty dictionaries
    /// </summary>
    private RoomStyleManager()
    {
        downloadedRoomStyles = new ConcurrentDictionary<int, RoomStyleResource>();
        currentlyDownloadingStyles = new ConcurrentDictionary<int, byte>();
    }
    
    /// <summary>
    /// checks if style was downloaded
    /// </summary>
    /// <param name="ID">the id of the style we want to check</param>
    /// <returns>true, if downloaded, false if not</returns>
    public  bool CheckIfDownloaded(int ID)
    {
        return downloadedRoomStyles.ContainsKey(ID);
    }

    /// <summary>
    /// checks if style is currently being downloaded
    /// </summary>
    /// <param name="ID">ID of style we want to know if being dowloaded</param>
    /// <returns>auto reset event for waiting on of this resource is currently downloaded</returns>
    public bool CheckIfDownloading(int ID)
    {
        return currentlyDownloadingStyles.ContainsKey(ID);
    }

    /// <summary>
    /// Adds a style to the downloaded styles and removes
    /// from currrently downloading tracker if contained
    /// </summary>
    /// <param name="st">the style we want to add</param>
    public void AddStyle(RoomStyleResource st)
    {
       downloadedRoomStyles.AddOrUpdate(st.ResourceID, st, (id, style) => { return st; });
        if (currentlyDownloadingStyles.ContainsKey(st.ResourceID))
        {
            byte b;
            currentlyDownloadingStyles.TryRemove(st.ResourceID,out b);
        }
    }

    /// <summary>
    /// notifies manager that this ID started downloading
    /// </summary>
    /// <param name="ID">ID of stlye we download</param>
    /// <returns>true if ID was added to tracker, false if not</returns>
    public bool NotifyDownloading(int ID)
    {
        return currentlyDownloadingStyles.TryAdd(ID, 0);
    }

    /// <summary>
    /// gets a style from the downloaded ones
    /// may return null if not found
    /// </summary>
    /// <param name="ID">id of style wanted</param>
    /// <returns>the style with the id: ID</returns>
    public RoomStyleResource GetStyle(int ID)
    {
        if(downloadedRoomStyles.ContainsKey(ID))
            return downloadedRoomStyles[ID];
        return null;
    }
}