using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Objective_KillBoss : Objective
{
    #region Serializable Variables

    [Header("Main Settings")]


    [Tooltip("Boss GameObject with all boss logic")]
    [SerializeField] GameObject bossObject;

    [Tooltip("Text that will be displayed to the player")]
    [SerializeField] string description;


    [Space]


    [Header("Event Settings")]


    [Tooltip("Event that tell this script that the boss has been killed")]
    [SerializeField] Objective_KillBoss_Event killEvent;


    [Tooltip("Events that will trigger at start")]
    [SerializeField] List<Event> startEvents = new List<Event>();


    [Tooltip("Events that will trigger once the objective has beeen completed")]
    [SerializeField] List<Event> completeEvents = new List<Event>();

    #endregion

    #region private Variables

    private GameObject textObjectInfo_Game;
    private GameObject textObjectInfo_Pause;
    private ObjectiveManager manager;

    private bool isComplete = false;

    #endregion

    #region Main Functions

    void Start()
    {
        AssingEvents();
        UpdateTextInfo();
        foreach(Event evt in startEvents)
        {
            evt.DoEvent();
        }
    }
    public override void ObjectiveReset()
    {

    }
    public override void ObjectiveComplete()
    {
        //Tell the objective manager that the objective has been completed
        isComplete = true;
        foreach(Event evt in completeEvents)
        {
            evt.DoEvent();
        }
        manager.CompleteObjective(this);
    }
    public override void EvaluateObjectiveProgress()
    {

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
        //Assign the kill event to the boss
        if(bossObject.GetComponent<BossHealthManager>()!= null)
        {
            List<Event> tempList = new List<Event>();
            tempList.Add(killEvent);
            bossObject.GetComponent<BossHealthManager>().Add_DeadEvents(tempList);
        }
        else if (bossObject.GetComponentInParent<BossHealthManager>() != null)
        {
            List<Event> tempList = new List<Event>();
            tempList.Add(killEvent);
            bossObject.GetComponentInParent<BossHealthManager>().Add_DeadEvents(tempList);
        }
        else if(bossObject.GetComponentInChildren<BossHealthManager>() != null)
        {
            List<Event> tempList = new List<Event>();
            tempList.Add(killEvent);
            bossObject.GetComponentInChildren<BossHealthManager>().Add_DeadEvents(tempList);
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
        return description;
    }

    #endregion
}
