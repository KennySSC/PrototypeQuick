using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class FpsMovement : BaseMovement
{
    #region Serialized Variables
    [Header("Main components")]

    [Tooltip("Player's camera")]
    [SerializeField] Transform cam;

    [Tooltip("Player's GameObject with CharacterController script")]
    [SerializeField] CharacterController charController;

    [Tooltip("Player's animator. If you are not using animation, you can leave it empty")]
    [SerializeField] Animator anim;


    [Space]


    [Header("Base movement settigs")]


    [Tooltip("Players walking speed in Unity units per second")]
    [SerializeField] float baseSpeed;

    [Tooltip("Players running speed in Unity units per second")]
    [SerializeField] float runSpeed;

    [Tooltip("Allows the player to change between walking and running in the air")]
    [SerializeField] bool canRun_WhileJumping;

    [Tooltip("Players crouch Speed in Unity units per second")]
    [SerializeField] float crouchSpeed;

    [SerializeField] CapsuleCollider playerCollider;

    [Tooltip("Put the center y of the player's collider. Value will also be applied to characterController")]
    [SerializeField] float colliderStand_Ypos;

    [Tooltip("Put the y size of the player's collider. Value will also be applied to characterController")]
    [SerializeField] float colliderStand_Size;

    [Tooltip("Put the center y of the collider of the player that you want while it's crouching. Value will also be applied to characterController")]
    [SerializeField] float colliderCrouch_Ypos;

    [Tooltip("Put the y size of the collider of the player that you want while it's crouching. Value will also be applied to characterController")]
    [SerializeField] float colliderCrouch_Size;

    [Tooltip("Player max jump height in Unity units")]
    [SerializeField] float jumpHeight;

    [Tooltip("Gravity for player in Unity units")]
    [SerializeField] float gravity;

    [Tooltip("Allows the player to jump in the air for a limited amount of time, great for platformers")]
    [SerializeField] float coyoteTime;


    [Space]


    [Header("Interact settings")]


    [SerializeField] float interactDistance;

    [SerializeField] float interactRadius;


    [Space]


    [Header("Shot settings")]


    [Tooltip("If you want to spawn an object that follows the same path of the shot, helping the player to aim, turn it on")]
    [SerializeField] bool spawnObject;


    [Space]


    [Header("Debug variables")]
    [SerializeField] bool isRunning = false;
    [SerializeField] bool isCrouching = false;
    [SerializeField] bool isJumping = false;
    [SerializeField] bool isGrounded = false;
    [SerializeField] bool isAiming = false;
    [SerializeField] bool canStandUp = false;
    [SerializeField] bool isOnCoyoteTime = false;
    [SerializeField] float tempSpeed = 0f;
    [SerializeField] Interactable currentInteractable;

    #endregion

    #region Private variables
    private PlayerMovement player;

    private GameObject[] footstepsSounds;
    private GameObject jumpSound;
    private GameObject stepParticle;

    private Transform particlePos;

    private float timeBetweenSteps_Base;
    private float currentMaxSpeed;

    private Vector3 velocity;
    private Vector2 speedControl;
    private bool hasGun = false;
    private float runMultiplier;
    private float crouchMultiplier;
    private float resetSteps;
    private bool calculateSteps = false;

    private Image img1;
    private Image img2;

    private TMP_Text tmp_Txt_1;
    private TMP_Text tmp_Txt_2;

    private Text txt_1;
    private Text txt_2;

    #endregion


    #region Main Functions
    private void Start()
    {
        ResetValues();
    }
    public override void ResetValues()
    {
        //Reset the variables
        isRunning = false;
        isCrouching = false;
        isJumping = false;
        canStandUp = false;
        isOnCoyoteTime = false;
        tempSpeed = 0f;
        currentMaxSpeed = baseSpeed;
        resetSteps = timeBetweenSteps_Base;
        runMultiplier = runSpeed / baseSpeed;
        crouchMultiplier = crouchSpeed / baseSpeed;

    }
    public override void Movement(Vector2 inputAxis)
    {
        speedControl = inputAxis;
        //Applies a gravity while it's grounded
        if (isGrounded && velocity.y < 0)
        {
            calculateSteps = true;
            velocity.y = -2f;
        }
        if (!isGrounded)
        {
            calculateSteps = false;
        }
        //Moves the player
        Vector3 moving = (((transform.right * inputAxis.x) + (transform.forward * inputAxis.y)) * currentMaxSpeed);
        charController.Move(moving * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        if (isOnCoyoteTime && velocity.y < 0)
        {
            velocity.y = 0f;
        }
        charController.Move(velocity * Time.deltaTime);

        //Calculate the spawn of the particle and sounds of the player steps
        
    }
    public override void StopMoving()
    {
        calculateSteps = false;
        tempSpeed = 0f;
    }
    public override void AnimationLogic()
    {
        if (anim != null)
        {
            //Applies animation
            if (speedControl == Vector2.zero)
            {
                calculateSteps = false;
                //Stand idle animations
                if (!isCrouching)
                {
                    anim.SetInteger("AnimVal", 0);
                    if (hasGun)
                    {
                        if (isAiming)
                        {
                            anim.SetInteger("Gun_AnimVal", 6);
                        }
                        else
                        {
                            anim.SetInteger("Gun_AnimVal", 1);
                        }
                    }
                }
                //Crouch idle animations
                else
                {
                    anim.SetInteger("AnimVal", 3);
                    if (hasGun)
                    {
                        if (isAiming)
                        {
                            anim.SetInteger("Gun_AnimVal", 7);
                        }
                        else
                        {
                            anim.SetInteger("Gun_AnimVal", 4);
                        }

                    }
                }

            }
            else
            {
                //Running animations
                if (isRunning && !isJumping)
                {
                    anim.SetInteger("AnimVal", 2);
                    if (hasGun)
                    {
                        anim.SetInteger("Gun_AnimVal", 3);
                    }
                }
                //Moving while crouching animations
                else if (isCrouching)
                {
                    anim.SetInteger("AnimVal", 4);
                    if (hasGun)
                    {
                        if (isAiming)
                        {
                            anim.SetInteger("Gun_AnimVal", 7);
                        }
                        else
                        {
                            anim.SetInteger("Gun_AnimVal", 5);
                        }

                    }
                }
                //Stand moving animations
                else if (!isJumping)
                {
                    calculateSteps = true;
                    anim.SetInteger("AnimVal", 1);
                    if (hasGun)
                    {
                        if (isAiming)
                        {
                            anim.SetInteger("Gun_AnimVal", 6);
                        }
                        else
                        {
                            anim.SetInteger("Gun_AnimVal", 2);
                        }

                    }
                }
            }
            //Jumping animations
            if (isJumping && isGrounded)
            {
                isJumping = false;
                anim.SetBool("FinishJump", true);
                StartCoroutine(WaitToJump());

            }
        }
        else
        {
            if(speedControl == Vector2.zero)
            {
                calculateSteps = false;
            }
            else if (!isJumping)
            {
                calculateSteps = true;
            }
            if(isJumping && isGrounded)
            {
                isJumping = false;
            }
        }
        //Calculate step particles and sound
        if (calculateSteps)
        {
            if (timeBetweenSteps_Base > 0)
            {
                float tempRest = Time.deltaTime;
                if (isRunning)
                {
                    tempRest *= runMultiplier;
                }
                if (isCrouching)
                {
                    tempRest *= crouchMultiplier;
                }
                timeBetweenSteps_Base -= tempRest;

                if (timeBetweenSteps_Base <= 0)
                {
                    if (stepParticle != null)
                    {
                        Instantiate(stepParticle, particlePos.position, particlePos.rotation);
                    }
                    if (footstepsSounds != null && footstepsSounds.Length>0)
                    {
                        int random = Random.Range(0, footstepsSounds.Length);

                        Instantiate(footstepsSounds[random], particlePos.position, particlePos.rotation);
                    }
                    timeBetweenSteps_Base = resetSteps;
                }
            }
        }
    }
    public override void Jump()
    {
        if (isGrounded && !canStandUp)
        {
            if (isCrouching)
            {
                Crouch();
            }
            if(anim!= null)
            {
                anim.SetBool("FinishJump", false);
                anim.Play("JumpStart");
            }
            isJumping = true;
            isOnCoyoteTime = false;
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if(jumpSound != null)
            {
                Instantiate(jumpSound, particlePos.position, particlePos.rotation);
            }
        }
    }
    public override bool IsJumping()
    {
        return isJumping;
    }
    public override void Crouch()
    {
        if (!isJumping)
        {
            if (isCrouching && !canStandUp)
            {
                currentMaxSpeed = baseSpeed;
                playerCollider.height = colliderStand_Size;
                charController.height = colliderStand_Size;
                playerCollider.center = new Vector3(playerCollider.center.x, colliderStand_Ypos, playerCollider.center.z);
                charController.center = new Vector3(charController.center.x, colliderStand_Ypos, charController.center.z);
                isCrouching = false;
            }
            else
            {
                currentMaxSpeed = crouchSpeed;
                playerCollider.height = colliderCrouch_Size;
                charController.height = colliderCrouch_Size;
                playerCollider.center = new Vector3(playerCollider.center.x, colliderCrouch_Ypos, playerCollider.center.z);
                charController.center = new Vector3(charController.center.x, colliderCrouch_Ypos, charController.center.z);
                isCrouching = true;
                isRunning = false;
            }
        }
    }
    public override void Run()
    {
        if (!isRunning)
        {
            if (!canRun_WhileJumping && isJumping)
            {
                return;
            }
            else if (!isCrouching)
            {
                currentMaxSpeed = runSpeed;
                isRunning = true;
            }
            else if (!canStandUp && isCrouching)
            {
                Crouch();
                currentMaxSpeed = runSpeed;
                isRunning = true;
            }
        }
        else
        {
            currentMaxSpeed = baseSpeed;
            isRunning = false;
        }

    }
    //Stops or starts player running externally
    public override void ChangeRun(bool newValue)
    {
        isRunning = newValue;
    }
    //Stops or starts player aiming externally
    public override void ChangeAiming(bool newValue)
    {
        isAiming = newValue;
    }
    //Call interactions
    public override void Interact_1()
    {
        if (currentInteractable != null)
        {
            StartCoroutine(TryToEmpty_HUD());
            currentInteractable.Use_1(gameObject);
            if(anim!= null)
            {
                anim.Play("Interact", 2);
            }

        }
    }
    public override void Interact_2()
    {
        if (currentInteractable != null)
        {
            StartCoroutine(TryToEmpty_HUD());
            currentInteractable.Use_2(gameObject);
            if (anim!= null)
            {
                anim.Play("Interact", 2);
            }
        }
    }
    public override void SearchInteractable()
    {
        //Searches an interactable with a spherecast
        RaycastHit hit;
        if (Physics.SphereCast(cam.position, interactRadius, cam.forward, out hit, interactDistance))
        {
            if (hit.collider.tag == "Interactable" && hit.collider.gameObject.GetComponent<Interactable>() != null)
            {
                if(currentInteractable != null && hit.collider.gameObject.GetComponent<Interactable>() != currentInteractable)
                {
                    currentInteractable.Miss(gameObject);
                }
                currentInteractable = hit.collider.gameObject.GetComponent<Interactable>();
                currentInteractable.Found(gameObject);
            }
            else
            {
                if (currentInteractable != null)
                {
                    currentInteractable.Miss(gameObject);
                    currentInteractable = null;
                }
            }
        }
        else if(!Physics.SphereCast(cam.position, interactRadius, cam.forward, out hit, interactDistance))
        {

        }
    }
    public override void TryToStandUp(bool willStand)
    {
        canStandUp = willStand;
    }
    public override void ReadGround(bool grounded)
    {
        isGrounded = grounded;
    }
    public override void ReverseJumping(bool jump)
    {
        isJumping = jump;
        if (isJumping && isGrounded)
        {
            if(anim!= null)
            {
                anim.Play("JumpIdle");
            }
            StartCoroutine(WaitToJump());
        }
    }
    public override void RotateCharacter()
    {
        
    }

    #endregion

    #region Get Set
    public override float GetCoyoteTime()
    {
        return coyoteTime;
    }
    public override void IsOnCoyote(bool isCoyote)
    {
        isOnCoyoteTime = isCoyote;
    }
    public override Transform GetShotPosition()
    {
        return cam;
    }
    public Transform ShootPosition()
    {
        return cam;
    }
    public override void ChangeHasGun(bool change)
    {
        hasGun = change;
        if (anim != null)
        {
            if (hasGun)
            {
                anim.SetInteger("Gun_AnimVal", 1);
            }
            else
            {
                anim.SetInteger("Gun_AnimVal", 0);
            }
        }

    }
    public override bool CanSpawnShotObject()
    {
        return spawnObject;
    }
    public override Animator GetPlayerAnimator()
    {
        return anim;
    }
    public override Collider GetPlayerCollider()
    {
        return playerCollider;
    }
    public override void SetPlayerMovement(PlayerMovement newPlayerMovement)
    {
        player = newPlayerMovement;
    }
    //Set sound & particles for this movement
    public override void SetSound_Particles(GameObject[] stepSound, GameObject jumpSnd, GameObject footstepParticle, Transform particlePosition, float stepsTime)
    {
        footstepsSounds = stepSound;
        jumpSound = jumpSnd;
        stepParticle = footstepParticle;
        particlePos = particlePosition;
        timeBetweenSteps_Base = stepsTime;
    }
    //Sets UI info for interactables
    public override void SetUIInfo(TMP_Text tmpTxt_1, TMP_Text tmpTxt_2, Text newtxt_1, Text newtxt_2, Image newimg1, Image newimg2)
    {
        tmp_Txt_1 = tmpTxt_1;
        tmp_Txt_2 = tmpTxt_2;

        txt_1 = newtxt_1;
        txt_2 = newtxt_2;

        img1 = newimg1;
        img2 = newimg2;
    }
    //Changes shown texts & images when found interactable
    public override void SetSpritesInfo(Sprite sprt1, Sprite sprt2)
    {
        if(sprt1!= null)
        {
            if (!img1.gameObject.activeSelf)
            {
                img1.gameObject.SetActive(true);
            }
            img1.sprite = sprt1;
        }
        else
        {
            if (img1.gameObject.activeSelf)
            {
                img1.gameObject.SetActive(false);
            }
        }
        if (sprt2 != null)
        {
            if (!img2.gameObject.activeSelf)
            {
                img2.gameObject.SetActive(true);
            }
            img2.sprite = sprt2;
        }
        else
        {

            if (img2.gameObject.activeSelf)
            {
                img2.gameObject.SetActive(false);
            }
        }
    }
    public override void SetExtraInfo(string info1, string info2)
    {
        if(tmp_Txt_1 != null)
        {
            tmp_Txt_1.text = info1;
        }else if(txt_1 != null)
        {
            txt_1.text = info1;
        }
        if (tmp_Txt_2 != null)
        {
            tmp_Txt_2.text = info2;
        }
        else if (txt_2 != null)
        {
            txt_2.text = info2;
        }
    }
    #endregion

    #region Coroutines
    IEnumerator WaitToJump()
    {
        yield return new WaitForSeconds(0.3f);
        if(anim!= null)
        {
            anim.SetBool("FinishJump", false);
        }
    }
    IEnumerator TryToEmpty_HUD()
    {
        yield return new WaitForSeconds(0.1f);
        SetExtraInfo("", "");
        SetSpritesInfo(null, null);
    }
    #endregion 
}
