using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Event : MonoBehaviour
{
    public abstract void DoEvent();
    public abstract void GetObjects(List<GameObject> newObjects);
}
