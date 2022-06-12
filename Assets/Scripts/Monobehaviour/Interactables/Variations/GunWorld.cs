using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunWorld : Interactable
{
    #region Serialized variables

    [Header("Main Settings")]


    [Tooltip("Prefab of the usable gun")]
    [SerializeField] GameObject gun;

    [Tooltip("Image that will be displayed to the player, representing that he can take the gun")]
    [SerializeField] Sprite takeSprt;


    [Space]


    [Header("Sound")]


    [Tooltip("It will perform when the use1 activates. If you don't need it, leave it empty")]
    [SerializeField] GameObject use1Sound;

    [Tooltip("It will perform when the use2 activates. If you don't need it, leave it empty")]
    [SerializeField] GameObject use2Sound;


    [Space]


    [Header("Debug variables")]
    [SerializeField] private int magazineLeft = 0;

    #endregion

    #region Private variables

    private GameObject player;
    private WeaponController controller;
    private BaseAmmo ammoType;
    private List<Material> materials = new List<Material>();

    #endregion

    #region Main Functions

    private void Awake()
    {
        //Gets all the mesh renderers in the game object
        List<MeshRenderer> meshes = new List<MeshRenderer>();
        if (gameObject.GetComponent<MeshRenderer>() != null)
        {
            if (!meshes.Contains(gameObject.GetComponent<MeshRenderer>()))
            {
                meshes.Add(gameObject.GetComponent<MeshRenderer>());
            }
        }
        if (gameObject.GetComponentInChildren<MeshRenderer>() != null)
        {
            MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            if(renderers != null)
            {
                foreach(MeshRenderer render in renderers)
                {
                    if (!meshes.Contains(render))
                    {
                        meshes.Add(render);
                    }
                }
            }
        }
        //Find the player
        player = GameObject.FindGameObjectWithTag("Player");
        //Set the magazine of the gun to the max value
        if (gun.GetComponent<BaseGun>() != null)
        {
            magazineLeft = gun.GetComponent<BaseGun>().GetMagazineSize();
            ammoType = gun.GetComponent<BaseGun>().SendAmmoType();
        }
        //Get the weapon controller of the player
        if(controller == null && player != null)
        {
            if(player.GetComponentInChildren<WeaponController>() != null)
            {
                controller = player.GetComponentInChildren<WeaponController>();
            }
            else if (player.GetComponent<WeaponController>() != null)
            {
                controller = player.GetComponent<WeaponController>();
            }

        }
        //Get all the materials of the gun and hide outline
        if(GetComponent<MeshRenderer>() != null && GetComponent<MeshRenderer>().material != null)
        {
            foreach(Material mtl in GetComponent<MeshRenderer>().materials)
            {
                if (!materials.Contains(mtl))
                {
                    materials.Add(mtl);
                }
            }

        }
        else if (GetComponentInChildren<MeshRenderer>() != null && GetComponentInChildren<MeshRenderer>().materials != null)
        {
            foreach (MeshRenderer mesh in meshes)
            {
                Material[] tempMaterials = mesh.materials;
                foreach(Material mtl in tempMaterials)
                {
                    if (mtl != null && !materials.Contains(mtl))
                    {
                        materials.Add(mtl);
                    }
                }

            }
        }
        if(materials != null)
        {
            foreach (Material mtl in materials)
            {
                mtl.SetInt("Show_Outline", 0);
            }
        }


    }
    //Gives the magazine bullets to the player inventory
    public override void Use_1(GameObject whoInteracted)
    {
        if (whoInteracted.GetComponent<WeaponController>() != null)
        {
            controller = whoInteracted.GetComponent<WeaponController>();
        }
        if(use1Sound!= null)
        {
            Instantiate(use1Sound, transform.position, transform.rotation);
        }
        if (controller != null && magazineLeft >0 && ammoType != null)
        {
            controller.OnTakeBullets(ammoType, magazineLeft);
            magazineLeft = 0;
        }
    }
    //Get the usable gun prefab
    public override void Use_2(GameObject whoInteracted)
    {
        if (whoInteracted.GetComponent<WeaponController>() != null)
        {
            controller = whoInteracted.GetComponent<WeaponController>();
        }
        if (use2Sound != null)
        {
            Instantiate(use2Sound, transform.position, transform.rotation);
        }
        if (controller != null)
        {
            if (whoInteracted.GetComponent<PlayerMovement>() != null)
            {
                whoInteracted.GetComponent<PlayerMovement>().GetMovement().SetSpritesInfo(null, null);
                whoInteracted.GetComponent<PlayerMovement>().GetMovement().SetExtraInfo("", "");
            }
            controller.OnTakeNewGun(magazineLeft, gun);
            Destroy(gameObject);
        }
    }
    //Shows the outline
    public override void Found(GameObject whoFind)
    {
        if(materials != null)
        {
            foreach (Material mtl in materials)
            {
                mtl.SetInt("Show_Outline", 1);
            }
        }
        if (whoFind.GetComponent<PlayerMovement>() != null)
        {
            whoFind.GetComponent<PlayerMovement>().GetMovement().SetSpritesInfo(ammoType.GetAmmoIcon(), takeSprt);
            whoFind.GetComponent<PlayerMovement>().GetMovement().SetExtraInfo("("+magazineLeft+")", "Take");
        }
    }
    //Hides the outline
    public override void Miss(GameObject whoMiss)
    {
        if(materials != null)
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

    #endregion

    #region Get Set

    //Change the magazine bullets ammount
    public void ChangeMagazine(int magazineBullets)
    {
        magazineLeft = magazineBullets;
    }
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
