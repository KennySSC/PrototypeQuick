using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Add_Health : Interactable
{

    #region Serializable Variables

    [Header("Main Settings")]


    [Tooltip("Ammount of healing points gained inmediatly when something interacts with the GameObject that contains this script")]
    [SerializeField] int hpAmmount;

    [Tooltip("Image displayed to the player that indicates that this object is health")]
    [SerializeField] Sprite hpSprt;


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
    private Health[] healths;

    #endregion

    #region Main Functions
    private void Start()
    {
        //Get all the mesh renderers and materials to set the outline
        if (GetComponent<MeshRenderer>() != null)
        {
            renderers = GetComponents<MeshRenderer>();
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
        if (materials != null)
        {
            foreach (Material mtl in materials)
            {
                mtl.SetInt("Show_Outline", 0);
            }
        }
    }
    //Show outline
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
            whoFind.GetComponent<PlayerMovement>().GetMovement().SetSpritesInfo(hpSprt, hpSprt);
            whoFind.GetComponent<PlayerMovement>().GetMovement().SetExtraInfo("(" + hpAmmount + ")", "(" + hpAmmount + ")");
        }
    }
    //Hide outline
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
    //The one who interacts obtains the health and detroy the object
    public override void Use_1(GameObject whoInteracted)
    {
        if (whoInteracted.GetComponent<Health>() != null)
        {
            healths = whoInteracted.GetComponents<Health>();
        }
        if(use1Sound!= null)
        {
            Instantiate(use1Sound, transform.position, transform.rotation);
        }

        if (healths != null)
        {
            foreach(Health hp in healths)
            {
                if(hp.GetCurrentHealth()< hp.GetMaxHealth())
                {
                    hp.GetHealing(hpAmmount);
                    if (whoInteracted.GetComponent<PlayerMovement>() != null)
                    {
                        whoInteracted.GetComponent<PlayerMovement>().GetMovement().SetSpritesInfo(null, null);
                        whoInteracted.GetComponent<PlayerMovement>().GetMovement().SetExtraInfo("", "");
                    }
                    Destroy(gameObject);
                    break;
                }
            }
        }
    }
    //The one who interacts obtains the health and detroy the object
    public override void Use_2(GameObject whoInteracted)
    {
        if (whoInteracted.GetComponent<WeaponController>() != null)
        {
            healths = whoInteracted.GetComponents<Health>();
        }
        if (use2Sound != null)
        {
            Instantiate(use2Sound, transform.position, transform.rotation);
        }
        if (healths != null)
        {
            foreach (Health hp in healths)
            {
                if (hp.GetCurrentHealth() < hp.GetMaxHealth())
                {
                    hp.GetHealing(hpAmmount);
                    if (whoInteracted.GetComponent<PlayerMovement>() != null)
                    {
                        whoInteracted.GetComponent<PlayerMovement>().GetMovement().SetSpritesInfo(null, null);
                        whoInteracted.GetComponent<PlayerMovement>().GetMovement().SetExtraInfo("", "");
                    }
                    Destroy(gameObject);
                    break;
                }
            }
        }
    }

    #endregion

    #region Get Set
    public override void Add_Events_Use1(List<Event> newEvents)
    {

    }
    public override void Add_Events_Use2(List<Event> newEvents)
    {

    }
    public override void ExternallyChange_Sprites(Sprite newSprt1, Sprite newSprt2)
    {

    }
    public override void ExternallyChange_Info(string newInfo1, string newInfo2)
    {

    }

    #endregion
}
