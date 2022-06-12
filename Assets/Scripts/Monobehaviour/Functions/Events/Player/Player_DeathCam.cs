using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_DeathCam : Event
{
    #region Serializable Variables

    [Tooltip("Current player movement script")]
    [SerializeField] PlayerMovement player;

    [Tooltip("Death camera with animator object")]
    [SerializeField] GameObject deathCam;

    #endregion

    #region private variables

    private GameObject currentCam;

    #endregion


    #region Main Functions

    private void Start()
    {
        currentCam = player.GetBaseCameraMove().GetCamera();
        if (deathCam.activeSelf)
        {
            deathCam.SetActive(false);
        }
    }

    public override void DoEvent()
    {
        if (!deathCam.activeSelf)
        {
            deathCam.SetActive(true);
        }
        if (currentCam.activeSelf)
        {
            currentCam.SetActive(false);
        }
        player.SetChangeView(false);
        deathCam.GetComponent<Animator>().Play("Death");
    }

    #endregion

    #region Get Set

    public override void GetObjects(List<GameObject> newObjects)
    {
       
    }

    #endregion
}
