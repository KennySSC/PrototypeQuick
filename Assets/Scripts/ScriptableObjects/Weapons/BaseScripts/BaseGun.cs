using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.UI;

public enum RateType{Semiauto, Fullauto}
public class BaseGun : MonoBehaviour
{
    #region Serialized variables


    [Header("Animation components")]


    [Tooltip("Empty object where the rig will try to position the player's left arm. Don't put it to far away from the trigger or will result in weird animations")]
    [SerializeField] Transform l_Arm_Pos;

    [Tooltip("Gun's animator. If you aren't using animation, you can leave it empty")]
    [SerializeField] Animator gunAnimator;

    [Tooltip("Animator override of the specific gun you are setting, must contain at least all the reload animations and shot animations")]
    [SerializeField] AnimatorOverrideController playerGunOverride;


    [Space]


    [Header("Shot and reload settings")]


    [Tooltip("A scriptable object, it can be any child of BaseAim script")]
    [SerializeField] BaseAim aimType;

    [Tooltip("If you use a scope aim AimType, you must put a canvas with scope texture here, else leave empty")]
    [SerializeField] GameObject sightCanvas;

    [Tooltip("Way that the gun will shot")]
    [SerializeField] RateType _rateType;

    [Tooltip("Scriptable object, it can be any child of the script BaseAmmo")]
    [SerializeField] BaseAmmo ammoType;

    [Tooltip("Scriptable object, it can be any child of the script BaseShoot")]
    [SerializeField] BaseShoot shotType;

    [Tooltip("Damage per impact, becareful with shotguns")]
    [SerializeField] int damage;

    [Tooltip("The shots that the gun can perform in a second")]
    [SerializeField] float rateOfFire;

    [SerializeField] int magazineSize;

    [Tooltip("The ammount of bullets added for each reload cycle (for example, in a shotgun you put a bullet at once, and in a pistol all the bullets that can hold)")]
    [SerializeField] int bulletsAdd;

    [SerializeField] float reloadTime;

    [Tooltip("Allows to keep one more bullet in the magazine if the reload is performed before running out of bullets in the current magazine")]
    [SerializeField] bool chamberBullet;

    [Tooltip("Allows to shot while reloading, but only if the magazine has a bullet")]
    [SerializeField] bool shootWhileReload;


    [Space]


    [Header("Other")]


    [Tooltip("The world gun prefab. It should contain a GunWorld script")]
    [SerializeField] GameObject worldObject;


    [Space]


    [Header("Particles & FX")]


    [Tooltip("The position where the prefab of the bullet case will spawn after shooting")]
    [SerializeField] Transform bulletCaseSpawn;

    [Tooltip("Prefab of a bullet Case, must have a rigid body and LifeTime script attached. If you don't want to spawn a prefab leave it empty")]
    [SerializeField] GameObject bulletCasePrefab;

    [Tooltip("The position where the muzzle flash of the gun will spawn after shooting")]
    [SerializeField] Transform muzzleSpawn;

    [Tooltip(" If you don't want to spawn a muzzle flash, leave it empty")]
    [SerializeField] GameObject muzzlePrefab;

    [Tooltip("A light that shows a fraction of a second after shooting, it should be realtime and child of the gun with this script." +
        " High permormance cost for mobile. If you don't want a lightleave it empty")]
    [SerializeField] GameObject muzzleLight;

    [Tooltip("If the model has a bullet on the magazine that is a independent Game Object, if the player reloads the gun with bullets left, it will show, otherwhise it will be hidden." +
        "If you don't want to use this, you can leave it empty")]
    [SerializeField] GameObject magBullet;


    [Space]


    [Header("Sound")]


    [Tooltip("It will perform when the gun can shot a bullet. If you don't want to spawn a sound leave it empty")]
    [SerializeField] GameObject shotSound;

    [Tooltip("Extra shot sound with a wait time, useful for shotgun pull pump, sniper bolt and more. If you don't want to spawn a sound leave it empty")]
    [SerializeField] GameObject shotExtraSound;
    [SerializeField] float shotExtraSound_WaitTime;

