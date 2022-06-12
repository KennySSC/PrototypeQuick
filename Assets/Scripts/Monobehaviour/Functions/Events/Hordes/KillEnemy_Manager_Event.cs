using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillEnemy_Manager_Event : Event
{
    //Event for the horde manager to detect if a enemy has been killed
    private Hordes_Manager manager;
    private void Awake()
    {
        manager = FindObjectOfType<Hordes_Manager>();
    }
    public override void DoEvent()
    {
        manager.KillEnemy();
    }
    public override void GetObjects(List<GameObject> newObjects)
    {

    }
}
