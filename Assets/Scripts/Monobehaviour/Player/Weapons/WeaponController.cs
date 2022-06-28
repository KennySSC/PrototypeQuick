using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;
using TMPro;
public class WeaponController : MonoBehaviour
{
    #region Serialized Variables

    [Header("Main Settings")]


    [Tooltip("All the scriptable objects of Base Ammo that exist  and you want to have an inventory for")]
    [SerializeField] BaseAmmo[] ammoTypes;

    [Tooltip("The current ammo for each ammo type you set. If exceeds the limit, they will set to the max value when take a gun with that ammo type")]
    [SerializeField] private int[] ammoInventory;

    [Tooltip("The position where the usable gun will spawn, should be child of the right hand bone")]
    [SerializeField] Transform gunPosition;

    [Tooltip("The position where the gun world of your current usable gun will appear once you want to change it for one on the ground")]
    [SerializeField] Transform leaveGunPosition;



    [Space]


    [Header("Rig settings")]


    [Tooltip("Rig builder script attached to the player. Without it, nothing else of the rigging options will work. If you don't need it, leave it empty")]
    [SerializeField] RigBuilder build;

    [Tooltip("Left arm ik constrain")]
    [SerializeField] TwoBoneIKConstraint L_arm_IK;

    [Tooltip("The target that will reposition on the weapons left arm position")]
    [SerializeField] Transform Dynamic_L_Arm_Target;

    [Tooltip("The default target of the left arm ik constrain")]
    [SerializeField] Transform Static_L_Arm_Target;

    [Tooltip("The child of the default target of the left arm ik constrain")]
    [SerializeField] Transform Static_L_Arm_Target_Child;

    [Space]


    [Header("Enemy Settings")]

    [SerializeField] bool isEnemy;


    [Space]


    [Header("UI Settings (if needed)")]

    [SerializeField] Image img_AmmoType;

    [SerializeField] TMP_Text txt_Remaining_Ammo;

    [SerializeField] Sprite noneGun_Sprite;


    [Space]


    [Header("Crosshai settings")]


    [Tooltip("Which layers are considered to detect damagables")]
    [SerializeField] LayerMask damagableMask;

    [Tooltip("Crosshair UI Image")]
    [SerializeField] Image crosshair;

    [Tooltip("Sprite that will be shown while player has a gun")]
    [SerializeField] Sprite crosshair_Sprite_Waist;

    [Tooltip("Sprite that will be shown while player has a gun and is aiming")]
    [SerializeField] Sprite crosshair_Sprite_Aim;

    [Space]


    [Header("Debug variables")]

    [SerializeField] private int[] ammoMaxInventory;

    [SerializeField] BaseGun currentGun;

    [SerializeField] private PlayerMovement player;

    [SerializeField] private EnemyController enemy;


    #endregion

    #region Private variables
    [SerializeField]private Transform shotPosition;
    private Transform objectSpawnPos;
    private bool canShot = true;
    private bool spawnShotObj = false;
    #endregion

