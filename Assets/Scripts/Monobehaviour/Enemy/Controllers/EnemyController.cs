using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Serialized variables
    [Header("Required Components")]


    [Tooltip("Enemy_Base script variant that controls the enemy")]
    [SerializeField] Enemy_Base enemyMovement;

    [Header("Optional components")]
    [Tooltip("If the controller needs to have weapons, put a controller, otherwise leave it empty")]
    [SerializeField] WeaponController weaponController;

    [Tooltip("The weapon that you want the enemy to have")]
    [SerializeField] GameObject weapon;

    [Tooltip("Where the bullet will appear")]
    [SerializeField] Transform shotPosition;

    [Tooltip("If you turn it up, an object with the same direction of the shot will appear, helping the player to know where the shots are coming from")]
    [SerializeField] bool canSpawnShotObject;

    [Tooltip("Turn on if you want to give a random weapon to the enemy, overriding the weapon")]
    [SerializeField] bool randomWeapon;

    [Tooltip("List of possible random weapon")]
    [SerializeField] GameObject[] posibleWeapons;


    [Space]


    [Header("Debug variables")]


    [SerializeField] private bool hasGun = false;

    #endregion

    #region Private variables

    private List<Renderer> renderers = new List<Renderer>();
    private List<Material> materials = new List<Material>();

    #endregion
    #region Main Functions

    private void Awake()
    {
        //Get all renderer components
        if (GetComponent<Renderer>() != null)
        {
            Renderer[] tempRenderers = GetComponents<Renderer>();
            foreach (Renderer render in tempRenderers)
            {
                if (!renderers.Contains(render))
                {
                    renderers.Add(render);
                }
            }
        }
        if (GetComponentInChildren<Renderer>() != null)
        {
            Renderer[] tempChildRenderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer render in tempChildRenderers)
            {
                if (!renderers.Contains(render))
                {
                    renderers.Add(render);
                }
            }
        }
        if (renderers != null && renderers.Count > 0)
        {
            foreach (Renderer rnd in renderers)
            {
                Material[] tempMaterials = rnd.materials;
                if (tempMaterials != null && tempMaterials.Length > 0)
                {
                    foreach (Material mtl in tempMaterials)
                    {
                        if (!materials.Contains(mtl))
                        {
                            materials.Add(mtl);
                        }
                    }
                }
            }
        }
       
    }
    private void Start()
    {
        //If the controller has the weapons module, it will set the shot position
        if (weaponController != null)
        {
            weaponController.ChangeShotPos(shotPosition);
            weaponController.ChangeCanSpawnShotObj(shotPosition);

            if (randomWeapon && posibleWeapons.Length > 0)
            {
                int random = Random.Range(0, posibleWeapons.Length);
                int fullBullets = posibleWeapons[random].GetComponent<BaseGun>().GetMagazineSize();
                weaponController.OnTakeNewGun(fullBullets, posibleWeapons[random]);
            }
            else
            {
                int fullBullets = weapon.GetComponent<BaseGun>().GetMagazineSize();
                weaponController.OnTakeNewGun(fullBullets, weapon);
            }
            hasGun = true;

        }


    }
    //Applies logic
    private void Update()
    {
        OnMovement();
        OnLook();
        OnAnimationLogic();

    }
    private void OnMovement()
    {
        enemyMovement.Movement();
    }
    private void OnLook()
    {

        enemyMovement.Look();
    }

    public void OnShoot()
    {
        if (weaponController != null)
        {
            Debug.Log("Enemy controller tried to shot");
            weaponController.TryToShoot();
            weaponController.OnTryCancelShoot();
            weaponController.TryToShoot();

        }
    }

    public void OnReload()
    {
        if (weaponController != null)
        {
            weaponController.OnTryReload();
        }
    }
    private void OnAnimationLogic()
    {
        enemyMovement.AnimationLogic();
    }
    #endregion
    #region Get Set
    public void OnChangeShootPos(Transform position)
    {
        shotPosition = position;
        if (weaponController != null)
        {
            weaponController.ChangeShotPos(shotPosition);
        }
    }
    public void ChangeHasGun(bool change)
    {
        hasGun = change;
        enemyMovement.ChangeHasGun(change);
    }
    public bool SendHasGun()
    {
        return hasGun;
    }
    public Animator GetEnemyAnimator()
    {
        return enemyMovement.GetAnimator();
    }
    public List<Renderer> GetEnemyRenderers()
    {
        return renderers;
    }
    public Transform GetShotPosition()
    {
        return shotPosition;
    }

    #endregion
}
