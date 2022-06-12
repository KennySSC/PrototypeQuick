using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefab_Event : Event
{
    //Tells to a procedural manager script that it must spawn a new prefab and a mini level has been completed

    #region private variables

    Procedural_Manager manager;

    #endregion

    #region Main Functions

    private void Start()
    {
        manager = FindObjectOfType<Procedural_Manager>();
    }
    public override void DoEvent()
    {
        manager.AddPoints(1);
        manager.SpawnOther();
    }

    #endregion

    #region Get Set

    public override void GetObjects(List<GameObject> newObjects)
    {
        
    }

    #endregion
}