    [Tooltip("It will perform when the gun hasn't bullets on it's magazine. If you don't want to spawn a sound leave it empty")]
    [SerializeField] GameObject noBulletShotSound;

    [Tooltip("It will perform after a wait time when the gun is starting a reload. If you don't want to spawn a sound leave it empty")]
    [SerializeField] GameObject startReloadSound;
    [SerializeField] float startReloadSound_WaitTime;

    [Tooltip("It will perform after a wait time when the gun is starting the idle reloads. If you don't want to spawn a sound leave it empty")]
    [SerializeField] GameObject idleReloadSound;
    [SerializeField] float idleReloadSound_WaitTime;

    [Tooltip("It will perform after a wait time when the gun is finishing a reload. If you don't want to spawn a sound leave it empty")]
    [SerializeField] GameObject finishReloadSound;
    [SerializeField] float finishReloadSound_WaitTime;



    #endregion

    #region Private variables
    private float currentRecoil = 0f;
    private float currentMaxRecoil = 0f;
    private float recoilAument;
    private float recoilAument_Aiming;
    private float maxRecoil;
    private float timeToBaseRecoil;
    private float timeBetweenShots;
    private float resetReloadTime;

    private int cnt_IdleReloadSound = 0;
    private int currentBullets = 0;
    private int inventoryAmmo = 0;

    private bool calculateRecoil;
    private bool canShoot = true;
    private bool isReloading = false;
    private bool isReloading_Full = false;
    private bool canPutChamberBullet = false;
    private bool fingerUp = true;
    private bool isAiming = false;
    private bool spawnObject = false;

    private List<Renderer> renderers = new List<Renderer>();
    private List<Material> materials = new List<Material>();

    [SerializeField]private Transform shotSpawn;
    private Transform objectSpawn;
    private Animator playerAnimator;
    private WeaponController weaponController;
    private float reloadIntervalsTime;
    private TMP_Text ammo;

    #endregion

