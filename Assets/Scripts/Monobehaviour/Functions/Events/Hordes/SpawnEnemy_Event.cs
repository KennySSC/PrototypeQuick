using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy_Event : Event
{
    //This script tells a hordes manager script that one enemy has spawned
    #region Private variables

    Hordes_Manager manager;

    #endregion

    #region Main Functions

    private void Start()
    {
        manager = FindObjectOfType<Hordes_Manager>();
    }

    public override void DoEvent()
    {
        manager.AddNewEnemy(1);
    }

    #endregion

    #region Get Set

    public override void GetObjects(List<GameObject> newObjects)
    {

    }

    #endregion 
}
