using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Single_Shot", menuName = "Scriptable Objects/Shoot Type/ Single shot", order = 0)]
public class SingleShot : BaseShoot
{
    #region Serializable Variables 

    [Tooltip("The value of how much the recoil will aument for each consecutive shot")]
    [SerializeField] float recoilAument;

    [Tooltip("Limits the recoil")]
    [SerializeField] float maxRecoil;

    [Tooltip("Percentage of the recoil that will be applied while aiming")]
    [Range(0.01f, 0.99f)] [SerializeField] float aimingRecoilAument_Multiplier;

    [Tooltip("The time that the player must to wait before a shot has the minimum recoil")]
    [SerializeField] float timeToBaseRecoil;

    [Tooltip("Layers that will be affected")]
    [SerializeField] LayerMask affectedLayers;


    [Space]


    [Header("Sound & Particles")]

    [Tooltip("Prefab that goes in raycast direction that helps the player to aim. If you don't need it you can leave it empty")]
    [SerializeField] GameObject bulletObject;

    [Tooltip("Prefab of a bullet hole. It must have a LifeTime script attached. If you don't want a bullet hole to spawn, leave it empty")]
    [SerializeField] GameObject bulletHole;

    [Tooltip("It will appear where it impacts if it has a health script. If you don't need it, leave it empty")]
    [SerializeField] GameObject damageParticle;

    [Tooltip("It will appear where it impacts a object that doesn't have a health script. If you don't need it, leave it empty")]
    [SerializeField] GameObject objectParticle;

    [Tooltip("It will perform where it impacts if the object doesn't have a health script. If you don't need it, leave it empty")]
    [SerializeField] GameObject impactSound;



    #endregion

    #region Private Variables

    private Collider fatherCollider;

    #endregion

    #region  Main Functions 

    public override void DoShoot(Transform spawnPosition,Transform objectSpawnPos, float dispersion, int damage, bool spawnObject)
    {
        //Creates a single raycast that searches for a damagable object that has a health script
        GameObject temp = null;
        //Spawns bullet object and set it's trajectory to equals the ray cast

        Vector3 tempDirectionObject = new Vector3(0,0,0);
        RaycastHit hit;
        float dispX = (Random.Range((dispersion * -1), dispersion)/10);
        float dispY = (Random.Range((dispersion * -1), dispersion)/10);
        Vector3 offsetAdd = new Vector3(dispX, dispY, 0);
        if (bulletObject != null && spawnObject)
        {
            temp = Instantiate(bulletObject, spawnPosition.position, spawnPosition.rotation);
            temp.transform.position = objectSpawnPos.position;
            temp.GetComponent<Bullet_Object>().ChangeFollowRay(false);
            temp.transform.forward += offsetAdd;
        }

        if (Physics.Raycast(spawnPosition.position, spawnPosition.forward + offsetAdd, out hit, Mathf.Infinity, affectedLayers))
        {
            tempDirectionObject = hit.point;
            if (((hit.collider.gameObject.tag == "Damagable" || hit.collider.gameObject.tag == "Player") && hit.collider != fatherCollider) && hit.collider.gameObject.GetComponent<Health>() != null)
            {
                if(hit.collider.gameObject.tag == "Player")
                {
                    hit.collider.gameObject.GetComponent<Health>().GetDamage(damage/2);
                }
                else
                {
                    hit.collider.gameObject.GetComponent<Health>().GetDamage(damage);
                }

                if(damageParticle!= null)
                {
                    Instantiate(damageParticle, hit.point, damageParticle.transform.rotation);
                }
                
            }
            else
            {
                GameObject hole = Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal));
                hole.transform.position += hole.transform.forward / 1000;
                if(impactSound != null)
                {
                    Instantiate(impactSound, hit.point, impactSound.transform.rotation);
                }
                if(objectParticle!= null)
                {
                    Instantiate(objectParticle, hit.point, objectParticle.transform.rotation);
                }
            }
            if (tempDirectionObject != null || tempDirectionObject != new Vector3(0, 0, 0) && temp != null)
            {
                temp.GetComponent<Bullet_Object>().ChangeFollowRay(true);
                temp.GetComponent<Bullet_Object>().ChangeDirectionPos(tempDirectionObject);
            }
        }

    }

    #endregion

    #region Get Set

    public override float GetRecoilAument()
    {
        return recoilAument;
    }
    public override float GetMaxRecoil()
    {
        return maxRecoil;
    }
    public override float GetRecoilAument_WhileAiming()
    {
        return (recoilAument * aimingRecoilAument_Multiplier);
    }
    public override float GetTimeToBaseRecoil()
    {
        return timeToBaseRecoil;
    }
    public override void GetFatherCollider(Collider fatherCol)
    {
        fatherCollider = fatherCol;
    }
    #endregion
}
