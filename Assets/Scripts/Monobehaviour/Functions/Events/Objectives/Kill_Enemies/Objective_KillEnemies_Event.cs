using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective_KillEnemies_Event : Event
{
    //Tells to a objective kill enemies script that a enemy has been killed
    #region private variables

    Objective_KillEnemies objective_Kill = null;

    #endregion

    #region Main Functions

    private void Start()
    {
        if (GetComponent<Objective_KillEnemies>() != null)
        {
            objective_Kill = GetComponent<Objective_KillEnemies>();
        }else if(GetComponentInParent<Objective_KillEnemies>() != null)
        {
            objective_Kill = GetComponentInParent<Objective_KillEnemies>();
        }else if (GetComponentInChildren<Objective_KillEnemies>())
        {
            objective_Kill = GetComponentInChildren<Objective_KillEnemies>();
        }
    }
    public override void DoEvent()
    {
        if(objective_Kill!= null && !objective_Kill.GetIsComplete())
        {
            objective_Kill.KillEnemy(1);
        }
    }

    #endregion

    #region Get Set

    public override void GetObjects(List<GameObject> newObjects)
    {

    }

    #endregion
}
