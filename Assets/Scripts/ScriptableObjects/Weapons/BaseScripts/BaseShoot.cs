using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseShoot : ScriptableObject
{
    //Base functions of a shot
    public abstract void DoShoot(Transform spawnPosition, Transform objectSpawnPos,float recoil, int damage, bool spawnObject);
    public abstract float GetRecoilAument();
    public abstract float GetRecoilAument_WhileAiming();
    public abstract float GetMaxRecoil();
    public abstract float GetTimeToBaseRecoil();
    public abstract void GetFatherCollider(Collider fatherCol);

}
