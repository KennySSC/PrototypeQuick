using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow_Y_Pos : MonoBehaviour
{
    //This script makes the object that has it, to look at a target only rotating in x
    [SerializeField] Transform targetPos;

    private void Update()
    {
        if(targetPos!= null)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, targetPos.position.y, gameObject.transform.position.z);
        }

    }
}
