using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Open_Door : MonoBehaviour
{
    #region Serializable Variables
    [Header("Main Settings")]


    [Space]


    [Tooltip("The object that renders the door and has a collider")]
    [SerializeField] GameObject door;

    [Tooltip("Door mesh renderer")]
    [SerializeField] MeshRenderer doorRenderer;

    [Tooltip("The material that will have the door if its unlocked")]
    [SerializeField] Material openMaterial;

    [Tooltip("The material that will have the door if its locked")]
    [SerializeField] Material closeMaterial;

    [Tooltip("Lock or unlock the door")]
    [SerializeField] bool canBeOpened = true;


    [Space]


    [Header("Sound")]

    [Tooltip("It will perform each time the door open or closes. If you don't need it leave it empty")]
    [SerializeField] GameObject doorSound;

    #endregion

    #region Private Variables

    private bool isOpen = false;

    [SerializeField]private int currentObjects_Inside = 0;

    private float moveX = 0;

    private Vector3 originalPos;
    private Vector3 finishPos;

    private List<Collider> insideCols = new List<Collider>();

    #endregion

    #region Main Functions

    private void Start()
    {
        //Set variables and door position
        originalPos = door.transform.localPosition;
        finishPos = new Vector3(originalPos.x + 1.1f, originalPos.y, originalPos.z);

        if (canBeOpened)
        {
            doorRenderer.material = openMaterial;
        }
        else if (!canBeOpened)
        {
            doorRenderer.material = closeMaterial;
        }

    }
    //Open the door if something gets inside the trigger and its unlocked
    private void OnTriggerEnter(Collider other)
    {
        if (canBeOpened && (other.gameObject.tag == "Player" || other.gameObject.tag == "Damagable"))
        {
            if (!insideCols.Contains(other))
            {
                insideCols.Add(other);
            }
            if (!isOpen)
            {
                StartCoroutine(OpenDoor());
            }
            currentObjects_Inside++;
        }
    }
    //Closes the door there is nothing on the trigger and its unlocked
    private void OnTriggerExit(Collider other)
    {
        if (canBeOpened && (other.gameObject.tag == "Player" || other.gameObject.tag == "Damagable"))
        {
            currentObjects_Inside--;
            if (insideCols.Contains(other))
            {
                insideCols.Remove(other);
            }
            if (currentObjects_Inside <= 0 && isOpen)
            {
                StartCoroutine(CloseDoor());
            }
        }
    }

    #endregion

    #region Get Set

    //Unlock or lock the door and assign it a material depending on its state
    public void Reverse_OpenClose_Door()
    {
        if (canBeOpened)
        {
            canBeOpened = false;
            doorRenderer.material = closeMaterial;
            if (isOpen)
            {
                StartCoroutine(CloseDoor());
            }

        }
        else
        {
            canBeOpened = true;
            doorRenderer.material = openMaterial;
            if (currentObjects_Inside > 0)
            {
                StartCoroutine(OpenDoor());
            }
        }
    }
    public bool GetIsUnlocked()
    {
        return canBeOpened;
    }
    #endregion

    #region Coroutines

    IEnumerator OpenDoor()
    {
        float elapsed = 0;
        while (elapsed < 0.5)
        {
            elapsed += Time.deltaTime;
            moveX += Time.deltaTime;
            door.transform.localPosition = new Vector3(originalPos.x + moveX, door.transform.localPosition.y, door.transform.localPosition.z);
            yield return null;
        }
        isOpen = true;
        moveX = 1;
        if(doorSound != null)
        {
            Instantiate(doorSound, door.transform.position, door.transform.rotation);
        }
        door.transform.localPosition = finishPos;
    }
    IEnumerator CloseDoor()
    {
        float elapsed = 0;
        door.transform.localPosition = finishPos;
        while (elapsed < 0.5)
        {
            elapsed += Time.deltaTime;
            moveX -= Time.deltaTime;
            door.transform.localPosition = new Vector3(finishPos.x - moveX, door.transform.localPosition.y, door.transform.localPosition.z);
            yield return null;
        }
        door.transform.localPosition = originalPos;
        moveX = 0;
        if (doorSound != null)
        {
            Instantiate(doorSound, door.transform.position, door.transform.rotation);
        }
        isOpen = false;
    }

    #endregion
}
