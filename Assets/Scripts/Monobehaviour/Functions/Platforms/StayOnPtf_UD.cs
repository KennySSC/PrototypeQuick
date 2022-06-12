using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]

public class StayOnPtf_UD : MonoBehaviour
{
    [SerializeField] Move_UpDown movePtf;

    PlayerMovement mv;

    private void Start()
    {
        mv = FindObjectOfType<PlayerMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(mv!= null)
            {
                mv.SetCurrentPf_UD(movePtf, true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (mv != null)
            {
                mv.SetCurrentPf_UD(null, false);
            }
        }
    }
}
