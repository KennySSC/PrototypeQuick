using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective_KillBoss_Event : Event
{
    //Event that indicates a objective kill boss script that the boss has been killed
    #region private Variables

    Objective_KillBoss killBoss;

    #endregion

    #region Main Functions

    private void Start()
    {
        if (GetComponent<Objective_KillBoss>() != null)
        {
            killBoss = GetComponent<Objective_KillBoss>();
        }
        else if (GetComponentInParent<Objective_KillBoss>() != null)
        {
            killBoss = GetComponentInParent<Objective_KillBoss>();
        }
        else if (GetComponentInChildren<Objective_KillBoss>())
        {
            killBoss = GetComponentInChildren<Objective_KillBoss>();
        }
    }
    public override void DoEvent()
    {
        if(killBoss != null && !killBoss.GetIsComplete())
        {
            killBoss.ObjectiveComplete();
        }
    }

    #endregion

    #region Get Set

    public override void GetObjects(List<GameObject> newObjects)
    {

    }

    #endregion 
}
