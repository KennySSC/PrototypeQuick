using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayZone_Player : MonoBehaviour
{
    #region Serializable variables
    [Header("Main Settings")]

    [Tooltip("Ammount of time that the player needs to stay in it to trigger events")]
    [SerializeField] float stayTime;

    [Tooltip("Events that will be triggered")]
    [SerializeField] List<Event> events = new List<Event>();


    [Space]


    [Header("Sound")]

    [Tooltip("Performs when a player enters to the zone. If you don't need it leave it empty")]
    [SerializeField] GameObject enterSound;

    [Tooltip("Performs when a player exits of the zone. If you don't need it, leave it empty")]
    [SerializeField] GameObject exitSound;


    [Space]


    [Header("Feedback settings")]

    [Tooltip("Floor sprite that will change color depending on the progress of the stay time")]
    [SerializeField] SpriteRenderer floorObject;
    [SerializeField] Color noOneInsideColor;
    [SerializeField] Color someoneInsideColor;
    [SerializeField] Color finishingColor;

    #endregion

    #region Private variables

    private float stayTime_reset;
    private int currentPlayersInside = 0;
    private bool finished = false;

    #endregion

    #region Main Functions
    private void Start()
    {
        stayTime_reset = stayTime;
    }
    private void Update()
    {
        //Only rests time when at least a player is inside
        if(currentPlayersInside > 0 && !finished)
        {

            stayTime -= Time.deltaTime;
            float temp = stayTime / stayTime_reset;

            if (stayTime <= 0)
            {
                FinishZone();
            }
            if (temp < 0)
            {
                temp = 0;
            }
            floorObject.color = Color.Lerp(finishingColor, someoneInsideColor, temp); 
        }
        //If there aren't players inside, the time will reset
        else if(!finished)
        {
            stayTime = stayTime_reset;
        }
    }
    //Trigger events
    private void FinishZone()
    {
        finished = true;
        floorObject.color = finishingColor;
        foreach (Event evt in events)
        {
            evt.DoEvent();
        }
    }
    //Detects if a player enters
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (enterSound != null)
            {
                Instantiate(enterSound, transform.position, transform.rotation);
            }
            if (currentPlayersInside == 0 && !finished)
            {
                floorObject.color = someoneInsideColor;
            }
                currentPlayersInside++;

        }
    }
    //Detect if a player leaves
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (exitSound != null)
            {
                Instantiate(exitSound, transform.position, transform.rotation);
            }
            currentPlayersInside--;
            if(currentPlayersInside == 0)
            {
                if (!finished)
                {
                    floorObject.color = noOneInsideColor;
                }
            }
        }
    }

    #endregion

    #region Get Set

    //Call this function to add events externally
    public void Add_Events(List<Event> newEvents)
    {
        foreach(Event evt in newEvents)
        {
            if (!events.Contains(evt))
            {
                events.Add(evt);
            }
        }
    }

    //If you need to know if the zone has been completed externally, call this function
    public bool GetFinished()
    {
        return finished;
    }
    public void SetStayTime(float newTime)
    {
        stayTime = newTime;
        stayTime_reset = newTime;
    }

    #endregion
}
