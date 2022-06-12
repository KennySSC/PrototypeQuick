using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Add_Ammo : Interactable
{
    #region Serializable variables

    [Header("Main Settings")]


    [Tooltip("Ammo Type scriptable object")]
    [SerializeField] BaseAmmo ammoType;

    [Tooltip("Ammo that can be obtained when something interacts with an object with this script")]
    [SerializeField] int ammoAmmount;


    [Space]


    [Header("Sound")]


    [Tooltip("It will perform when the use1 activates. If you don't need it, leave it empty")]
    [SerializeField] GameObject use1Sound;

    [Tooltip("It will perform when the use2 activates. If you don't need it, leave it empty")]
    [SerializeField] GameObject use2Sound;

    #endregion

    #region private variables

    private MeshRenderer[] renderers;
    private List<Material> materials = new List<Material>();
    private WeaponController controller;

    #endregion

    #region Main functions

    private void Start()
    {
        controller = FindObjectOfType<WeaponController>();
        //Get all the mesh renderers to set the outline
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
        //Hide outline
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
            whoFind.GetComponent<PlayerMovement>().GetMovement().SetSpritesInfo(ammoType.GetAmmoIcon(), ammoType.GetAmmoIcon());
            whoFind.GetComponent<PlayerMovement>().GetMovement().SetExtraInfo("("+ammoAmmount+")", "(" + ammoAmmount + ")");
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
    //The one who interacts obtains the ammo and detroy the object
    public override void Use_1(GameObject whoInteracted)
    {
        if (whoInteracted.GetComponent<WeaponController>() != null)
        {
            controller = whoInteracted.GetComponent<WeaponController>();
        }
        if (whoInteracted.GetComponent<PlayerMovement>() != null)
        {
            whoInteracted.GetComponent<PlayerMovement>().GetMovement().SetSpritesInfo(null, null);
            whoInteracted.GetComponent<PlayerMovement>().GetMovement().SetExtraInfo("", "");
        }
        if (use1Sound != null)
        {
            Instantiate(use1Sound, transform.position, transform.rotation);
        }
        if (controller != null && ammoType != null)
        {
            controller.OnTakeBullets(ammoType, ammoAmmount);
            Destroy(gameObject);
        }
    }
    //The one who interacts obtains the ammo and detroy the object
    public override void Use_2(GameObject whoInteracted)
    {
        if (whoInteracted.GetComponent<WeaponController>() != null)
        {
            controller = whoInteracted.GetComponent<WeaponController>();
        }
        if (whoInteracted.GetComponent<PlayerMovement>() != null)
        {
            whoInteracted.GetComponent<PlayerMovement>().GetMovement().SetSpritesInfo(null, null);
            whoInteracted.GetComponent<PlayerMovement>().GetMovement().SetExtraInfo("", "");
        }
        if (use2Sound != null)
        {
            Instantiate(use2Sound, transform.position, transform.rotation);
        }
        if (controller != null && ammoType != null)
        {
            controller.OnTakeBullets(ammoType, ammoAmmount);
            Destroy(gameObject);
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
