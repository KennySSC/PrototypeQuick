using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy_Base : MonoBehaviour
{
    public abstract void Movement();
    public abstract void SetPlayer(Transform player);
    public abstract void Look();
    public abstract void AnimationLogic();
    public abstract void ChangeIsAlive(bool imAlive);
    public abstract void ChangeHasGun(bool has_Gun);
    public abstract Animator GetAnimator();
    public abstract bool MoveToLocation(Vector3 target);

    public abstract void Set_Waypoints(List<Transform> newWaypoints);
}
