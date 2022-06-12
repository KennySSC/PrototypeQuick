using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InstaKill : MonoBehaviour
{
    #region Serializable Variables

    [Tooltip("Allows the trigger to kill enemies if they enter to it")]
    [SerializeField] bool killEnemies;

    [Tooltip("Allows the trigger to kill players if they enter to it")]
    [SerializeField] bool killPlayers;

    [Tooltip("Allows the trigger to kill bosses if they enter to it")]
    [SerializeField] bool killBosses;

    #endregion

    #region Main Functions

    private void OnTriggerEnter(Collider other)
    {
        //Deal great damage to the selected targets
        if (other.gameObject.GetComponent<Health>() != null)
        {
            if(killPlayers && other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponent<Health>().GetDamage(999999);
            }
            if(killEnemies && other.gameObject.tag == "Damagable" )
            {
                other.gameObject.GetComponent<Health>().GetDamage(999999);
            }
        }
        else if(other.gameObject.GetComponentInChildren<Health>() != null)
        {
            if (killPlayers && other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponentInChildren<Health>().GetDamage(999999);
            }
            if (killEnemies && other.gameObject.tag == "Damagable")
            {
                other.gameObject.GetComponentInChildren<Health>().GetDamage(999999);
            }
        }
        else if (other.gameObject.GetComponentInParent<Health>() != null)
        {
            if (killPlayers && other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponentInParent<Health>().GetDamage(999999);
            }
            if (killEnemies && other.gameObject.tag == "Damagable")
            {
                other.gameObject.GetComponentInParent<Health>().GetDamage(999999);
            }
        }
        if(killBosses && other.GetComponent<BossHealthManager>()!= null)
        {
            other.GetComponent<BossHealthManager>().GetDamage(999999);
        }
        else if (killBosses && other.GetComponentInChildren<BossHealthManager>() != null)
        {
            other.GetComponentInChildren<BossHealthManager>().GetDamage(999999);
        }
        else if (killBosses && other.GetComponentInParent<BossHealthManager>() != null)
        {
            other.GetComponentInParent<BossHealthManager>().GetDamage(999999);
        }
    }

    #endregion
}
