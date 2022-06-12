using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Objective_GetCollectible : Objective
{
    #region Serializable variables

    [Header("Main Settings")]


    [Tooltip("Prefab of the collectible object")]
    [SerializeField] GameObject collectiblePrefab;

    [Tooltip("Position where a collectible will be spawned. Assign one for each collectible that you want in the play")]
    [SerializeField] Transform[] spawns;

    [Tooltip("The text that will be displayed to the player")]
    [SerializeField] string description;


    [Space]


    [Header("Events Settings")]


    [Tooltip("Event that tells this script that a collectible has been taken")]
    [SerializeField] Objective_GetCollectible_Event getCollectible_Event;

    [Tooltip("Event that will be triggered once a primary interaction has been performed in a collectible. If you don't need them leave it empty")]
    [SerializeField] List<Event> collectibleEvents_use1 = new List<Event>();

    [Tooltip("Event that will be triggered once a secondary interaction has been performed in a collectible. If you don't need them leave it empty")]
    [SerializeField] List<Event> collectibleEvents_use2 = new List<Event>();

    [Tooltip("Events that will be performed at start")]
    [SerializeField] List<Event> startEvents = new List<Event>();

    [Tooltip("Event that will be triggered once all collectibles had been collected. If you don't need them leave it empty")]
    [SerializeField] List<Event> completeEvents = new List<Event>();


    [Space]


    [Header("Sound")]


    [Tooltip("It will perform when the player complet a goal of the objective. It should have infinite range. If you don't need it leave it empty")]
    [SerializeField] GameObject progressObjectiveSound;


    #endregion

    #region Private Variables

    private List<Interactable> currentCollectibles = new List<Interactable>();
    private ObjectiveManager manager;
    private GameObject textObjectInfo_Game;
    private GameObject textObjectInfo_Pause;

    private bool isComplete = false;

    private int collectiblesLeft;

    #endregion

    #region Main Functions

    private void Start()
    {
        ObjectiveReset();
        AssingEvents();
        foreach (Event evt in startEvents)
        {
            evt.DoEvent();
        }
    }
    public override void ObjectiveReset()
    {
        //Reset variables
        currentCollectibles.Clear();
        foreach (Transform spn in spawns)
        {
           GameObject temp = Instantiate(collectiblePrefab, spn.position, spn.rotation);
           temp.transform.parent = gameObject.transform;
           currentCollectibles.Add(temp.GetComponent<Interactable>());
        }
        collectiblesLeft = spawns.Length;
        UpdateTextInfo();
    }
    public override void ObjectiveComplete()
    {
        //Tell the objective manager that the objective has been completed
        isComplete = true;
        foreach (Event evt in completeEvents)
        {
            evt.DoEvent();
        }
        manager.CompleteObjective(this);
    }
    public override void EvaluateObjectiveProgress()
    {
        if (collectiblesLeft <= 0)
        {
            collectiblesLeft = 0;
            UpdateTextInfo();
            ObjectiveComplete();
        }
        else
        {
            if(progressObjectiveSound!= null)
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
        //Assign the events to all collectibles spawned
        foreach(Interactable inter in currentCollectibles)
        {
            List<Event> use1 = collectibleEvents_use1;
            List<Event> use2 = collectibleEvents_use2;
            use1.Add(getCollectible_Event);
            use2.Add(getCollectible_Event);
            inter.Add_Events_Use1(use1);
            inter.Add_Events_Use2(use2);
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
        return description + " " + collectiblesLeft.ToString();
    }
    public void GetCollectible(int ammount)
    {
        collectiblesLeft -= ammount;
        EvaluateObjectiveProgress();
    }

    #endregion
}
