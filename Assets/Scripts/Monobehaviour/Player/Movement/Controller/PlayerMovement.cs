using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using TMPro;


public enum Movement_Camera_Type{Fps, Tps, Isometric, Fps_Tps, Custom}
public class PlayerMovement : MonoBehaviour
{
    #region Serialized variables
    [Header("Required Components")]

    [Tooltip("Transform where the raycast that detects ground will cast")]
    [SerializeField] Transform groundPosition;

    [Tooltip("Layer that is considerer ground and where the player can jump")]
    [SerializeField] LayerMask groundMask;

    [SerializeField] float groundDistance;

    [Tooltip("Transform where the raycast that detects if the player can stand up will cast")]
    [SerializeField] Transform standPosition;
    [SerializeField] float standRayDistance = 0.7f;

    [Tooltip("Health script that calculates the remaining life of the player")]
    [SerializeField] Health hp;

    [Space]


    [Header("Movement types settings, required")]


    [Tooltip("Select the movement and camera control type for start. If you choose Fps_Tps, the fps style will be the default state")]
    [SerializeField] Movement_Camera_Type move_Camera_Type;

    [Tooltip("Fps movement, must be attached to the same GameObject of this script")]
    [SerializeField] BaseMovement fpsMovement;

    [Tooltip("Tps movement, must be attached to the same GameObject of this script")]
    [SerializeField] BaseMovement tpsMovement;

    [Tooltip("Isometric movement, must be attached to the same GameObject of this script")]
    [SerializeField] BaseMovement isoMovement;

    [Tooltip("Custom movement type. You can override a movement type while running the game for one custom, useful to change to drive a vehicle, or you can use it" +
    "if you want to create your own movement style. If you don't need it, leave it empty")]
    [SerializeField] BaseMovement customMovement;



    [Space]


    [Header("Camera control type settings, required")]


    [Tooltip("Fps camera move, must be attached to the same GameObject of this script")]
    [SerializeField] BaseCameraMove fpsCameraMove;

    [Tooltip("Tps camera move, must be attached to the same GameObject of this script")]
    [SerializeField] BaseCameraMove tpsCameraMove;

    [Tooltip("Isometric camera move, must be attached to the same GameObject of this script")]
    [SerializeField] BaseCameraMove isoCameraMove;

    [Tooltip("Custom camera movement. You can override a movement type while running the game for one custom, useful to change to drive a vehicle,or you can use it" +
    "if you want to create your own camera movement style. If you don't need it, leave it empty")]
    [SerializeField] BaseCameraMove customCameraMove;


    [Space]


    [Header("Components of cameras and movement types")]


    [Tooltip("Fps style objects, needed for this style to work (example, cameras, containers, extras, interactable collider if needed etc)")]
    [SerializeField] GameObject[] fpsObjects;

    [Tooltip("Tps style objects, needed for this style to work (example, cameras, containers, extras, interactable collider if needed etc)")]
    [SerializeField] GameObject[] tpsObjects;

    [Tooltip("Isometric style objects, needed for this style to work (example, cameras, containers, extras, interactable collider if needed etc)")]
    [SerializeField] GameObject[] isoObjects;

    [Tooltip("Custom objects, the ones that you need for your custom movement to work. You can override them while running. If you don't need them leave it empty")]
    [SerializeField] GameObject[] customObjects;


    [Space]


    [Header("UI")]


    [Tooltip("Needed to pause, unpause game, exit and change settings")]
    [SerializeField] GameObject ui_Controller;

    [Tooltip("It will perform when the game pauses. If you don't need it leave it empty")]
    [SerializeField] GameObject pauseSound;

    [Tooltip("It will perform when the game unpauses. If you don't need it leave it empty")]
    [SerializeField] GameObject unpauseSound;

    [Tooltip("Image that gives informationt to the player about interactions use1. The movement type should import UI Libraries")]
    [SerializeField] Image img1;

