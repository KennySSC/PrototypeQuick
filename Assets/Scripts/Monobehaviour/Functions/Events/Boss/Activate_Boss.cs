using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activate_Boss : Event
{
    [Tooltip("The boss that you want to activate")]
    [SerializeField] Boss_Controller bossController;
    public override void DoEvent()
    {
        bossController.ActivateBoss();
    }
    public override void GetObjects(List<GameObject> newObjects)
    {
        
    }

}
