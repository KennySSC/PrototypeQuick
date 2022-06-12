using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAim : ScriptableObject
{
    //Base functions of a gun aim
    public abstract void StartReset(List<Renderer> playerRenderers);
    public abstract void Aim(GameObject cam);
    public abstract void CancelAim(GameObject cam);
    public abstract void BaseFov(float fov);
    public abstract void SightObject(GameObject sightObj);
    public abstract void GetRenderers(List<Renderer> playerRenderers, List<Renderer> gunRenderers);
    public abstract int GetAimType_Index();
}
