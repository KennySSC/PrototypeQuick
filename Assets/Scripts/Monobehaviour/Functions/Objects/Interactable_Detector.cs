using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Detector : MonoBehaviour
{
    #region Serializable Variables

    [Header("Main Settings")]


    [Tooltip("Player object")]
    [SerializeField] GameObject player;


    [Space]


    [Header("Debug variables")]

    [Tooltip("Current interactables inside zone")]
    [SerializeField]private List<Interactable> interactableList = new List<Interactable>();

    #endregion

    #region Main Functions
    //Detects if interactable objects are inside the area
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Interactable" && other.GetComponent<Interactable>() != null)
        {
            if (!interactableList.Contains(other.GetComponent<Interactable>()))
            {
                interactableList.Add(other.GetComponent<Interactable>());
            }
        }
    }
    //Detects if interactable objects exits the area
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Interactable" && other.GetComponent<Interactable>() != null)
        {
            if (interactableList.Contains(other.GetComponent<Interactable>()))
            {
                other.GetComponent<Interactable>().Miss(player);
                interactableList.Remove(other.GetComponent<Interactable>());
            }
        }
    }

    #endregion

    #region Get Set
    //Call this to obtain the first interactable that entered to the area
    public Interactable TopInteractable()
    {
        if(interactableList.Count > 0 && interactableList != null)
        {
            return interactableList[0];
        }
        else
        {
            return null;
        }

    }
    //Call this to remove null references in the list
    public void RemoveInteractable()
    {
        foreach(Interactable inte in interactableList)
        {
            if(inte == null)
            {
                interactableList.Remove(inte);
                break;
            }
        }
    }
    #endregion
}
