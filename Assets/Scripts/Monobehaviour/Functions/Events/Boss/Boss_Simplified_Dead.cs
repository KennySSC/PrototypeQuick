using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Simplified_Dead : Event
{
    [SerializeField] Boss_Controller_Simplified controller;

    public override void DoEvent()
    {
        controller.Death();
    }
    public override void GetObjects(List<GameObject> newObjects)
    {
        
    }
}
