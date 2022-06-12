using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StopAgent_Event : Event
{
    [SerializeField] NavMeshAgent agent;
    public override void DoEvent()
    {
        agent.isStopped = true;
    }
    public override void GetObjects(List<GameObject> newObjects)
    {
        
    }
}
