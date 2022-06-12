using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShotgunShoot", menuName = "Scriptable Objects/Shoot Type/ Shotgun shot", order = 1)]
public class ShotgunShoot : BaseShoot
{
    #region Serialized variables

    [Header("Main Settings")]

    [Tooltip("The ammount of impacts that the shotgun will permorm")]
    [SerializeField] int pelletAmount;

    [Tooltip("The dispersion added for each consecutive shot")]
    [SerializeField] float dispersionAument;

    [Tooltip("Limits the dispersion")]
    [SerializeField] float maxDispersion;

    [Tooltip("Percentage of the dispersion that will be applied while aiming")]
    [Range(0.01f, 0.99f)] [SerializeField] float aimingDispersionAument_Multiplier; 

    [Tooltip("The time that the player must to wait before a shot has the minimum dispersion")]
    [SerializeField] float timeToBaseDisperion;

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

    #region Private variables

    private Collider fatherCollider;

    #endregion

    #region Main Functions

    public override void DoShoot(Transform spawnPosition, Transform shotObjectSpawnPos, float dispersion, int damage, bool canSpawnObj)
    {

        //Calculate the dispersion for each pellet
        for(int i = 0; i<pelletAmount; i++)
        {
            Vector3 tempDirectionObject = new Vector3(0, 0, 0);
            GameObject temp = null;
            float dispX = (Random.Range(((dispersion * -1) - 0.8f), dispersion + 0.8f) / 10);
            float dispY = (Random.Range((dispersion * -1)-0.8f, dispersion+0.8f) / 10);
            Vector3 offsetAdd = new Vector3(dispX, dispY, 0);
            RaycastHit hit;
            float disp = Random.Range((dispersion * -1), dispersion);

            //Spawns bullet object and set it's trajectory to equals the ray cast
            if (bulletObject != null && canSpawnObj)
            {
                temp = Instantiate(bulletObject, spawnPosition.position, spawnPosition.rotation);
                temp.transform.position = shotObjectSpawnPos.position;
                temp.GetComponent<Bullet_Object>().ChangeFollowRay(false);
                temp.transform.forward += offsetAdd;
            }
            if (Physics.Raycast(spawnPosition.position, spawnPosition.forward + offsetAdd, out hit, Mathf.Infinity, affectedLayers))
            {
                tempDirectionObject = hit.point;
                if(((hit.collider.gameObject.tag == "Damagable" || hit.collider.gameObject.tag == "Player") && hit.collider!= fatherCollider) && hit.collider.gameObject.GetComponent<Health>()!=null)
                {
                    if (hit.collider.gameObject.tag == "Player")
                    {
                        hit.collider.gameObject.GetComponent<Health>().GetDamage(damage / 2);
                    }
                    else
                    {
                        hit.collider.gameObject.GetComponent<Health>().GetDamage(damage);
                    }
                    if (damageParticle!= null)
                    {
                        Instantiate(damageParticle, hit.point, damageParticle.transform.rotation);
                    }
                }
                else
                {
                    GameObject hole = Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal));
                    hole.transform.position += hole.transform.forward / 1000;
                    if(objectParticle != null)
                    {
                        Instantiate(objectParticle, hit.point, objectParticle.transform.rotation);
                    }
                    if(impactSound != null)
                    {
                        Instantiate(impactSound, hit.point, impactSound.transform.rotation);
                    }
                }
                if (tempDirectionObject != null || tempDirectionObject != new Vector3(0, 0, 0) && temp != null)
                {
                    temp.GetComponent<Bullet_Object>().ChangeFollowRay(true);
                    temp.GetComponent<Bullet_Object>().ChangeDirectionPos(tempDirectionObject);
                }
            }
        }
    }
    #endregion

    #region Get Set

    public override float GetRecoilAument()
    {
        return dispersionAument;
    }
    public override float GetMaxRecoil()
    {
        return maxDispersion;
    }
    public override float GetTimeToBaseRecoil()
    {
        return timeToBaseDisperion;
    }
    public override float GetRecoilAument_WhileAiming()
    {
        return (dispersionAument* aimingDispersionAument_Multiplier);
    }
    public override void GetFatherCollider(Collider fatherCol)
    {
        fatherCollider = fatherCol;
    }
    #endregion 
}
