using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective_ZoneComplete_Event : Event
{
    //Tells the objective stay in zone script that one zone has been completed
    #region private variables

    Objective_StayInZone zoneObjective;

    #endregion

    #region Main Functions

    private void Start()
    {
        if (GetComponent<Objective_StayInZone>() != null)
        {
            zoneObjective = GetComponent<Objective_StayInZone>();
        }
        else if (GetComponentInParent<Objective_StayInZone>() != null)
        {
            zoneObjective = GetComponentInParent<Objective_StayInZone>();
        }
        else if (GetComponentInChildren<Objective_StayInZone>())
        {
            zoneObjective = GetComponentInChildren<Objective_StayInZone>();
        }
    }
    public override void DoEvent()
    {
        if(zoneObjective != null && !zoneObjective.GetIsComplete())
        {
            zoneObjective.CompleteZone();
        }
    }

    #endregion

    #region Get Set

    public override void GetObjects(List<GameObject> newObjects)
    {

    }

    #endregion
}
