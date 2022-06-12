using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ParentObject : MonoBehaviour
{
    //Parent a object if it enters
    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.transform.parent = transform;
    }
    //Unparent a object if it exits
    private void OnTriggerExit(Collider other)
    {
        other.gameObject.transform.parent = null;
    }
}
