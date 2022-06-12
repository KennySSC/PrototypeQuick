using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporObject : MonoBehaviour
{
    //Makes a game object to follow the position of another game object
    #region Serializable Variables
    [Tooltip("The object that will be followed")]
    [SerializeField] Transform objectToFollow;

    [Tooltip("The reference object for the rotation")]
    [SerializeField] Transform EulerAngles;

    [Tooltip("Smooth ammount for the follow movement")]
    [SerializeField] float smooth;

    [Tooltip("Turn on to copy the angles of the Euler angles reference object")]
    [SerializeField] bool SameEulerAngles = false;

    #endregion

    #region Private Variables

    private bool hasObject = false;

    #endregion

    #region Main functions
    private void Start()
    {
        if(objectToFollow!=null)
        {
            hasObject = true;
        }
        if(EulerAngles != null)
        {
            SameEulerAngles = true;
        }
    }
    void Update()
    {
        //Follow the object if it has one
        if (hasObject)
        {
            Vector3 originalPos = transform.position;
            Vector3 targetPos = Vector3.Lerp(originalPos, objectToFollow.position, smooth);
            transform.position = targetPos;
            if (SameEulerAngles)
            {
                transform.localEulerAngles = EulerAngles.localEulerAngles;
            }

        }


    }

    #endregion

    #region Get Set

    //Set the target
    public void ChangeTarget(Transform target)
    {
        objectToFollow = target;
        if(objectToFollow != null)
        {
            hasObject = true;
        }
        else
        {
            hasObject = false;
        }
    }
    public void ChangeEulerAngles(Transform eulerTarget)
    {
        EulerAngles = eulerTarget;
        if(EulerAngles!= null)
        {
            SameEulerAngles = true;
        }
        else
        {
            SameEulerAngles = false;
        }
    }

    #endregion
}