    [Tooltip("Aditional Info via text for use1")]
    [SerializeField] TMP_Text tmp_Txt_1;

    [Tooltip("Aditional Info via text for use1. Use this if you aren't using TMPro")]
    [SerializeField] Text txt_1;

    [Tooltip("Image that gives informationt to the player about interactions use2. The movement type should import UI Libraries")]
    [SerializeField] Image img2;

    [Tooltip("Aditional Info via text for use2")]
    [SerializeField] TMP_Text tmp_Txt_2;

    [Tooltip("Aditional Info via text for use1. Use this if you aren't using TMPro")]
    [SerializeField] Text txt_2;

    [Tooltip("Pointer Image that is used for aiming to. If Isocamera it will be disabled")]
    [SerializeField] Image crosshair;


    [Space]


    [Header("Sound & particles")]

    [Tooltip("It can be performed by a movement type while the player it's moving. If you don't need it leave it empty")]
    [SerializeField] GameObject[] footstepSounds;

    [Tooltip("It can appear by a movement type while the player it's moving. If you don't need it leave it empty")]
    [SerializeField] GameObject footstepParticle;

    [Tooltip("Where the footsteps sounds and particles will spawn")]
    [SerializeField] Transform particlePos;

    [Tooltip("The time that has to pass before a sound and particle can spawn")]
    [SerializeField] float timeBetweenSteps_Base;

    [Tooltip("It can be performed by a movement type if the player jumps. If you don't need it leave it empty")]
    [SerializeField] GameObject jumpSound;


    [Space]


    [Header("Optional components")]


    [Tooltip("If the controller needs to have weapons, put a controller, otherwise leave it empty")]
    [SerializeField] WeaponController weaponController;

    [Tooltip("The object that has a follow rotation script, used to change if player's body will follow the camera view or will see forward. Use it only with rigged characters. Else leave it empty")]
    [SerializeField] FollowRotation Arms_RotationTarget;

    [Tooltip("Fps Arms target, a game object that will be followed in x rotation by the model arms. If you don't need whis you can leave it empty")]
    [SerializeField] Transform ArmsTarget_Fps;

    [Tooltip("Fps Arms target, a game object that will be followed in x rotation by the model arms. If you don't need whis you can leave it empty")]
    [SerializeField] Transform ArmsTarget_Tps;

    [Tooltip("Tps cinemachine brain camera object, used to rotate the player column to the aim position on y")]
    [SerializeField] Transform CameraBrain_Object_Tps;

    [Tooltip("Fps Arms target, a game object that will be followed in x rotation by the model arms (for iso movement it's recommended a object that doesn't move or rotate from it's parent)" +
    ". If you don't need whis you can leave it empty")]
    [SerializeField] Transform ArmsTarget_Iso;

    [Tooltip("A GameObject that will be followed in x rotation by the model arms. If you don't need whis you can leave it empty")]
    [SerializeField] Transform ArmsTarget_Custom;

    [Tooltip("Controller type index means how the weapon controller (if you use it) and zoom in weapons will behave."
    + "2 means that there will not be any zoom. " 
    + "other number will try to get a default camera or cinemachine freelook camera")]
    [SerializeField] int weaponControllerType_Index;

    [Tooltip("If you are using the included custom shader, it allows to calculate or not dithering, recommended for third person controllers")]
    [SerializeField] bool calculateDither_Custom;

    [Space]


    [Header("Debug variables")]
    [SerializeField] bool canStand;
    [SerializeField] private bool hasGun = false;

    #endregion
    #region Private variables

    private Transform shotPosition;
    private PlayerInputs inputs;

    private List<Renderer> renderers = new List<Renderer>();
    private List<Material> materials = new List<Material>();

    private Vector2 moveMouse;
    private Vector2 mousePosition;
    private Vector2 move;

    private float coyoteTime;
    private float resetCoyote;
    private float ptfSpeed;

    private int controllerType_Index = 0;

