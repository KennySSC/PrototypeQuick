using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    #region Serializable Variables

    [Header("Main Settings")]


    [Tooltip("All the objective scripts that you want the script to choose from")]
    [SerializeField] GameObject[] objectives;

    [Tooltip("The amount of objectives that will be used on a play")]
    [SerializeField] int objectivesPerPlay;
    

    [Space]


    [Header("Events settings")]


    [Tooltip("Events that will trigger when all objectives has been completed")]
    [SerializeField] List<Event> winEvents = new List<Event>();


    [Space]


    [Header("UI Settings")]


    [Tooltip("A prefab that must have a Text or TMP_Text component to show objectives description")]
    [SerializeField] GameObject uiTextPrefab;

    [Tooltip("The container of each objective of the play")]
    [SerializeField] GameObject objectiveTextContainer;


    [Space]


    [Header("Sound")]


    [Tooltip("It will perform when a objective has been completed. It should have infinite range. If you don't need it leave it empty")]
    [SerializeField] GameObject completeObjectiveSound;


    #endregion

    #region private variables
    private Transform ui_Objective_Container;
    private List<Objective> currentObjectives = new List<Objective>();
    [SerializeField] private List<GameObject> uiTexts = new List<GameObject>();
    [SerializeField] private List<GameObject> pauseTexts = new List<GameObject>();
    private UI_Controller controller;

    private List<GameObject> notUsed_ObjectivesObj = new List<GameObject>();
    private List<GameObject> currentObj = new List<GameObject>();
    private bool canContinue = false;

    #endregion

    #region Main Functions

    private void Awake()
    {
        Time.timeScale = 1;
        controller = FindObjectOfType<UI_Controller>();
        ui_Objective_Container = controller.GetContentObject().transform;
        //Verifies that at least one objective will be choosen
        if (objectivesPerPlay <= 0 && objectives.Length >0)
        {
            objectivesPerPlay = 1;
        }else if (objectivesPerPlay > objectives.Length && objectives.Length >0)
        {
            //verifies that the objectives of the play be more than the available
            objectivesPerPlay = objectives.Length;
        }
        else
        {
            
        }
        //Select random objectives and verify that they doen't repeat
        for(int i = 0; i<objectivesPerPlay; i++)
        {
            canContinue = false;
            do
            {
                int temp = Random.Range(0, objectives.Length);
                if (!currentObjectives.Contains(objectives[temp].GetComponent<Objective>()))
                {
                    currentObjectives.Add(objectives[temp].GetComponent<Objective>());
                    currentObj.Add(objectives[temp]);
                    canContinue = true;
                }

            } while (!canContinue);

        }
        //Creates text prefabs and set the descriptions
        foreach (Objective goal in currentObjectives)
        {
            GameObject uiShow = Instantiate(uiTextPrefab, objectiveTextContainer.transform.position, objectiveTextContainer.transform.rotation);
            GameObject uiPause = Instantiate(uiTextPrefab, ui_Objective_Container.position, ui_Objective_Container.rotation);
            uiShow.transform.SetParent(objectiveTextContainer.transform);
            uiPause.transform.SetParent(ui_Objective_Container);
            if (uiTextPrefab.GetComponent<Text>() != null)
            {
                uiShow.GetComponent<Text>().text = goal.GetObjectiveDescription();
                uiPause.GetComponent<Text>().text = goal.GetObjectiveDescription();
            }else if(uiTextPrefab.GetComponent<TMP_Text>() != null)
            {
                uiShow.GetComponent<TMP_Text>().text = goal.GetObjectiveDescription();
                uiPause.GetComponent<TMP_Text>().text = goal.GetObjectiveDescription();
            }
            if (!uiTexts.Contains(uiShow))
            {
                uiTexts.Add(uiShow);
            }
            if (!pauseTexts.Contains(uiPause))
            {
                pauseTexts.Add(uiPause);
            }
            goal.SetManager(this);
            goal.SetTextInfo(uiShow, uiPause);
        }

        //Disables not used objectives
        foreach(GameObject allobj in objectives)
        {
            if (!currentObj.Contains(allobj))
            {
                notUsed_ObjectivesObj.Add(allobj.gameObject);
                
            }
        }
        foreach(GameObject gm in notUsed_ObjectivesObj)
        {
            if (gm.activeSelf)
            {
                gm.SetActive(false);
            }
        }
        StartCoroutine(ActivatePause_ObjectivesPnl());
    }
    //Call this to tell this script that one objective has been completed
    public void CompleteObjective(Objective currentCompleted)
    {
        //verifies if all objectives has been completed
        if (currentObjectives.Contains(currentCompleted))
        {
            if (uiTextPrefab.GetComponent<Text>() != null)
            {
                Debug.Log("Has normal text");
                GameObject tempUi = null;
                GameObject tempPause = null;
                foreach (GameObject obj in uiTexts)
                {
                    if (currentCompleted.GetObjectiveDescription() == obj.GetComponent<Text>().text)
                    {
                        Debug.Log("Found normal text");
                        obj.GetComponent<Objective_Description>().FinishObjective();
                        tempUi = obj;
                    }
                }
                foreach (GameObject obj in pauseTexts)
                {
                    if (currentCompleted.GetObjectiveDescription() == obj.GetComponent<Text>().text)
                    {
                        obj.GetComponent<Text>().text = "Completed";
                        obj.GetComponent<Text>().color = Color.green;
                        obj.GetComponent<Text>().text = "Completed";
                        tempPause = obj;
                    }
                }
                if (uiTexts.Contains(tempUi))
                {
                    uiTexts.Remove(tempUi);
                }
                if (pauseTexts.Contains(tempPause))
                {
                    pauseTexts.Remove(tempPause);
                }
            }
            else if (uiTextPrefab.GetComponent<TMP_Text>() != null)
            {
                Debug.Log("Has TMP");
                GameObject tempUi = null;
                GameObject tempPause = null;
                foreach (GameObject obj in uiTexts)
                {
                    if (currentCompleted.GetObjectiveDescription() == obj.GetComponent<TMP_Text>().text)
                    {
                        Debug.Log("Found TMP");
                        obj.GetComponent<Objective_Description>().FinishObjective();
                        tempUi = obj;
                    }
                }
                foreach (GameObject obj in pauseTexts)
                {
                    if (currentCompleted.GetObjectiveDescription() == obj.GetComponent<TMP_Text>().text)
                    {
                        obj.GetComponent<TMP_Text>().text = "Completed";
                        obj.GetComponent<TMP_Text>().color = Color.green;
                        obj.GetComponent<TMP_Text>().text = "Completed";
                        tempPause = obj;
                    }
                }
                if (uiTexts.Contains(tempUi))
                {
                    uiTexts.Remove(tempUi);
                }
                if (pauseTexts.Contains(tempPause))
                {
                    pauseTexts.Remove(tempPause);
                }
            }

            currentObjectives.Remove(currentCompleted);
      
        }
        if(currentObjectives.Count <= 0)
        {
            foreach(Event evt in winEvents)
            {
                evt.DoEvent();
            }
        }
        else
        {
            if (completeObjectiveSound != null)
            {
                Instantiate(completeObjectiveSound, transform.position, transform.rotation);
            }
        }
    }

    #endregion


    #region Coroutines
    IEnumerator ActivatePause_ObjectivesPnl()
    {
        yield return new WaitForSeconds(0.3f);
        controller.ActivateObjectivesPanel();
    }

    #endregion
}
