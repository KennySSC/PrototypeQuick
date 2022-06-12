using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective_Destroy_Event : Event
{
    //Event that must be called when a object of the objective destroy has been destroyed
    #region Private variables

    private Objective_Destroy destroy;

    #endregion

    #region Main Functions

    void Start()
    {
        if (GetComponent<Objective_Destroy>() != null)
        {
            destroy = GetComponent<Objective_Destroy>();
        }
        else if (GetComponentInParent<Objective_Destroy>() != null)
        {
            destroy = GetComponentInParent<Objective_Destroy>();
        }
        else if (GetComponentInChildren<Objective_Destroy>())
        {
            destroy = GetComponentInChildren<Objective_Destroy>();
        }
    }
    public override void DoEvent()
    {
        if(destroy!= null && !destroy.GetIsComplete())
        {
            destroy.DestroyObjective(1);
        }

    }

    #endregion

    #region Get Set

    public override void GetObjects(List<GameObject> newObjects)
    {

    }

    #endregion
}