    #region Main Functions
    private void Awake()
    {
        //Sets the max ammo for each ammo type
        ammoMaxInventory = new int[ammoTypes.Length];
        ammoInventory = new int[ammoTypes.Length];
        for (int i = 0; i < ammoTypes.Length; i++)
        {
            ammoMaxInventory[i] = ammoTypes[i].GetInventory();
            if (ammoInventory[i] > ammoMaxInventory[i])
            {
                if (isEnemy)
                {
                    ammoInventory[i] = 99999;
                }
                else
                {
                    ammoInventory[i] = ammoMaxInventory[i];
                }
            }
            if (isEnemy)
            {
                if (isEnemy)
                {
                    ammoInventory[i] = 99999;
                }
            }
        }
        //Find the controller
        if (isEnemy)
        {
            if (GetComponent<EnemyController>() != null)
            {
                enemy = GetComponent<EnemyController>();
            }
            else if (GetComponentInChildren<EnemyController>() != null)
            {
                enemy = GetComponentInChildren<EnemyController>();
            }
        }
        else
        {
            player = FindObjectOfType<PlayerMovement>();
        }
        //Set crosshair
        if (crosshair_Sprite_Aim != null && crosshair != null)
        {
            crosshair.sprite = crosshair_Sprite_Aim;
        }
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        if(currentGun != null && !isEnemy)
        {
            RaycastHit hit;
            if(Physics.Raycast(currentGun.GetShotPosition().position, currentGun.GetShotPosition().forward, out hit, Mathf.Infinity, damagableMask))
            {
                if(hit.collider.gameObject.tag == "Damagable")
                {
                    if (crosshair != null)
                    {
                        crosshair.color = Color.red;
                    }
                }
                else
                {
                    if (crosshair != null)
                    {
                        crosshair.color = Color.green;
                    }
                }

            }
            else
            {
                if (crosshair != null)
                {
                    crosshair.color = Color.green;
                }
            }
        }
    }
    public void TryToShoot()
    {
        if (canShot)
        {
            if (currentGun != null)
            {
                Debug.Log("Weapon controller tryed to shot");
                currentGun.TryToShoot();
            }
        }

    }
    public void OnTryCancelShoot()
    {
        if(currentGun != null)
        {
            currentGun.OnFingerUp();
        }
    }
    public void OnTryAim(GameObject cam)
    {
        if(currentGun!= null)
        {
            currentGun.TryToAim(cam);
            //Set crosshair
            if (crosshair_Sprite_Aim != null && crosshair != null)
            {
                crosshair.sprite = crosshair_Sprite_Aim;
            }
        }
    }
    public void OnTryCancelAim(GameObject cam)
    {
        if (currentGun != null)
        {
            currentGun.TryCancelAim(cam);
            //Set crosshair
            if (crosshair_Sprite_Waist != null && crosshair != null)
            {
                crosshair.sprite = crosshair_Sprite_Waist;
            }
        }
    }
    //Function to set the current gun ammo inventory
    public void OnChangeAmmoValue(BaseAmmo ammoType, int ammoAmount)
    {
        if(currentGun != null)
        {
            for (int i = 0; i < ammoTypes.Length; i++)
            {
                if (ammoType == ammoTypes[i])
                {
                    ammoInventory[i] = currentGun.SendInventoryAmmo();
                    return;
                }
            }
        }
    }
    public void OnTryReload()
    {
        if(currentGun != null)
        {
            currentGun.ReloadWeapon();
        }
    }
    public void OnReloadRig()
    {
        if (isEnemy && build != null)
        {
            enemy.GetEnemyAnimator().enabled = false;
            Dynamic_L_Arm_Target.eulerAngles = new Vector3(0, 0, 0);
            if (Dynamic_L_Arm_Target.GetComponent<TeleporObject>() != null)
            {
                Dynamic_L_Arm_Target.GetComponent<TeleporObject>().ChangeTarget(null);
            }
            L_arm_IK.data.target = Static_L_Arm_Target;
            enemy.GetEnemyAnimator().enabled = true;
            build.enabled = false;
            build.enabled = true;
        }
        else if (!isEnemy && build != null)
        {
            player.GetPlayerAnimator().enabled = false;
            Dynamic_L_Arm_Target.eulerAngles = new Vector3(0, 0, 0);
            if (Dynamic_L_Arm_Target.GetComponent<TeleporObject>() != null)
            {
                Dynamic_L_Arm_Target.GetComponent<TeleporObject>().ChangeTarget(null);
            }
            L_arm_IK.data.target = Static_L_Arm_Target;
            player.GetPlayerAnimator().enabled = true;
            build.enabled = false;
            build.enabled = true;
        }

    }
    public void FinishingReload()
    {
        if (isEnemy && build !=null)
        {
            enemy.GetEnemyAnimator().enabled = false;
            L_arm_IK.data.target = Dynamic_L_Arm_Target;
            Dynamic_L_Arm_Target.position = Static_L_Arm_Target_Child.position;
            if (Dynamic_L_Arm_Target.GetComponent<TeleporObject>() != null)
            {
                Dynamic_L_Arm_Target.GetComponent<TeleporObject>().ChangeTarget(Static_L_Arm_Target_Child);
            }
            enemy.GetEnemyAnimator().enabled = true;
            build.enabled = false;
            build.enabled = true;
        }
        else if(!isEnemy && build != null)
        {
            player.GetPlayerAnimator().enabled = false;
            L_arm_IK.data.target = Dynamic_L_Arm_Target;
            Dynamic_L_Arm_Target.position = Static_L_Arm_Target_Child.position;
            if (Dynamic_L_Arm_Target.GetComponent<TeleporObject>() != null)
            {
                Dynamic_L_Arm_Target.GetComponent<TeleporObject>().ChangeTarget(Static_L_Arm_Target_Child);
            }
            player.GetPlayerAnimator().enabled = true;
            build.enabled = false;
            build.enabled = true;
        }

    }
    public void OnTakeNewGun(int magazine, GameObject newGun)
    {
        GameObject oldGun;
        GameObject tempGun;
        //Applies animation
        if (isEnemy)
        {
            enemy.GetEnemyAnimator().Play("Gun_TakeOut", 1);
        }
        else
        {
            player.GetPlayerAnimator().Play("Gun_TakeOut", 1);
        }

        //If you already have a gun in your hand, it will spawn a world gun of the same type that has the same bullets on the magazine which you left it
        //Then will destroy your current usable gun
        if (currentGun != null)
        {
            BaseAmmo ammoType = currentGun.SendAmmoType();
            for (int i = 0; i < ammoTypes.Length; i++)
            {
                if (ammoType == ammoTypes[i])
                {
                    ammoInventory[i] = currentGun.SendInventoryAmmo();
                    oldGun = currentGun.SendWorldObject();
                    if (oldGun.GetComponent<GunWorld>() != null)
                    {
                        GameObject newOldGun = Instantiate(oldGun, leaveGunPosition.position, oldGun.transform.rotation);
                        newOldGun.GetComponent<GunWorld>().ChangeMagazine(currentGun.SendCurrentBullets());
                    }
                    Destroy(currentGun.gameObject);
                }
            }
        }
        //Instantiates the new gun in your hand
        currentGun = null;
        if (crosshair_Sprite_Waist != null && crosshair != null)
        {
            crosshair.sprite = crosshair_Sprite_Waist;
        }
        tempGun = Instantiate(newGun, gunPosition.position, gunPosition.rotation);
        tempGun.transform.parent = gunPosition.transform;
        currentGun = tempGun.GetComponent<BaseGun>();
        currentGun.GetWeaponController(this);
        if (currentGun != null)
        {
            //Set the inventory for this gun and ammo type 
            BaseAmmo tempAmmoType = currentGun.SendAmmoType();
            for (int i = 0; i < ammoTypes.Length; i++)
            {
                if (tempAmmoType == ammoTypes[i])
                {
                    currentGun.GetInventoryAmmo(ammoInventory[i]);
                    currentGun.GetCurrentBullets(magazine);
                    currentGun.OnTakeNewGun(shotPosition, objectSpawnPos, spawnShotObj);
                }
            }
            //Set UI
            if(img_AmmoType != null && txt_Remaining_Ammo != null)
            {
                currentGun.SetUIElements(img_AmmoType, txt_Remaining_Ammo);
            }
            //Set the left arm position
            if (player != null)
            {
                player.ChangeHasGun(true);
                currentGun.GetPlayerAnimator(player.GetPlayerAnimator());
                if(build != null)
                {
                player.GetPlayerAnimator().enabled = false;
                L_arm_IK.data.target = Dynamic_L_Arm_Target;
                Static_L_Arm_Target_Child.localPosition = new Vector3(0.05f + currentGun.SendLeftArmPos().localPosition.x, Static_L_Arm_Target_Child.localPosition.y, 0 + currentGun.SendLeftArmPos().localPosition.y);
                Dynamic_L_Arm_Target.position = Static_L_Arm_Target_Child.position;
                if (Dynamic_L_Arm_Target.GetComponent<TeleporObject>() != null)
                {
                    Dynamic_L_Arm_Target.GetComponent<TeleporObject>().ChangeTarget(Static_L_Arm_Target_Child);
                }
                    if (player.GetIsDither())
                    {
                        List<Material> tempMtls = currentGun.GetGunMaterials();
                        foreach(Material mtl in tempMtls)
                        {
                            mtl.SetInt("_Calculate_Dither", 1);
                        }
                    }
                    else
                    {
                        List<Material> tempMtls = currentGun.GetGunMaterials();
                        foreach (Material mtl in tempMtls)
                        {
                            mtl.SetInt("_Calculate_Dither", 0);
                        }
                    }
                player.GetPlayerAnimator().enabled = true;
                build.enabled = false;
                build.enabled = true;
                }

            }
            else if(isEnemy && enemy != null)
            {
                enemy.ChangeHasGun(true);
                currentGun.GetPlayerAnimator(enemy.GetEnemyAnimator());
                if(build != null)
                {
                    enemy.GetEnemyAnimator().enabled = false;
                    L_arm_IK.data.target = Dynamic_L_Arm_Target;
                    Static_L_Arm_Target_Child.localPosition = new Vector3(0.05f + currentGun.SendLeftArmPos().localPosition.x, Static_L_Arm_Target_Child.localPosition.y, 0 + currentGun.SendLeftArmPos().localPosition.y);
                    Dynamic_L_Arm_Target.position = Static_L_Arm_Target_Child.position;
                    if (Dynamic_L_Arm_Target.GetComponent<TeleporObject>() != null)
                    {
                        Dynamic_L_Arm_Target.GetComponent<TeleporObject>().ChangeTarget(Static_L_Arm_Target_Child);
                    }
                    enemy.GetEnemyAnimator().enabled = true;
                    build.enabled = false;
                    build.enabled = true;
                }

            }
        }
        else
        {
            img_AmmoType.sprite = noneGun_Sprite;
            //Reset the left arm position
            if (player != null && currentGun == null)
            {
                player.ChangeHasGun(false);
                if(build != null)
                {
                    player.GetPlayerAnimator().enabled = false;
                    Dynamic_L_Arm_Target.eulerAngles = new Vector3(0, 0, 0);
                    if (Dynamic_L_Arm_Target.GetComponent<TeleporObject>() != null)
                    {
                        Dynamic_L_Arm_Target.GetComponent<TeleporObject>().ChangeTarget(null);
                    }
                    L_arm_IK.data.target = Static_L_Arm_Target;
                    player.GetPlayerAnimator().enabled = true;
                    build.enabled = false;
                    build.enabled = true;
                }

            }else if (enemy!= null && currentGun == null)
            {
                enemy.ChangeHasGun(false);
                if(build != null)
                {
                    enemy.GetEnemyAnimator().enabled = false;
                    Dynamic_L_Arm_Target.eulerAngles = new Vector3(0, 0, 0);
                    if (Dynamic_L_Arm_Target.GetComponent<TeleporObject>() != null)
                    {
                        Dynamic_L_Arm_Target.GetComponent<TeleporObject>().ChangeTarget(null);
                    }
                    L_arm_IK.data.target = Static_L_Arm_Target;
                    enemy.GetEnemyAnimator().enabled = true;
                    build.enabled = false;
                    build.enabled = true;
                }
            }
        }
        //Set the gun renderers
        if (isEnemy)
        {

        }
        else
        {
            currentGun.OnSetAimRenderers(player.GetPlayerRenderers());
        }

    }
    //If you want to add bullets via other game object, call this function
    public void OnTakeBullets(BaseAmmo ammoType, int ammoAdd)
    {
        for (int i = 0; i < ammoTypes.Length; i++)
        {
            if (ammoType == ammoTypes[i])
            {
                ammoInventory[i] += ammoAdd;
                if(ammoInventory[i]> ammoMaxInventory[i])
                {
                    ammoInventory[i] = ammoMaxInventory[i];
                }
                if(currentGun != null && currentGun.SendAmmoType() == ammoType)
                {
                    currentGun.GetInventoryAmmo(ammoInventory[i]);
                }
                return;
            }
        }
    }
    #endregion

    #region Get Set
    public void ChangeShotPos(Transform position)
    {
        shotPosition = position;
        if(currentGun!= null)
        {
            currentGun.ChangeShotSpawn(position);
        }
    }

    public void ChangeCanSpawnShotObj(bool newValue)
    {
        spawnShotObj = newValue;
    }
    public List<Renderer> GetRenderers()
    {
        if (isEnemy)
        {
            return enemy.GetEnemyRenderers();
        }
        else
        {
            return player.GetPlayerRenderers();
        }

    }
    public int Get_ControllerType_Index()
    {
        if(player != null)
        {
            return player.Get_ControllerType_Index();
        }
        else
        {
            return -1;
        }
    }
    public PlayerMovement GetPlayerMovement()
    {
        if(player != null)
        {
            return player;
        }
        else
        {
            return null;
        }
    }
    public List<Material> GetCurrentGunMaterials()
    {
        return currentGun.GetGunMaterials();
    }
    //Allows to externally disable shooting without disabling the script, preventing errors
    public void SetCanShot(bool value)
    {
        canShot = value;
    }
    #endregion

    #region Coroutines
    IEnumerator WaitToChangePos()
    {
        yield return new WaitForSeconds(0.1f);
    }
    #endregion
}
