using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Objective_KillEnemies : Objective
{
    #region Serializable Variables
    [Header("Main Settings")]
    [Tooltip("Enemies quantity that must be killed to complete the objective")]
    [SerializeField] int enemiesCount;

    [Tooltip("Text that will be displayed to the player")]
    [SerializeField] string description;


    [Space]


    [Header("Event Settings")]


    [Tooltip("A event that will tell this script that a enemy has been killed")]
    [SerializeField] Objective_KillEnemies_Event killEnemyEvent;

    [Tooltip("Events that will be asigned to all enemies that will spawn and will be triggered each time one dies")]
    [SerializeField] List<Event> killEnemiesEvents= new List<Event>();

    [Tooltip("Events that will trigger at start")]
    [SerializeField] List<Event> startEvents = new List<Event>();

    [Tooltip("Events that will trigger when the objective has been completed")]
    [SerializeField] List<Event> completeEvents = new List<Event>();


    [Space]


    [Header("Sound")]


    [Tooltip("It will perform when the player complet a goal of the objective. It should have infinite range. If you don't need it leave it empty")]
    [SerializeField] GameObject progressObjectiveSound;

    #endregion

    #region private variables

    private ObjectiveManager manager;
    private GameObject textObjectInfo_Game;
    private GameObject textObjectInfo_Pause;
    private Enemy_Spawner[] spawners;

    private bool isComplete = false;

    private int enemiesLeft;

    #endregion

    #region Main Functions

    private void Start()
    {
        //Find all the spawners in the scene and set variables
        ObjectiveReset();
        spawners = FindObjectsOfType<Enemy_Spawner>();
        AssingEvents();
        foreach (Event evt in startEvents)
        {
            evt.DoEvent();
        }
    }
    public override void ObjectiveComplete()
    {
        //Tell the objective manager that this one has been completed
        isComplete = true;
        //Do complete events
        foreach(Event evt in completeEvents)
        {
            evt.DoEvent();
        }
        manager.CompleteObjective(this);
    }
    public override void ObjectiveReset()
    {
        enemiesLeft = enemiesCount;
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
    public override void EvaluateObjectiveProgress()
    {
        if(enemiesLeft <= 0)
        {
            enemiesLeft = 0;
            UpdateTextInfo();
            ObjectiveComplete();
        }
        else
        {
            if (progressObjectiveSound != null)
            {
                Instantiate(progressObjectiveSound, transform.position, transform.rotation);
            }
        }
        UpdateTextInfo();
    }
    public override void AssingEvents()
    {
        //All the spawners receive the kill enemy events to spawn enemies with the events
        foreach (Enemy_Spawner spawner in spawners)
        {
            List<Event> tempList = killEnemiesEvents;
            tempList.Add(killEnemyEvent);
            spawner.Add_KillEnemyEvent(tempList);
        }
    }
    public override string GetObjectiveDescription()
    {
        return description + " " + enemiesLeft.ToString();
    }
    public override void UpdateTextInfo()
    {
        if(textObjectInfo_Game.GetComponent<Text>()!= null)
        {
            textObjectInfo_Game.GetComponent<Text>().text = GetObjectiveDescription();
        }else if (textObjectInfo_Game.GetComponent<TMP_Text>() != null)
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
    public void KillEnemy(int ammount)
    {
        enemiesLeft -= ammount;
        EvaluateObjectiveProgress();
    }

    #endregion
}
