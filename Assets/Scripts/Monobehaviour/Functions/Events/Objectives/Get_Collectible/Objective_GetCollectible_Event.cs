using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective_GetCollectible_Event : Event
{
    #region private Variables

    Objective_GetCollectible getCollectible;

    #endregion

    #region Main Functions

    private void Start()
    {
        if (GetComponent<Objective_GetCollectible>() != null)
        {
            getCollectible = GetComponent<Objective_GetCollectible>();
        }
        else if (GetComponentInParent<Objective_GetCollectible>() != null)
        {
            getCollectible = GetComponentInParent<Objective_GetCollectible>();
        }
        else if (GetComponentInChildren<Objective_GetCollectible>())
        {
            getCollectible = GetComponentInChildren<Objective_GetCollectible>();
        }
    }
    public override void DoEvent()
    {
        //Tell the objective get collectible that a collectible has been taken
        if(getCollectible!=null && !getCollectible.GetIsComplete()){

        }
        getCollectible.GetCollectible(1);

    }

    #endregion

    #region Get Set

    public override void GetObjects(List<GameObject> newObjects)
    {

    }

    #endregion
}
