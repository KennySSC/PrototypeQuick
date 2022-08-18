using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_RespawnCam : Event
{
    #region Serializable Variables

    [Tooltip("Current player movement script")]
    [SerializeField] PlayerMovement player;

    [Tooltip("Death camera with animator object")]
    [SerializeField] GameObject deathCam;

    [Tooltip("The wait time before the player cam activates and this respawn cam deactivates")]
    [SerializeField] float respawnCamAnimation = 2.5f;

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
        deathCam.GetComponent<Animator>().SetInteger("AnimVal",1);
        deathCam.GetComponent<Animator>().Play("Respawn");
        StartCoroutine(SwitchCams());

    }

    #endregion

    #region Get Set

    public override void GetObjects(List<GameObject> newObjects)
    {

    }

    #endregion

    #region Coroutines

    private IEnumerator SwitchCams()
    {
        yield return new WaitForSeconds(respawnCamAnimation);
        if (deathCam.activeSelf)
        {
            deathCam.SetActive(false);
        }
        if (!currentCam.activeSelf)
        {
            currentCam.SetActive(true);
        }
    }

    #endregion
}

