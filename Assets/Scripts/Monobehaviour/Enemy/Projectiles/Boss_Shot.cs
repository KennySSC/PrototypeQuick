using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Shot : MonoBehaviour
{
    #region Serializable Variables
    [Header("Particles & Sound")]

    [Tooltip("Appears when the shot hits")]
    [SerializeField] GameObject sound;

    [Tooltip("Appears when the shot hits")]
    [SerializeField] GameObject particle;

    [Space]

    [Header("Debug variables")]

    [SerializeField] Rigidbody rb;

    [SerializeField] int damage = 0;

    [SerializeField] float speed = 0.1f;

    [SerializeField] float radius = 0.5f;

    #endregion

    #region Main Functions

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        //Applies force
        if (rb.velocity.magnitude < speed)
        {
            rb.AddForce(speed * 100 * transform.forward * Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //Explodes when hit to the player and applies damage
        if (other.gameObject.tag == "Player")
        {
            Instantiate(particle, transform.position, transform.rotation);
            if (other.gameObject.GetComponent<Health>())
            {
                other.gameObject.GetComponent<Health>().GetDamage(damage);
            }
            Destroy(gameObject);
        }
    }

    #endregion

    #region Get Set
    //Set projectile values
    public void SetValues(float newSpeed, float newRadius, int newDamage)
    {
        speed = newSpeed;
        radius = newRadius * 2;
        transform.localScale = new Vector3(radius, radius , radius);
        radius = radius / 2;
        damage = newDamage;
    }

    #endregion
}