    #region Main Functions
    private void Awake()
    {
        //Gets all the renderer components in the object, used for dithering or scope aim to disable the gun, otherwhise it may block the view
        if(GetComponent<Renderer>()!= null)
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
        if(GetComponentInChildren<Renderer>()!= null)
        {
            Renderer[] tempChildRenderers = GetComponentsInChildren<Renderer>();
            foreach(Renderer render in tempChildRenderers)
            {
                if (!renderers.Contains(render))
                {
                    renderers.Add(render);
                }
            }
        }
        foreach(Renderer rnd in renderers)
        {
            Material[] mtl = rnd.materials;
            
            foreach(Material tempMtl in mtl)
            {
                if (!materials.Contains(tempMtl))
                {
                    materials.Add(tempMtl);
                }
            }
        }
        //Calculates the time for each reload cycle
        reloadIntervalsTime = reloadTime / ((magazineSize/bulletsAdd)+2);

        //Get father collider
        if(weaponController != null)
        {
            shotType.GetFatherCollider(weaponController.GetPlayerMovement().GetPlayerCollider());
        }
    }
    private void Start()
    {
        if(gunAnimator != null)
        {
            gunAnimator.Play("Entry");
        }

        //Obtains the recoil values for this weapon, given by the shot scriptable object
        if (shotType != null)
        {
            recoilAument = shotType.GetRecoilAument();
            recoilAument_Aiming = shotType.GetRecoilAument_WhileAiming();
            maxRecoil = shotType.GetMaxRecoil();
            timeToBaseRecoil = shotType.GetTimeToBaseRecoil();
        }
        //Calculates the wait time between shots
        timeBetweenShots = 1 / rateOfFire;
        resetReloadTime = reloadIntervalsTime;
        //Reset the animator values
        if (gunAnimator != null)
        {
            gunAnimator.SetBool("FinishReload", false);
        }
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("FinishReload", false);
        }
        //If the gun has a scope aim type and the gun has a canvas with the texture, the values are set
        if(sightCanvas != null && aimType != null)
        {
            aimType.SightObject(sightCanvas);
            aimType.StartReset(weaponController.GetRenderers());
        }
        //If the gun has a light set, and the game object is active, it will desactivate
        if(muzzleLight != null && muzzleLight.activeSelf)
        {
            muzzleLight.SetActive(false);
        }
    }
    private void Update()
    {
        Reload_ShootReloading_Logic();
        RecoilLogic();
        ConstantAimLogic();
    }
    public void TryToShoot()
    {
        //Try to perform a shot
        if (!isReloading_Full)
        {
            if (canShoot && fingerUp && currentBullets > 0)
            {
                Debug.Log("Weapon Tryed to shot");
                if (shootWhileReload && isReloading)
                {
                    if (weaponController != null)
                    {
                        weaponController.FinishingReload();
                    }
                }
                if (weaponController != null)
                {
                    if (weaponController.Get_ControllerType_Index() == 1)
                    {
                        weaponController.GetPlayerMovement().GetMovement().RotateCharacter();
                    }

                }
                //Starts the recoil logic
                calculateRecoil = true;
                //Applies animation
                if (gunAnimator != null)
                {
                    gunAnimator.SetFloat("RateOfFire_Multiplier", (1 / timeBetweenShots));
                    gunAnimator.Play("Shot");
                }
                if (playerAnimator != null)
                {
                    playerAnimator.SetFloat("RateOfFire_Multiplier", (1 / timeBetweenShots));
                    if (playerAnimator.GetInteger("Gun_AnimVal") == 6)
                    {
                        playerAnimator.Play("Shot_StandAim", 1);
                    }
                    else if (playerAnimator.GetInteger("Gun_AnimVal") == 7)
                    {
                        playerAnimator.Play("Shot_CrouchAim", 1);
                    }
                    else if (playerAnimator.GetInteger("Gun_AnimVal") == 4 || playerAnimator.GetInteger("Gun_AnimVal") == 5)
                    {
                        playerAnimator.Play("Shot_Crouch", 1);
                    }
                    else
                    {
                        playerAnimator.Play("Shot", 1);
                    }
                }
                //If the gun can shot while reloading, it cancels the reload and reset values
                isReloading = false;
                reloadIntervalsTime = resetReloadTime;
                shotType.DoShoot(shotSpawn, muzzleSpawn ,currentRecoil, damage, spawnObject);

                //Instantiate the muzzle flash for non scope aim types (otherwhise it may block the player's view

                if (aimType.GetAimType_Index() == 1 && isAiming)
                {

                }
                else
                {
                    if (muzzlePrefab != null)
                    {
                        GameObject tempMuzzle = Instantiate(muzzlePrefab, muzzleSpawn.position, muzzleSpawn.rotation);
                        tempMuzzle.transform.parent = muzzleSpawn.transform;
                    }
                }
                //If the gun has a light set, it shows
                if (muzzleLight != null)
                {
                    StartCoroutine(LightTime());
                }
                //If the gun has a bullet case prefab, it spawns
                if (bulletCasePrefab != null)
                {
                    GameObject tempCase = Instantiate(bulletCasePrefab, bulletCaseSpawn.position, bulletCaseSpawn.rotation);
                    if (tempCase.GetComponent<Rigidbody>() != null)
                    {
                        tempCase.GetComponent<Rigidbody>().AddForce(bulletCaseSpawn.forward * -1, ForceMode.Impulse);
                    }
                }
                //If the gun has a sound prefab set, it spawns
                if (shotSound != null)
                {
                    Instantiate(shotSound, bulletCaseSpawn.position, bulletCaseSpawn.rotation);
                }
                if (shotExtraSound != null)
                {
                    StartCoroutine(Spawn_ExtraShotSound());
                }
                currentBullets--;
                //Adds the recoil and bullet deflection
                if (isAiming)
                {
                    currentRecoil += recoilAument_Aiming;
                }
                else
                {
                    currentRecoil += recoilAument;
                }
                //Updates the UI if it has one
                if (ammo != null)
                {
                    ammo.text = currentBullets.ToString() + "/" + inventoryAmmo.ToString();
                }
                // Limits the recoil
                if (currentRecoil > maxRecoil)
                {
                    currentRecoil = maxRecoil;
                }
                currentMaxRecoil = currentRecoil;
                //Blocks the gun until the time between shots has finished
                canShoot = false;
                fingerUp = false;
                if (currentBullets > 0)
                {
                    StartCoroutine(ShotWait());
                }

            }
            else
            {
                //If there are not bullets remaining in the magazine, but the inventory has, it performs a reload
                if (inventoryAmmo > 0 && currentBullets <= 0)
                {
                    canPutChamberBullet = false;
                    ReloadWeapon();
                }
                if(noBulletShotSound!= null && currentBullets <=0 && (!isReloading || !isReloading_Full))
                {
                    Instantiate(noBulletShotSound, bulletCaseSpawn.position, bulletCaseSpawn.rotation);
                }
            }
        }

    }
    //If the gun has a aim type set, it will try to aim
    public void TryToAim(GameObject cam)
    {
        if(aimType!= null && (!isReloading_Full|| isReloading))
        {
            //Applies aim animation
            isAiming = true;
            if(playerAnimator!= null)
            {
                playerAnimator.SetBool("IsAiming", true);
            }
            //Applies zoom
            if(weaponController.GetPlayerMovement().Get_ControllerType_Index() != 2)
            {
                if (cam.GetComponent<CinemachineFreeLook>() != null)
                {
                    aimType.BaseFov(cam.GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView);
                }
                else if (cam.GetComponent<Camera>() != null)
                {
                    aimType.BaseFov(cam.GetComponent<Camera>().fieldOfView);
                }
                aimType.Aim(cam);
            }

        }
    }
    //Cancel the aim
    public void TryCancelAim(GameObject cam)
    {
        if(aimType != null)
        {
            //Applies animation
            isAiming = false;
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("IsAiming", false);
            }
            //Returns to the normal zoom
            if (weaponController.GetPlayerMovement().Get_ControllerType_Index() != 2)
            {
                aimType.CancelAim(cam);
            }

        }

    }
    //If the player has bullets in it's inventory, and the current bullets are less than the magazine size, perform a reload
    public void ReloadWeapon()
    {
        if (inventoryAmmo > 0 && currentBullets < magazineSize)
        {
            weaponController.OnReloadRig();
            Debug.Log("Trying to reload");
            StartCoroutine(StartReload());
        }
    }
    //Logic that allows to shot while reloading
    private void Reload_ShootReloading_Logic()
    {
        if (isReloading && reloadIntervalsTime > 0)
        {
            int tempBullets = 0;
            int tempMagazine = magazineSize;
            //If it's reloading and already has a bullet in the magazine, the player can shoot again
            if (currentBullets > 0)
            {
                canShoot = true;
            }
            //Adds one to the max gun capacity if the bool is turned on (only applies when the player reloads the gun when it has more than 0 bullets inside left)
            if (canPutChamberBullet)
            {
                tempMagazine += 1;
            }
            //This makes that only one sound spawns each time a bullet is inserted, otherwhise the sound will spam
            if(cnt_IdleReloadSound < 1)
            {
                if(idleReloadSound != null)
                {
                    StartCoroutine(Spawn_IdleReloadSound());
                }
                cnt_IdleReloadSound++;
            }
            reloadIntervalsTime -= Time.deltaTime;
            //This verifies that another chunk of bullets can be added, a "BulletsAdd". This can be posible only if the player has bullets in its inventory
            if (reloadIntervalsTime <= 0 && currentBullets < tempMagazine)
            {
                //Stops realoading if the player doen't have bullets in its inventory
                if (inventoryAmmo <= 0)
                {
                    isReloading = false;
                    return;
                }
                // If the player has less bullets in its inventory than the bullets add, all the remaining bullets are added and the inventory sets to 0
                if (inventoryAmmo < bulletsAdd && inventoryAmmo > 0)
                {
                    tempBullets = inventoryAmmo;
                    inventoryAmmo = 0;
                }
                //Normal reload cycle
                else 
                {
                    tempBullets = bulletsAdd;
                    inventoryAmmo -= bulletsAdd;
                }
                currentBullets += tempBullets;
                //Prevent errors of adding more bullets than the weapon can take, and the extra are returned to the inventory
                if (currentBullets >= tempMagazine)
                {
                    int tempReturn = currentBullets - tempMagazine;
                    inventoryAmmo += tempReturn;
                    currentBullets = tempMagazine;
                    isReloading = false;
                    //Updates the main inventory bullet count
                    weaponController.OnChangeAmmoValue(ammoType, inventoryAmmo);
                    //Starts the reload finisher
                    StartCoroutine(FinishReload());
                    return;
                    //Stops the reloading if the player doesn't have any remaining bullets (for the cycle to not repeat again)
                }else if(currentBullets < tempMagazine && inventoryAmmo <= 0)
                {
                    isReloading = false;
                    weaponController.OnChangeAmmoValue(ammoType, inventoryAmmo);
                    StartCoroutine(FinishReload());
                }
                //Updates UI
                if (ammo != null)
                {
                    ammo.text = currentBullets.ToString() + "/" + inventoryAmmo.ToString();
                }
                //Reset the cycle
                weaponController.OnChangeAmmoValue(ammoType, inventoryAmmo);
                cnt_IdleReloadSound = 0;
                reloadIntervalsTime = resetReloadTime;
            }
        }
    }
    //Applies recoil
    private void RecoilLogic()
    {
        if (calculateRecoil)
        {
            //Substracts recoil while not shooting
            if(currentRecoil > 0)
            {
                currentRecoil -= ((Time.deltaTime / timeToBaseRecoil) * currentMaxRecoil);
                if(currentRecoil <= 0)
                {
                    currentRecoil = 0;
                    calculateRecoil = false;
                }
            }
        }
    }
    private void ConstantAimLogic()
    {
        if (isAiming)
        {
            if (weaponController != null)
            {
                if (weaponController.Get_ControllerType_Index() == 1)
                {
                    weaponController.GetPlayerMovement().GetMovement().RotateCharacter();
                }

            }
        }

    }
    #endregion
    #region Get Set
    public void GetInventoryAmmo(int newammo)
    {
        //Set the inventory ammo of the gun.
        inventoryAmmo = newammo;
        if (ammo != null)
        {
            ammo.text = currentBullets.ToString() + "/" + inventoryAmmo.ToString();
        }
    }
    public void GetCurrentBullets(int currentAmmo)
    {
        //Set the current bullets inside the gun.
        currentBullets = currentAmmo;
        if(currentBullets > 0)
        {
            canShoot = true;
            fingerUp = true;
        }
    }
    public int SendInventoryAmmo()
    {
        return inventoryAmmo;
    }
    public BaseAmmo SendAmmoType()
    {
        return ammoType;
    }
    public void GetWeaponController(WeaponController controller)
    {
        weaponController = controller;
    }
    public int SendCurrentBullets()
    {
        return currentBullets;
    }
    public void SendPlayerAnimator(Animator anim)
    {
        playerAnimator = anim;
    }
    public int GetMagazineSize()
    {
        return magazineSize;
    }
    public void OnTakeNewGun(Transform shootPos, Transform objectSpawnPos, bool spawnObj)
    {
        //Every gun needs a shotposition, but it depends on the camera type. FPS is recommended the camera, tps to. But top down for example requieres the gun's barrel or similar to shot
        shotSpawn = shootPos;
        // The place where the bullet object (the one that is used for art, not damage) will appear
        objectSpawn = objectSpawnPos;
        //A bool that sets if the bullet object will appear (False is recommended for fps unless you want to test)
        spawnObject = spawnObj;

        if(currentBullets <= 0)
        {
            ReloadWeapon();
        }
    }
    public GameObject SendWorldObject()
    {
        return worldObject;
    }
    public void ChangeShotSpawn(Transform position)
    {
        shotSpawn = position;
    }
    public Transform GetShotPosition()
    {
        return shotSpawn;
    }
    public void OnFingerUp()
    {
        Debug.Log("Tryed fingerUp");
        fingerUp = true;
    }
    public void GetPlayerAnimator(Animator playerAnim)
    {
        playerAnimator = playerAnim;
        if (playerGunOverride != null)
        {
            playerAnimator.runtimeAnimatorController = playerGunOverride;
            playerAnimator.enabled = false;
            playerAnimator.enabled = true;
        }
        if (gunAnimator != null)
        {
            gunAnimator.SetBool("FinishReload", false);
            
        }
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("FinishReload", false);
        }
    }
    public  Transform SendLeftArmPos()
    {
        return l_Arm_Pos;
    }
    public void OnChangeAim(bool newValue)
    {
        isAiming = newValue;
    }
    public void OnSetAimRenderers(List<Renderer> playerRenderers)
    {
        aimType.GetRenderers(playerRenderers, renderers);
    }
    public void SetUIElements(Image ammoIcon, TMP_Text txt)
    {
        if(ammoIcon != null)
        {
            ammoIcon.sprite = ammoType.GetAmmoIcon();
        }
        if(txt!= null)
        {
            ammo = txt;
            ammo.text = currentBullets.ToString() + "/" + inventoryAmmo.ToString();
        }
    }
    public List<Material> GetGunMaterials()
    {
        return materials;
    }
    #endregion

    #region Coroutines
    IEnumerator ShotWait()
    {
        //The wait time to shot again, needed to not spam shots and make the gun empty it's bullets almost inmediatly.
        Debug.Log("Start shot wait");
        yield return new WaitForSeconds(timeBetweenShots);
        canShoot = true;
        if (_rateType == RateType.Fullauto && !fingerUp)
        {
            fingerUp = true;
            TryToShoot();
        }
        Debug.Log("Finish shot wait");
    }
    //If the gun can't shot while reloading, this coroutine is more performant than the update shot while reloading
    IEnumerator ReloadWeaponCompletely()
    {
        if (idleReloadSound != null)
        {
            StartCoroutine(Spawn_IdleReloadSound());
        }
        //Waits to add a chunk of bullets (bullets add)
        yield return new WaitForSeconds(reloadIntervalsTime);
        int tempBullets = 0;
        int tempMagazine = magazineSize;
        //Verifies if the gun can add one more bullet to it's capacity if the player reloaded while having more than 0 bullets in the gun
        if (canPutChamberBullet)
        {
            tempMagazine += 1;
        }
        //If the player hasn't enough bullets in the inventory to fill the bullets add, it takes all the remaining bullets and sets the inventory to 0
        if (inventoryAmmo < bulletsAdd && inventoryAmmo > 0)
        {
            tempBullets = inventoryAmmo;
            inventoryAmmo = 0;
        }
        //Normal reload cycle
        else
        {
            tempBullets = bulletsAdd;
            inventoryAmmo -= bulletsAdd;
        }
        currentBullets += tempBullets;
        //Updates UI
        if (ammo != null)
        {
            ammo.text = currentBullets.ToString() + "/" + inventoryAmmo.ToString();
        }
        //Prevents that the gun's magazine have more bullets than the max capacity, and returns the surplus to the inventory
        if (currentBullets >= tempMagazine)
        {
            int tempReturn = currentBullets - tempMagazine;
            inventoryAmmo += tempReturn;
            currentBullets = tempMagazine;
            if(inventoryAmmo < 0)
            {
                int backReturn = 0 - inventoryAmmo;
                currentBullets -= backReturn;
                inventoryAmmo = 0;
            }
            //Updates UI
            if (ammo != null)
            {
                ammo.text = currentBullets.ToString() + "/" + inventoryAmmo.ToString();
            }
            //Start the finishing reload animation
            weaponController.OnChangeAmmoValue(ammoType, inventoryAmmo);
            StartCoroutine(FinishReload());
            yield break;
        }
        else
        {
            weaponController.OnChangeAmmoValue(ammoType, inventoryAmmo);
            StartCoroutine(ReloadWeaponCompletely());
        }
    }
    IEnumerator StartReload()
    {
        //Applies animation
        isReloading = true;
        if (!shootWhileReload)
        {
            isReloading_Full = true;
        }

        if (gunAnimator != null)
        {
            gunAnimator.SetBool("FinishReload", false);
            gunAnimator.Play("Reload_Start");
            gunAnimator.SetFloat("Reload_SpeedMultiplier", (1 / resetReloadTime));
        }
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("FinishReload", false);
            int playerAnimVal = playerAnimator.GetInteger("Gun_AnimVal");
            if (playerAnimVal > 3 && playerAnimVal != 6)
            {
                playerAnimator.Play("Reload_Crouch_Start");
            }
            else
            {
                playerAnimator.Play("Reload_Stand_Start");
            }
            playerAnimator.SetFloat("Reload_SpeedMultiplier", (1/resetReloadTime));
        }
        //verifies if the gun can have a extra bullet in this reload
        if (currentBullets > 0 && chamberBullet)
        {
            canPutChamberBullet = true;
        }
        else
        {
            canPutChamberBullet = false;
        }
        //Shows on the 3D Model the magbullet object if it has one and you want this detail (only when the mag has remaining bullets)
        if (currentBullets > 0)
        {
            if (magBullet != null && !magBullet.activeSelf)
            {
                magBullet.SetActive(true);
            }
        }
        //Hides the 3D model of the mag bullet
        else
        {
            if (magBullet != null && magBullet.activeSelf)
            {
                magBullet.SetActive(false);
            }
        }
        if (startReloadSound != null)
        {
            StartCoroutine(Spawn_StartReloadSound());
        }
        yield return new WaitForSeconds(resetReloadTime);
        if (shootWhileReload)
        {
            isReloading = true;
        }
        else
        {
            StartCoroutine(ReloadWeaponCompletely());
        }
    }
    IEnumerator FinishReload()
    {
        //Finish reload animations
        if (gunAnimator != null)
        {
            gunAnimator.SetFloat("Reload_SpeedMultiplier", (1 / resetReloadTime));
        }
        if (playerAnimator != null)
        {
            playerAnimator.SetFloat("Reload_SpeedMultiplier", (1 / resetReloadTime));
        }
        if (finishReloadSound != null)
        {
            StartCoroutine(Spawn_FinishReloadSound());
        }

        yield return new WaitForSeconds(resetReloadTime);
        if (gunAnimator != null)
        {
            gunAnimator.SetBool("FinishReload", true);
            gunAnimator.SetFloat("Reload_SpeedMultiplier", (1 / resetReloadTime));
        }
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("FinishReload", true);
            playerAnimator.SetFloat("Reload_SpeedMultiplier", (1 / resetReloadTime));
        }
        //Allows to shot again
        StartCoroutine(AnimationReloadReset());
        isReloading_Full = false;
        fingerUp = true;
        canShoot = true;
        currentRecoil = 0;
        if(weaponController != null)
        {
            weaponController.FinishingReload();
        }
        //Updates UI
        if (ammo != null)
        {
            ammo.text = currentBullets.ToString() + "/" + inventoryAmmo.ToString();
        }
    }

    IEnumerator AnimationReloadReset()
    {
        yield return new WaitForSeconds(1f);
        if (gunAnimator != null)
        {
            gunAnimator.SetBool("FinishReload", false);
        }
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("FinishReload", false);
        }
    }
    IEnumerator LightTime()
    {
        if (muzzleLight != null && !muzzleLight.activeSelf)
        {
            muzzleLight.SetActive(true);
        }
        yield return new WaitForSeconds(0.1f);
        if(muzzleLight!= null && muzzleLight.activeSelf)
        {
            muzzleLight.SetActive(false);
        }
    }

    //Sound sync coroutines
    IEnumerator Spawn_ExtraShotSound()
    {
        yield return new WaitForSeconds(shotExtraSound_WaitTime);
        Instantiate(shotExtraSound, bulletCaseSpawn.position, bulletCaseSpawn.rotation);
    }
    IEnumerator Spawn_StartReloadSound()
    {
        yield return new WaitForSeconds(startReloadSound_WaitTime);
        Instantiate(startReloadSound, bulletCaseSpawn.position, bulletCaseSpawn.rotation);
    }
    IEnumerator Spawn_IdleReloadSound()
    {
        yield return new WaitForSeconds(idleReloadSound_WaitTime);
        Instantiate(idleReloadSound, bulletCaseSpawn.position, bulletCaseSpawn.rotation);
    }
    IEnumerator Spawn_FinishReloadSound()
    {
        yield return new WaitForSeconds(finishReloadSound_WaitTime);
        Instantiate(finishReloadSound, bulletCaseSpawn.position, bulletCaseSpawn.rotation);
    }
    #endregion
}
