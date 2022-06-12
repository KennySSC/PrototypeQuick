using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractForEvent : Interactable
{
    #region Serializable Variables

    [Header("Main Settings")]


    [Tooltip("Events that will trigger with the primary interaction")]
    [SerializeField] List<Event> use1_Events = new List<Event>();

    [Tooltip("Events that will trigger with the secondary interaction")]
    [SerializeField] List<Event> use2_Events = new List<Event>();

    [Tooltip("Image displayed to the player representing use1. If you don't need it you can leave it empty")]
    [SerializeField] Sprite sprt1;

    [Tooltip("Image displayed to the player representing use2. If you don't need it you can leave it empty")]
    [SerializeField] Sprite sprt2;

    [Tooltip("Text displayed with additional info of use1. If you don't need it you can leave it empty")]
    [SerializeField] string extraInfo1;

    [Tooltip("Text displayed with additional info of use2. If you don't need it you can leave it empty")]
    [SerializeField] string extraInfo2;

    [SerializeField] bool destroyAfterUse1;

    [SerializeField] bool destroyAfterUse2;


    [Space]


    [Header("Sound")]


    [Tooltip("It will perform when the use1 activates. If you don't need it, leave it empty")]
    [SerializeField] GameObject use1Sound;

    [Tooltip("It will perform when the use2 activates. If you don't need it, leave it empty")]
    [SerializeField] GameObject use2Sound;

    #endregion

    #region Private variables

    private MeshRenderer[] renderers;
    private List<Material> materials = new List<Material>();

    #endregion

    #region Main Functions
    private void Awake()
    {
        //Find all the mesh renderers of the object to set outline
        if (GetComponent<MeshRenderer>() != null)
        {
            renderers = GetComponents<MeshRenderer>();
            foreach (MeshRenderer rnd in renderers)
            {
                Material[] tempMaterials = rnd.materials;
                foreach(Material mtl in tempMaterials)
                {
                    if (!materials.Contains(mtl))
                    {
                        materials.Add(mtl);
                    }
                }
            }
        }
        if (GetComponentInChildren<MeshRenderer>() != null)
        {
            renderers = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer rnd in renderers)
            {
                Material[] tempMaterials = rnd.materials;
                foreach (Material mtl in tempMaterials)
                {
                    if (!materials.Contains(mtl))
                    {
                        materials.Add(mtl);
                    }
                }
            }
        }
        if (GetComponentInParent<MeshRenderer>() != null)
        {
            renderers = GetComponentsInParent<MeshRenderer>();
            foreach (MeshRenderer rnd in renderers)
            {
                Material[] tempMaterials = rnd.materials;
                foreach (Material mtl in tempMaterials)
                {
                    if (!materials.Contains(mtl))
                    {
                        materials.Add(mtl);
                    }
                }
            }
        }
        //Hides outline
        if(materials != null)
        {
            foreach (Material mtl in materials)
            {
                mtl.SetInt("Show_Outline", 0);
            }
        }

    }
    //Shows outline
    public override void Found(GameObject whoFind)
    {
        if (materials != null)
        {
            foreach (Material mtl in materials)
            {
                mtl.SetInt("Show_Outline", 1);
            }
        }
        if (whoFind.GetComponent<PlayerMovement>() != null)
        {
            whoFind.GetComponent<PlayerMovement>().GetMovement().SetSpritesInfo(sprt1 , sprt2);
            whoFind.GetComponent<PlayerMovement>().GetMovement().SetExtraInfo(extraInfo1, extraInfo2);
        }
    }
    //Hides outline
    public override void Miss(GameObject whoMiss)
    {
        if (materials != null)
        {
            foreach (Material mtl in materials)
            {
                mtl.SetInt("Show_Outline", 0);
            }
        }
        if (whoMiss.GetComponent<PlayerMovement>() != null)
        {
            whoMiss.GetComponent<PlayerMovement>().GetMovement().SetSpritesInfo(null, null);
            whoMiss.GetComponent<PlayerMovement>().GetMovement().SetExtraInfo("", "");
        }
    }
    //Trigger primary events
    public override void Use_1(GameObject whoInteracted)
    {
        if(use1Sound!= null)
        {
            Instantiate(use1Sound, transform.position, transform.rotation);
        }

        foreach (Event evt in use1_Events)
        {
            evt.DoEvent();
        }
        if (destroyAfterUse1)
        {
            Destroy(gameObject);
        }
    }
    //Trigger secondary events
    public override void Use_2(GameObject whoInteracted)
    {
        if (use2Sound != null)
        {
            Instantiate(use2Sound, transform.position, transform.rotation);
        }
        foreach (Event evt in use2_Events)
        {
            evt.DoEvent();
        }
        if (destroyAfterUse2)
        {
            Destroy(gameObject);
        }
    }
    #endregion
    #region Get Set
    //If you need to add new events for the primary interaction externally, call this
    public override void Add_Events_Use1(List<Event> newEvents)
    {
        foreach (Event evt in newEvents)
        {
            if (!use1_Events.Contains(evt))
            {
                use1_Events.Add(evt);
            }
        }
    }
    //If you need to add new events for the secondary interaction externally, call this
    public override void Add_Events_Use2(List<Event> newEvents)
    {
        foreach (Event evt in newEvents)
        {
            if (!use2_Events.Contains(evt))
            {
                use2_Events.Add(evt);
            }
        }
    }
    public override void ExternallyChange_Sprites(Sprite newSprt1, Sprite newSprt2)
    {
        sprt1 = newSprt1;
        sprt2 = newSprt2;
    }
    public override void ExternallyChange_Info(string newInfo1, string newInfo2)
    {
        extraInfo1 = newInfo1;
        extraInfo2 = newInfo2;
    }
    #endregion
}
