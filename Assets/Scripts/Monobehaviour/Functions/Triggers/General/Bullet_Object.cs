using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Object : MonoBehaviour
{
    
    [SerializeField] float speed;
    [Tooltip("An empty object, that will be used if a shot hits and follow the hit position")]
    [SerializeField] Transform myPositionTransform;

    bool followRayCast = false;

    Vector3 Origin;

    private void Awake()
    {
        myPositionTransform.parent = null;
        Origin = myPositionTransform.position;
        followRayCast = false;
    }

    private void Update()
    {
        if (!followRayCast)
        {
            transform.localPosition += (transform.forward * speed * Time.deltaTime);
        }
        else if(followRayCast && myPositionTransform.position != Origin)
        {
            transform.localPosition += (transform.forward * speed * Time.deltaTime);
            transform.LookAt(myPositionTransform.position);
        }
        else
        {
            transform.localPosition += (transform.forward * speed * Time.deltaTime);
        }

    }
    public void ChangeFollowRay(bool newValue)
    {
        followRayCast = newValue;

    }
    public void ChangeDirectionPos(Vector3 newPosition)
    {
        if (followRayCast)
        {
            myPositionTransform.position = newPosition;
            transform.LookAt(myPositionTransform.position);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "FX")
        {
            Destroy(gameObject);
        }

    }
}
