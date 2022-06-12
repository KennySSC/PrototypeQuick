using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePerSecond : MonoBehaviour
{
    #region Serializable variables

    [Tooltip("This value will apply every second to all objects inside ths trigger every second. Also Applies as soon as the object enters")]
    [SerializeField] int damageAmmount;

    #endregion

    #region Private variables

    private List<Health> hpInside = new List<Health>();

    private float second = 1f;
    private float resetSecond = 1f;

    #endregion

    #region Main functions
    private void Start()
    {
        //Reset timers
        resetSecond = second;
        second = 0;
    }
    private void Update()
    {
        //Waits a second to apply damage
        if (second > 0)
        {
            second -= Time.deltaTime;
            if (second <= 0)
            {
                //Applies the damage to all the objects inside the zone
                foreach(Health hp in hpInside)
                {
                    hp.GetDamage(damageAmmount);
                }
                second = resetSecond;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //Applies damage if the object has a Health script and saves it to apply damage every second inside
        if(other.gameObject.GetComponent<Health>()!= null)
        {
            other.gameObject.GetComponent<Health>().GetDamage(damageAmmount);
            if (!hpInside.Contains(other.gameObject.GetComponent<Health>()))
            {
                hpInside.Add(other.gameObject.GetComponent<Health>());
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        //Remove the object from the list
        if (other.gameObject.GetComponent<Health>() != null)
        {
            if (hpInside.Contains(other.gameObject.GetComponent<Health>()))
            {
                hpInside.Remove(other.gameObject.GetComponent<Health>());
            }
        }
    }
    #endregion
}