    private bool canPause = true;
    private bool isPaused = false;
    private bool isOnGround;
    private bool isJumping;
    private bool isHybrid = false;
    private bool canChangeView = true;
    private bool isDithering = false;
    private bool isOnPtf_LR = false;
    private bool isOnPtf_UD = false;
    private bool isTryMove = false;
    private BaseMovement currentMovementType;
    private BaseCameraMove currentCameraMoveType;


    private Move_LeftRight currentPf_LR;
    private Move_UpDown currentPf_UD;
    private Transform currentArms_Target;
    private Collider playerCollider;

    #endregion

    #region Main Functions
    private void Awake()
    {
        //Set the input actions
        inputs = new PlayerInputs();
        inputs.Fps.Movement.performed += ctx => move = ctx.ReadValue<Vector2>();
        inputs.Fps.Movement.canceled += ctx => OnStopMovement();
        inputs.Fps.MouseInput.performed += ctx => moveMouse = ctx.ReadValue<Vector2>();
        inputs.Fps.MousePosition.performed += ctx => mousePosition = ctx.ReadValue<Vector2>();
        inputs.Fps.Jump.performed += ctx => OnJump();
        inputs.Fps.Run.performed += ctx => OnRun();
        inputs.Fps.Crouch.performed += ctx => OnCrouch();
        inputs.Fps.Shoot.performed += ctx => OnShoot();
        inputs.Fps.Shoot.canceled += ctx => OnCancelShoot();
        inputs.Fps.Aim.performed += ctx => OnAim();
        inputs.Fps.Aim.canceled += ctx => OnCancelAim();
        inputs.Fps.Reload.performed += ctx => OnReload();
        inputs.Fps.Interact_1.performed += ctx => OnInteract_1();
        inputs.Fps.Interact_2.performed += ctx => OnInteract_2();
        inputs.Fps.ChangeView.performed += ctx => OnChangeView();
        inputs.Fps.Pause.performed += ctx => OnChangePause();
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
        //Set controller type
        if (move_Camera_Type == Movement_Camera_Type.Fps)
        {
            SetFps();
            isHybrid = false;
        }
        else if (move_Camera_Type == Movement_Camera_Type.Tps)
        {
            SetTps();
            isHybrid = false;
        }
        else if (move_Camera_Type == Movement_Camera_Type.Isometric)
        {
            SetIso();
            isHybrid = false;
        }
        else if (move_Camera_Type == Movement_Camera_Type.Fps_Tps)
        {
            SetFps();
            isHybrid = true;
        }
        else if(move_Camera_Type == Movement_Camera_Type.Custom)
        {
            if(customMovement != null)
            {
                SetCustom(customMovement, customCameraMove, ArmsTarget_Custom, customObjects, weaponControllerType_Index, calculateDither_Custom);
            }
            else
            {
                Debug.LogError("The custom movement it's empty, you shoul assign something to all custom variables for it to work at the start");
            }
        }
        coyoteTime = currentMovementType.GetCoyoteTime();
        resetCoyote = coyoteTime;
    }
    private void Start()
    {
        //If the controller has the weapons module, it will set the shot position
        if (weaponController != null)
        {
            weaponController.ChangeShotPos(currentMovementType.GetShotPosition());
            weaponController.ChangeCanSpawnShotObj(currentMovementType.CanSpawnShotObject());
        }
        //Wait time to disable UI. Needed due UI's awake & start functions
        if (ui_Controller.activeSelf)
        {
            StartCoroutine(WaitToDisablePause());
        }
        if (currentCameraMoveType.GetLockMouse())
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    //Applies logic
    private void Update()
    {
        OnMovement(move);
        OnMovePtf_LR();
        OnMovePtf_UD();
        OnLook();
        ReadingGround();
        ReadStand();
        OnAnimationLogic();
        OnSearchInteract();

    }
    private void OnMovement(Vector2 movementRead)
    {
        isTryMove = true;
        currentMovementType.Movement(movementRead);
    }
    private void OnMovePtf_LR()
    {
        if (isOnPtf_LR && currentPf_LR != null)
        {
            transform.position = new Vector3(transform.position.x + currentPf_LR.GetSpeed(), transform.position.y, transform.position.z);
        }
    }
    private void OnMovePtf_UD()
    {
        if (isOnPtf_UD && currentPf_UD != null)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + currentPf_UD.GetSpeed(), transform.position.z);
        }
    }
    private void OnStopMovement()
    {
        move = Vector2.zero;
        currentMovementType.StopMoving();
        isTryMove = false;
    }
    private void OnLook()
    {
        if (currentCameraMoveType != null)
        {
            if (move_Camera_Type == Movement_Camera_Type.Isometric)
            {
                currentCameraMoveType.Look(mousePosition);
            }
            else
            {
                currentCameraMoveType.Look(moveMouse);
            }

        }

    }
    private void OnJump()
    {
        currentMovementType.Jump();
    }
    private void OnRun()
    {
        currentMovementType.Run();
        currentMovementType.ChangeAiming(false);
    }
    private void OnCrouch()
    {
        currentMovementType.Crouch();
    }
    private void OnShoot()
    {
        if (weaponController != null)
        {
            weaponController.TryToShoot();
        }
    }
    private void OnCancelShoot()
    {
        if (weaponController != null)
        {
            weaponController.OnTryCancelShoot();
        }
    }
    private void OnAim()
    {
        weaponController.OnTryAim(currentCameraMoveType.GetCamera());
        currentMovementType.ChangeRun(false);
        currentMovementType.ChangeAiming(true);
    }
    private void OnCancelAim()
    {
        weaponController.OnTryCancelAim(currentCameraMoveType.GetCamera());
        currentMovementType.ChangeAiming(false);
    }
    private void OnReload()
    {
        if (weaponController != null)
        {
            weaponController.OnTryReload();
        }
    }
    private void OnInteract_1()
    {
        currentMovementType.Interact_1();
    }
    private void OnInteract_2()
    {
        currentMovementType.Interact_2();
    }
    private void OnSearchInteract()
    {
        currentMovementType.SearchInteractable();
    }
    private void OnChangeView()
    {
        if (isHybrid && canChangeView)
        {
            if (controllerType_Index == 0)
            {
                SetTps();
            } else if (controllerType_Index == 1)
            {
                SetFps();
            }
        }
    }
    private void ReadingGround()
    {
        isOnGround = Physics.CheckSphere(groundPosition.position, groundDistance, groundMask);
        isJumping = currentMovementType.IsJumping();
        if (!isOnGround && isJumping == false)
        {
            StartCoroutine(CoyoteTimeWait());
        }
        currentMovementType.ReadGround(isOnGround);
    }
    private void ReadStand()
    {
        canStand = Physics.Raycast(standPosition.position, standPosition.up, standRayDistance);
        currentMovementType.TryToStandUp(canStand);
    }
    private void OnAnimationLogic()
    {
        currentMovementType.AnimationLogic();
    }
    public void OnChangePause()
    {
        if (canPause)
        {
            if (!isPaused)
            {
                if (!ui_Controller.activeSelf)
                {
                    Time.timeScale = 0;
                    if(weaponController != null)
                    {
                        weaponController.SetCanShot(false);
                    }
                    ui_Controller.SetActive(true);
                    if (ui_Controller.GetComponent<Animator>() != null)
                    {
                        ui_Controller.GetComponent<Animator>().Play("Pause");
                    }
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    if (pauseSound != null)
                    {
                        Instantiate(pauseSound, transform.position, transform.rotation);
                    }
                    isPaused = true;
                }
            }
            else if (isPaused)
            {
                if (ui_Controller.activeSelf)
                {
                    if (weaponController != null)
                    {
                        weaponController.SetCanShot(true);
                    }
                    if (unpauseSound != null)
                    {
                        Instantiate(unpauseSound, transform.position, transform.rotation);
                    }
                    if (ui_Controller.GetComponent<Animator>() != null)
                    {
                        ui_Controller.GetComponent<Animator>().Play("Unpause");
                    }
                    if (currentCameraMoveType.GetLockMouse())
                    {
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }
                    else
                    {
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                    }
                    ui_Controller.SetActive(false);
                    StartCoroutine(UnpauseSmooth());
                }
            }
        }
        else
        {
            if (ui_Controller.activeSelf)
            {
                ui_Controller.SetActive(false);
            }
        }
       
    }
    private void OnEnable()
    {
        inputs.Fps.Enable();
    }
    private void OnDisable()
    {
        inputs.Fps.Disable();
    }
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            if (currentCameraMoveType.GetLockMouse())
            {
                if (isPaused)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else if(!isPaused)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
            else if(!currentCameraMoveType.GetLockMouse())
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

        }
        else if(!focus)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
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
        currentMovementType.ChangeHasGun(hasGun);
        if (!hasGun && currentArms_Target != null &&currentArms_Target.gameObject.activeSelf)
        {
            currentArms_Target.gameObject.SetActive(false);
        } else if (hasGun && currentArms_Target != null && !currentArms_Target.gameObject.activeSelf)
        {
            currentArms_Target.gameObject.SetActive(true);
        }
    }
    public bool SendHasGun()
    {
        return hasGun;
    }
    public Animator GetPlayerAnimator()
    {
        return currentMovementType.GetPlayerAnimator();
    }
    public List<Renderer> GetPlayerRenderers()
    {
        return renderers;
    }
    public int Get_ControllerType_Index()
    {
        return controllerType_Index;
    }
    public BaseMovement GetMovement()
    {
        return currentMovementType;
    }
    public BaseCameraMove GetBaseCameraMove()
    {
        return currentCameraMoveType;
    }
    public Collider GetPlayerCollider()
    {
        return playerCollider;
    }

    public GameObject[] GetFootStepSounds()
    {
        return footstepSounds;
    }
    public GameObject GetJumpSound()
    {
        return jumpSound;
    } 
    public GameObject GetFootStepParticle()
    {
        return footstepParticle;
    }
    public Transform GetParticlePosition()
    {
        return particlePos;
    }
    public float GetTimeBetweenSteps()
    {
        return timeBetweenSteps_Base;
    }
    public PlayerMovement GetPlayerMovement()
    {
        return this;
    }
    public bool GetIsDither()
    {
        return isDithering;
    }
    public void ChangeCanPause(bool newValue)
    {
        canPause = newValue;
    }
    public void SetChangeView(bool newValue)
    {
        canChangeView = newValue;
    }
    public void SetCurrentPf_L_R(Move_LeftRight newPf, bool isOnPfVal)
    {
        currentPf_LR = newPf;
        isOnPtf_LR = isOnPfVal;
    }
    public void SetCurrentPf_UD(Move_UpDown newPf, bool isOnPfVal)
    {
        currentPf_UD = newPf;
        isOnPtf_UD = isOnPfVal;
    }
    public void SetIsHybrid(bool newIsHybrid)
    {
        //True allows to the player to change between fps and tps movement and camera views.
        //If you use custom movement for vehicles you should call this function to and set a value.
        isHybrid = newIsHybrid;
    }
    public void SetUIInfo(TMP_Text tmpTxt_1, TMP_Text tmpTxt_2, Text newtxt_1, Text newtxt_2, Image newimg1, Image newimg2)
    {
        currentMovementType.SetUIInfo(tmpTxt_1, tmpTxt_2, newtxt_1, newtxt_2, newimg1, newimg2);
    }

    #endregion
    #region Set Controller Type
    public void SetFps()
    {
        currentMovementType = fpsMovement;
        currentCameraMoveType = fpsCameraMove;
        isDithering = false;
        if(ArmsTarget_Fps!= null)
        {
            currentArms_Target = ArmsTarget_Fps;
        }
        controllerType_Index = 0;
        if (materials != null && materials.Count > 0)
        {
            foreach (Material mtl in materials)
            {
                mtl.SetInt("_Calculate_Dither", 0);
            }
        }
        if (fpsObjects != null && fpsObjects.Length > 0)
        {
            foreach (GameObject gm in fpsObjects)
            {
                if (!gm.activeSelf)
                {
                    gm.SetActive(true);
                }
            }
        }
        if (tpsObjects != null && tpsObjects.Length > 0)
        {
            foreach (GameObject gm in tpsObjects)
            {
                if (gm.activeSelf)
                {
                    gm.SetActive(false);
                }
            }
        }
        if (isoObjects != null && isoObjects.Length > 0)
        {
            foreach (GameObject gm in isoObjects)
            {
                if (gm.activeSelf)
                {
                    gm.SetActive(false);
                }
            }
        }
        if (customObjects != null && customObjects.Length > 0)
        {
            foreach (GameObject gm in customObjects)
            {
                if (gm.activeSelf)
                {
                    gm.SetActive(false);
                }
            }
        }
        playerCollider = currentMovementType.GetPlayerCollider();
        if(Arms_RotationTarget!= null)
        {
            Arms_RotationTarget.ChangeEulerAngles(currentCameraMoveType.GetCamera().transform);
        }
        coyoteTime = currentMovementType.GetCoyoteTime();
        resetCoyote = coyoteTime;
        fpsMovement.SetPlayerMovement(this);
        fpsMovement.SetSound_Particles(footstepSounds, jumpSound, footstepParticle, particlePos, timeBetweenSteps_Base);
        currentMovementType.SetUIInfo(tmp_Txt_1, tmp_Txt_2, txt_1, txt_2, img1, img2);
        if(crosshair!= null && !crosshair.gameObject.activeSelf)
        {
            crosshair.gameObject.SetActive(true);
        }
        hp.SetCameraFX(fpsCameraMove.GetCameraFX());
        fpsCameraMove.GetCameraFX().SetHP(hp);
        if (weaponController != null)
        {
            weaponController.ChangeShotPos(currentMovementType.GetShotPosition());
            weaponController.ChangeCanSpawnShotObj(currentMovementType.CanSpawnShotObject());
        }
    }
    public void SetTps()
    {
        currentMovementType = tpsMovement;
        currentCameraMoveType = tpsCameraMove;
        isDithering = true;
        if (ArmsTarget_Tps!= null)
        {
            currentArms_Target = ArmsTarget_Tps;
        }
        controllerType_Index = 1;
        if (currentCameraMoveType.GetCamera().GetComponent<CinemachineFreeLook>()!= null)
        {
            currentCameraMoveType.GetCamera().GetComponent<CinemachineFreeLook>().enabled = false;
            currentCameraMoveType.GetCamera().GetComponent<CinemachineFreeLook>().enabled = true;
        }
        if (materials != null && materials.Count > 0)
        {
            foreach (Material mtl in materials)
            {
                mtl.SetInt("_Calculate_Dither", 1);
            }
        }
        if (tpsObjects != null && tpsObjects.Length > 0)
        {
            foreach (GameObject gm in tpsObjects)
            {
                if (!gm.activeSelf)
                {
                    gm.SetActive(true);
                }
            }
        }
        if (fpsObjects != null && fpsObjects.Length > 0)
        {
            foreach (GameObject gm in fpsObjects)
            {
                if (gm.activeSelf)
                {
                    gm.SetActive(false);
                }
            }
        }
        if (isoObjects != null && isoObjects.Length > 0)
        {
            foreach (GameObject gm in isoObjects)
            {
                if (gm.activeSelf)
                {
                    gm.SetActive(false);
                }
            }
        }
        if (customObjects != null && customObjects.Length > 0)
        {
            foreach (GameObject gm in customObjects)
            {
                if (gm.activeSelf)
                {
                    gm.SetActive(false);
                }
            }
        }
        if (crosshair != null && !crosshair.gameObject.activeSelf)
        {
            crosshair.gameObject.SetActive(true);
        }
        playerCollider = currentMovementType.GetPlayerCollider();
        if(Arms_RotationTarget!= null)
        {
            Arms_RotationTarget.ChangeEulerAngles(CameraBrain_Object_Tps);
        }
        coyoteTime = currentMovementType.GetCoyoteTime();
        resetCoyote = coyoteTime;
        tpsMovement.SetPlayerMovement(this);
        tpsMovement.SetSound_Particles(footstepSounds, jumpSound, footstepParticle, particlePos, timeBetweenSteps_Base);
        currentMovementType.SetUIInfo(tmp_Txt_1, tmp_Txt_2, txt_1, txt_2, img1, img2);
        hp.SetCameraFX(tpsCameraMove.GetCameraFX());
        tpsCameraMove.GetCameraFX().SetHP(hp);
        if (weaponController != null)
        {
            weaponController.ChangeShotPos(currentMovementType.GetShotPosition());
            weaponController.ChangeCanSpawnShotObj(currentMovementType.CanSpawnShotObject());
        }
    }
    public void SetIso()
    {
        currentMovementType = isoMovement;
        currentCameraMoveType = isoCameraMove;
        isDithering = false;
        if (ArmsTarget_Iso!= null)
        {
            currentArms_Target = ArmsTarget_Iso;
        }
        controllerType_Index = 2;
        if (materials != null && materials.Count > 0)
        {
            foreach (Material mtl in materials)
            {
                mtl.SetInt("_Calculate_Dither", 0);
            }
        }
        if (isoObjects != null && isoObjects.Length > 0)
        {
            foreach (GameObject gm in isoObjects)
            {
                if (!gm.activeSelf)
                {
                    gm.SetActive(true);
                }
            }
        }
        if (tpsObjects != null && tpsObjects.Length > 0)
        {
            foreach (GameObject gm in tpsObjects)
            {
                if (gm.activeSelf)
                {
                    gm.SetActive(false);
                }
            }
        }
        if (fpsObjects != null && fpsObjects.Length > 0)
        {
            foreach (GameObject gm in fpsObjects)
            {
                if (gm.activeSelf)
                {
                    gm.SetActive(false);
                }
            }
        }
        if (customObjects != null && customObjects.Length > 0)
        {
            foreach (GameObject gm in customObjects)
            {
                if (gm.activeSelf)
                {
                    gm.SetActive(false);
                }
            }
        }
        if (crosshair != null && crosshair.gameObject.activeSelf)
        {
            crosshair.gameObject.SetActive(false);
        }
        playerCollider = currentMovementType.GetPlayerCollider();
        coyoteTime = currentMovementType.GetCoyoteTime();
        resetCoyote = coyoteTime;
        isoMovement.SetPlayerMovement(this);
        isoMovement.SetSound_Particles(footstepSounds, jumpSound, footstepParticle, particlePos, timeBetweenSteps_Base);
        currentMovementType.SetUIInfo(tmp_Txt_1, tmp_Txt_2, txt_1, txt_2, img1, img2);
        hp.SetCameraFX(isoCameraMove.GetCameraFX());
        isoCameraMove.GetCameraFX().SetHP(hp);
        if (weaponController != null)
        {
            weaponController.ChangeShotPos(currentMovementType.GetShotPosition());
            weaponController.ChangeCanSpawnShotObj(currentMovementType.CanSpawnShotObject());
        }
    }
    private void SetCustom(BaseMovement newMovement, BaseCameraMove newCameraMove, Transform newArmsTarget, GameObject[] newCustomObjects ,int controllerTypeIndex, bool calculateDither)
    {
        //Set a new Custom movement.

        //If you don't want to override some values you can send a null value.

        //If you want to calculate dithering in the included shader send 1, if you don't want to, send 0.

        //Controller type index means how the weapon controller (if you use it) and zoom in weapons will behave.
        //2 means that there will not be any zoom.
        //1 means that the zoom applied will be the tps zoom, used by cinemachine freelook.
        //0 means that the zoom applied will be fps zoom, used by default cameras.
        //If you are using a weapon controller and send another value, an error message will appear.
        if(newMovement != null)
        {
            customMovement = newMovement;
            currentMovementType = customMovement;
        }
        if(newCameraMove != null)
        {
            customCameraMove = newCameraMove;
            currentCameraMoveType = customCameraMove;
        }
        if(newArmsTarget != null)
        {
            ArmsTarget_Custom = newArmsTarget;
            currentArms_Target = ArmsTarget_Custom;
        }
        if(newCustomObjects != null && newCustomObjects.Length > 0)
        {
            customObjects = newCustomObjects;
        }
        controllerType_Index = controllerTypeIndex;
        isHybrid = false;
        isDithering = calculateDither;
        if (calculateDither)
        {
            if (materials != null && materials.Count > 0)
            {
                foreach (Material mtl in materials)
                {
                    mtl.SetInt("_Calculate_Dither", 1);
                }
            }
        }
        else
        {
            if (materials != null && materials.Count > 0)
            {
                foreach (Material mtl in materials)
                {
                    mtl.SetInt("_Calculate_Dither", 0);
                }
            }
        }

        if (isoObjects != null && isoObjects.Length > 0)
        {
            foreach (GameObject gm in isoObjects)
            {
                if (gm.activeSelf)
                {
                    gm.SetActive(false);
                }
            }
        }
        if (tpsObjects != null && tpsObjects.Length > 0)
        {
            foreach (GameObject gm in tpsObjects)
            {
                if (gm.activeSelf)
                {
                    gm.SetActive(false);
                }
            }
        }
        if (fpsObjects != null && fpsObjects.Length > 0)
        {
            foreach (GameObject gm in fpsObjects)
            {
                if (gm.activeSelf)
                {
                    gm.SetActive(false);
                }
            }
        }
        if(customObjects != null && customObjects.Length > 0)
        {
            foreach (GameObject gm in customObjects)
            {
                if (!gm.activeSelf)
                {
                    gm.SetActive(true);
                }
            }
        }
        playerCollider = currentMovementType.GetPlayerCollider();
        coyoteTime = currentMovementType.GetCoyoteTime();
        resetCoyote = coyoteTime;
        customMovement.SetPlayerMovement(this);
        customMovement.SetSound_Particles(footstepSounds, jumpSound, footstepParticle, particlePos, timeBetweenSteps_Base);
        currentMovementType.SetUIInfo(tmp_Txt_1, tmp_Txt_2, txt_1, txt_2, img1, img2);
        hp.SetCameraFX(currentCameraMoveType.GetCameraFX());
        if (weaponController != null)
        {
            weaponController.ChangeShotPos(currentMovementType.GetShotPosition());
            weaponController.ChangeCanSpawnShotObj(currentMovementType.CanSpawnShotObject());
        }
    }

    #endregion
    #region Coroutines
    //Send the ground info to the movement for the Coyote time
    IEnumerator CoyoteTimeWait()
    {
        isOnGround = true;
        isJumping = true;
        currentMovementType.IsOnCoyote(true);
        currentMovementType.ReadGround(isOnGround);
        yield return new WaitForSeconds(coyoteTime);
        currentMovementType.IsOnCoyote(false);
        isOnGround = false;
        isJumping = true;
        currentMovementType.ReverseJumping(isJumping);
    }

    IEnumerator UnpauseSmooth()
    {
        float elapsed = 0;

        while(elapsed < 1)
        {
            elapsed += Time.unscaledDeltaTime;
            if(elapsed > 1)
            {
                elapsed = 1;
            }
            Time.timeScale = elapsed;
            yield return null;
        }
        isPaused = false;
    }
    IEnumerator WaitToDisablePause()
    {
        yield return new WaitForSeconds(0.1f);
        ui_Controller.SetActive(false);
    }
    #endregion
}
