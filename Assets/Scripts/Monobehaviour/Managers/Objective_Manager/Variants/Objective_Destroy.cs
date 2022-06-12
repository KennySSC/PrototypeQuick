using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Objective_Destroy : Objective
{
    #region Serializable Variables

    [Header("Main Settings")]


    [Tooltip("Objects with health script that the player must destroy")]
    [SerializeField] Health[] objects_ToDestroy;

    [Tooltip("Description that will be displayer to the player")]
    [SerializeField] string description;


    [Space]


    [Header("Events settings")]


    [Tooltip("Event that will tell to this script that a target has been destroyed")]
    [SerializeField] Objective_Destroy_Event destroyEvent;


    [Tooltip("Events that will be assigned to all objects to destroy. They will perform when the object is destroyed. If you don't need events leave it empty")]
    [SerializeField] List<Event> detroyObject_Events = new List<Event>();

    [Tooltip("Events that will be performed at start")]
    [SerializeField] List<Event>startEvents = new List<Event>();

    [Tooltip("Events that will be performed when the objective is complete")]
    [SerializeField] List<Event> completeEvents = new List<Event>();


    [Space]


    [Header("Sound")]


    [Tooltip("It will perform when the player complet a goal of the objective. It should have infinite range. If you don't need it leave it empty")]
    [SerializeField] GameObject progressObjectiveSound;

    #endregion

    #region Private Variables

    private ObjectiveManager manager;
    private GameObject textObjectInfo_Game;
    private GameObject textObjectInfo_Pause;

    private int objectsCount;

    private bool isComplete = false;

    #endregion

    #region Main Functions

    private void Start()
    {
        ObjectiveReset();
        foreach (Event evt in startEvents)
        {
            evt.DoEvent();
        }
    }
    public override void ObjectiveReset()
    {
        //Start objective
        objectsCount = objects_ToDestroy.Length;
        UpdateTextInfo();
        AssingEvents();
    }
    public override void ObjectiveComplete()
    {
        //Tell objective manager that this objective is complete
        isComplete = true;
        foreach (Event evt in completeEvents)
        {
            evt.DoEvent();
        }
        manager.CompleteObjective(this);
    }
    public override void EvaluateObjectiveProgress()
    {
        if (objectsCount <= 0)
        {
            objectsCount = 0;
            UpdateTextInfo();
            ObjectiveComplete();
        }
        else
        {
            if(progressObjectiveSound != null)
            {
                Instantiate(progressObjectiveSound, transform.position, transform.rotation);
            }
        }
        UpdateTextInfo();
    }

    #endregion

    #region Get Set

    public override void SetManager(ObjectiveManager newManager)
    {
        manager = newManager;   
    }
    public override void SetTextInfo(GameObject textObjectGame, GameObject textObjectPause)
    {
        textObjectInfo_Game = textObjectGame;
        textObjectInfo_Pause = textObjectPause;
    }
    public override void AssingEvents()
    {
        // Assign events
        foreach(Health hp in objects_ToDestroy)
        {
            List<Event> tempList = detroyObject_Events;
            if (!tempList.Contains(destroyEvent))
            {
                tempList.Add(destroyEvent);
            }
            hp.Add_DeadEvents(tempList);
        }
    }
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
    public override bool GetIsComplete()
    {
        return isComplete;
    }
    public override string GetObjectiveDescription()
    {
        return description + " " + objectsCount;
    }
    public void DestroyObjective(int ammount)
    {
        objectsCount -= ammount;
        EvaluateObjectiveProgress();
    }

    #endregion 
}
