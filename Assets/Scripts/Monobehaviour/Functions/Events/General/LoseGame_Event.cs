using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseGame_Event : Event
{
    #region Serializable variables

    [Header("Main Settings")]

    [Tooltip("The lose panel menu object")]
    [SerializeField] GameObject pnl_LoseGame;

    [Tooltip("The time that will take the game to completely freeze after winning, to make an animation and prevent errors")]
    [SerializeField] float timeToFreezeGame;

    [Tooltip("Wait Time to start freezing game")]
    [SerializeField] float timeToStartFreeze;


    [Space]


    [Header("Sound")]


    [Tooltip("It will perform when the player loses. It should have infinite range and set to unscaled time. If you don't need it leave it empty")]
    [SerializeField] GameObject sound;

    #endregion

    #region Private Variables

    private GameObject[] players;

    #endregion

    #region Main Functions
    private void Start()
    {
        //if the lose panel was activated it hides it
        if (pnl_LoseGame.activeSelf)
        {
            pnl_LoseGame.SetActive(false);
        }
        players = GameObject.FindGameObjectsWithTag("Player");
    }
    public override void DoEvent()
    {
        //Show the lose panel menu and make the player inmortal

        if(sound!= null)
        {
            Instantiate(sound, transform.position, transform.rotation);
        }
        foreach (GameObject obj in players)
        {
            if (obj.GetComponent<Health>() != null)
            {
                obj.GetComponent<Health>().SetCanReceiveDamage(false);
            }
            else if (obj.GetComponentInChildren<Health>() != null)
            {
                obj.GetComponentInChildren<Health>().SetCanReceiveDamage(false);
            }
            else if (obj.GetComponentInParent<Health>() != null)
            {
                obj.GetComponentInParent<Health>().SetCanReceiveDamage(false);
            }

            //Makes the player's unable to pause
            if (obj.GetComponent<PlayerMovement>() != null)
            {
                obj.GetComponent<PlayerMovement>().ChangeCanPause(false);
            }
            else if (obj.GetComponentInChildren<PlayerMovement>() != null)
            {
                obj.GetComponentInChildren<PlayerMovement>().ChangeCanPause(false);
            }
            else if (obj.GetComponentInParent<PlayerMovement>() != null)
            {
                obj.GetComponentInParent<PlayerMovement>().ChangeCanPause(false);
            }
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartCoroutine(StartFreezeWait());
    }

    #endregion

    #region Get Set

    public override void GetObjects(List<GameObject> newObjects)
    {

    }

    #endregion

    #region Coroutines

    //Slows the game down until it freezes
    IEnumerator FreezeGame()
    {
        float elapsed = 0f;
        float tempValue = 1f;
        while (elapsed < timeToFreezeGame)
        {
            elapsed += (Time.unscaledDeltaTime / timeToFreezeGame);
            tempValue -= (Time.unscaledDeltaTime / timeToFreezeGame);
            if (tempValue < 0)
            {
                tempValue = 0;
            }
            Time.timeScale = tempValue;
            yield return null;
        }
        if (!pnl_LoseGame.activeSelf)
        {
            pnl_LoseGame.SetActive(true);
        }
        Time.timeScale = 0;
    }
    IEnumerator StartFreezeWait()
    {
        yield return new WaitForSeconds(timeToStartFreeze);
        StartCoroutine(FreezeGame());
    }
    #endregion 
}
