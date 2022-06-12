using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Open_Close_Doors_Event : Event
{
    //Indicates to the doors to lock or unlock, depending on its current state
    #region Serializable Variables

    [SerializeField] Open_Door[] doors;
    [SerializeField] InteractForEvent mechanism;

    [SerializeField] Sprite closedSprite;
    [SerializeField] Sprite openedSprite;

    #endregion

    #region Main Functions
    private void Start()
    {
        foreach (Open_Door od in doors)
        {
            if (od.GetIsUnlocked() && mechanism != null)
            {
                mechanism.ExternallyChange_Sprites(closedSprite, closedSprite);
                mechanism.ExternallyChange_Info("Lock", "Lock");
            }
            else if (mechanism != null && !od.GetIsUnlocked())
            {
                mechanism.ExternallyChange_Sprites(openedSprite, openedSprite);
                mechanism.ExternallyChange_Info("Unlock", "Unlock");
            }
        }
    }
    public override void DoEvent()
    {
        foreach(Open_Door od in doors)
        {
            od.Reverse_OpenClose_Door();
            if (od.GetIsUnlocked() && mechanism != null)
            {
                mechanism.ExternallyChange_Sprites(closedSprite, closedSprite);
                mechanism.ExternallyChange_Info("Lock", "Lock");
            }
            else if (mechanism != null && !od.GetIsUnlocked())
            {
                mechanism.ExternallyChange_Sprites(openedSprite, openedSprite);
                mechanism.ExternallyChange_Info("Unlock", "Unlock");
            }
        }
    }

    #endregion

    #region Get Set

    public override void GetObjects(List<GameObject> newObjects)
    {
        
    }

    #endregion 
}
