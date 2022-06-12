using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective_Description : MonoBehaviour
{
    //This scripts animates a text object when its objective has been completed

    #region Serializable Variables

    [Tooltip("Object to animate animator")]
    [SerializeField] Animator objectAnimator;

    [Tooltip("Animation duration")]
    [SerializeField] float animTime;

    #endregion

    #region Main Functions

    private void Start()
    {
        //Disables animator to improve performance
        if (objectAnimator.enabled)
        {
            objectAnimator.enabled = false;
        }
    }

    #endregion

    #region Get Set
    public void FinishObjective()
    {
        //Enables the animator and start animation
        if (!objectAnimator.enabled)
        {
            objectAnimator.enabled = true;
        }
        StartCoroutine(FinishAnim());
    }

    #endregion

    #region Coroutines

    // Do the animation, after that the object is destroyed
    IEnumerator FinishAnim()
    {
        objectAnimator.Play("Finish");
        yield return new WaitForSeconds(animTime);
        Destroy(gameObject);
    }

    #endregion
}
