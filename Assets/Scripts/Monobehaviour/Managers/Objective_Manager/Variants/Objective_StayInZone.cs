using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Objective_StayInZone : Objective
{
    #region Serializable Variables

    [Header("Main Settings")]


    [Tooltip("Triggers where the player must stay to finish this objective")]
    [SerializeField] StayZone_Player[] zones;

    [Tooltip("Time that the player must stay inside a zone")]
    [SerializeField] float stayTime;

    [Tooltip("Text that will be displayed to the player")]
    [SerializeField] string description;


    [Space]


    [Header("Event Settings")]


    [Tooltip("Event to detect that player stayed the time in the zones")]
    [SerializeField] Objective_ZoneComplete_Event completeZone_Event;

    [Tooltip("Any script that inherits from Event script. The events will be asigned to the stay triggers automatically")]
    [SerializeField] List<Event> events = new List<Event>();

    [Tooltip("Events that will trigger at start")]
    [SerializeField] List<Event> startEvents = new List<Event>();

    [Tooltip("Any script that inherits from Event script. This events will trigger when the objective is completed. If you dont need any leave it empty")]
    [SerializeField] List<Event> completeEvents = new List<Event>();


    [Space]


    [Header("Sound")]


    [Tooltip("It will perform when the player complet a goal of the objective. It should have infinite range. If you don't need it leave it empty")]
    [SerializeField] GameObject progressObjectiveSound;

    #endregion

    #region Private variables

    private ObjectiveManager manager;
    private GameObject textObjectInfo_Game;
    private GameObject textObjectInfo_Pause;
    private bool isComplete = false;
    private int remainingZones =0;

    #endregion

    #region Main Functions

    private void Start()
    {
        //Starts the objective
        ObjectiveReset();
        AssingEvents();
        foreach (Event evt in startEvents)
        {
            evt.DoEvent();
        }
    }
    public override void ObjectiveReset()
    {
        //Sets the ammount of zones that must be done
        remainingZones = zones.Length;
        foreach(StayZone_Player zone in zones)
        {
            zone.SetStayTime(stayTime);
        }
        UpdateTextInfo();
    }

    public override void ObjectiveComplete()
    {
        //Do complete events
        isComplete = true;
        foreach (Event evt in completeEvents)
        {
            evt.DoEvent();
        }
        //Tells to the manager that the objective it's complete
        manager.CompleteObjective(this);
    }
    //Evaluates if all the zones are completed,
    public override void EvaluateObjectiveProgress()
    {
        for(int i =0; i<zones.Length; i++)
        {
            if (zones[i].GetFinished() == false)
            {
                UpdateTextInfo();
                isComplete = false;
                if(progressObjectiveSound != null)
                {
                    Instantiate(progressObjectiveSound, transform.position, transform.rotation);
                }
                UpdateTextInfo();
                return;
            }
        }
        isComplete = true;
        UpdateTextInfo();
        ObjectiveComplete();
  
    }

    #endregion

    #region Get Set

    // Set the manager
    public override void SetManager(ObjectiveManager newManager)
    {
        manager = newManager;
    }
    //Set the object that will receive the description
    public override void SetTextInfo(GameObject textObjectGame, GameObject textObjectPause)
    {
        textObjectInfo_Game = textObjectGame;
        textObjectInfo_Pause = textObjectPause;
    }
    //Assign events to the zones
    public override void AssingEvents()
    {
        List<Event> tempEvents = events;

        if (!tempEvents.Contains(completeZone_Event))
        {
            tempEvents.Add(completeZone_Event);
        }
        foreach(StayZone_Player zone in zones)
        {
            zone.Add_Events(tempEvents);
        }
    }
    //Updates the progress of the objective
    public override void UpdateTextInfo()
    {
        if (textObjectInfo_Game.GetComponent<Text>() != null)
        {
            textObjectInfo_Game.GetComponent<Text>().text = GetObjectiveDescription();
        }
        else if (textObjectInfo_Game.GetComponent<TMP_Text>() != null)
        {
            textObjectInfo_Game.GetComponent<TMP_Text>().text = GetObjectiveDescription();
        }
        if (textObjectInfo_Pause.GetComponent<Text>() != null)
        {
            textObjectInfo_Pause.GetComponent<Text>().text = GetObjectiveDescription();
        }
        else if (textObjectInfo_Pause.GetComponent<TMP_Text>() != null)
        {
            textObjectInfo_Pause.GetComponent<TMP_Text>().text = GetObjectiveDescription();
        }
    }
    //If you need to know if this objective it's complete externally, call this function
    public override bool GetIsComplete()
    {
        return isComplete;
    }
    // If you need to get the current progress of this objective via text, call this function
    public override string GetObjectiveDescription()
    {
        return description + " " + remainingZones.ToString();
    }
    //Rest a zone and evaluates the progress
    public void CompleteZone()
    {
        remainingZones--;
        EvaluateObjectiveProgress();
    }

    #endregion 
}
