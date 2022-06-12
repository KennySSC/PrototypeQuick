using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRotation : MonoBehaviour
{
    #region Serializable Variables

    //Copies the rotation of a transform
    [Tooltip("The reference object for the rotation")]
    [SerializeField] Transform EulerAngles;

    [Tooltip("Turn on to copy the angles of the target")]
    [SerializeField] bool SameEulerAngles = false;

    #endregion

    #region Main Functions

    void Start()
    {
        if(EulerAngles != null)
        {
            SameEulerAngles = true;
        }
        else
        {
            SameEulerAngles = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SameEulerAngles)
        {
            transform.localEulerAngles = new Vector3(EulerAngles.eulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
    }

    #endregion

    #region Get Set

    public void ChangeEulerAngles(Transform eulerTarget)
    {
        EulerAngles = eulerTarget;
        if (EulerAngles != null)
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
