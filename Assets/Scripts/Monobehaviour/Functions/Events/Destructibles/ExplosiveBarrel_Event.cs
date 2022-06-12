using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel_Event : Event
{
    #region Serializable Variables

    [Header("Main Settings")]

    [SerializeField] float explosionRadius;

    [SerializeField] int explosionDamage;

    [SerializeField] bool canDamageBoss;


    [Space]


    [Header("Sound & Particles")]

    [Tooltip("It will perform when the event starts. If you don't need it leave it empty")]
    [SerializeField] GameObject explosionSound;

    [Tooltip("It will appear when the event starts. If you don't need it leave it empty")]
    [SerializeField] GameObject explosionParticle;

    #endregion

    #region Main Functions
    public override void DoEvent()
    {
        //Explodes and verifies the objects with a health script
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, explosionRadius, transform.forward, 0);
        foreach (RaycastHit hit in hits)
        {
            if (canDamageBoss)
            {
                if(hit.collider.gameObject.GetComponent<Health>()!= null)
                {
                    hit.collider.gameObject.GetComponent<Health>().GetDamage(explosionDamage);
                }
            }
            else
            {
                if (hit.collider.gameObject.GetComponent<Health>() != null && !hit.collider.gameObject.GetComponent<Health>().GetIsBoss())
                {
                    hit.collider.gameObject.GetComponent<Health>().GetDamage(explosionDamage);
                }
            }
        }
        if (explosionSound != null)
        {
            Instantiate(explosionSound, transform.position, transform.rotation);
        }
        if (explosionParticle != null)
        {
            Instantiate(explosionParticle, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }

    #endregion

    #region Get Set
    public override void GetObjects(List<GameObject> newObjects)
    {
        
    }
    #endregion

}
