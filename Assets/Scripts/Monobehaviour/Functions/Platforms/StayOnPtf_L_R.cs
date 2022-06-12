using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]

public class StayOnPtf_L_R : MonoBehaviour
{
    [SerializeField] Move_LeftRight movePtf;

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
                mv.SetCurrentPf_L_R(movePtf, true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (mv != null)
            {
                mv.SetCurrentPf_L_R(null, false);
            }
        }
    }
}
