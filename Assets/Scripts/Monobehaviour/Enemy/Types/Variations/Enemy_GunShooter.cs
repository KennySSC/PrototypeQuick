using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_GunShooter : Enemy_Base
{
    #region Serializable variables
    [Header("Requiered components")]
    [SerializeField] NavMeshAgent agent;

    [Tooltip("If you aren't using animations you can leave it empty")]
    [SerializeField] Animator enemyAnimator;

    [Tooltip("Gameobjects where the AI will move while isn't chasing the player")]
    [SerializeField] List<Transform> patrolPoints = new List<Transform>();

    [Tooltip("Object where the AI will detect that is touching ground")]
    [SerializeField] Transform groundPosition;

    [SerializeField] LayerMask groundMask;


    [Space]


    [Header("Look and detection settings")]


    [Tooltip("AI eyes, forward direction of this object is where the AI will detect the player")]
    [SerializeField] Transform lookPosition;

    [Tooltip("Layermask that allows the enemy to detect some objects. Ground & Player are recommended. If you select the same layer as any object inside the enemy it will detect itself and " +
        "it will not detect the player properly. You can select more than one")]
    [SerializeField] LayerMask enemyDetectionMask;

    [Tooltip("An object that will look to the player on x rotation, allowing the AI attacking the player if the player isn't in the same level, but it's visible")]
    [SerializeField] Transform playerYLook;

    [Tooltip("Max looking distance")]
    [SerializeField] float lookingDistance;

    [Tooltip("Max looking radius")]
    [SerializeField] float lookingRadius;

    [Tooltip("The time after stop seeing the player that AI still knowns where the player is. Useful in contexts where there is an object blocking AI's sight, preventing that it to stop moving, " +
    "so the AI feel less dumb")]
    [SerializeField] float waitToLosePlayer;

    [Tooltip("The time that takes the AI to return to it's patrol state after losing the player")]
    [SerializeField] float waitPatrol;

    [Tooltip("If on, always chases the player, no matter where it is. When off, uses the patrol behaviour")]
    [SerializeField] bool alwaysKnow_WherePlayerIs;

    [Tooltip("Transform that will have the same position of the player, when the player isn't in sight, this will be used to try to found player a little while. Requiered")]
    [SerializeField] Transform lastPlayerPosition;


    [Space]


    [Header("Speed settings")]
    [SerializeField] float patrolSpeed;

    [SerializeField] float chasingSpeed;


    [Space]


    [Header("Attack settings")]


    [Tooltip("The distance from the player to start attacking it")]
    [SerializeField] float startAttackDistance;

    [Tooltip("Delay between attacks")]
    [SerializeField] float attackDelay;


    [Space]


    [Header("Sound & Particles settings")]

    [Tooltip("It will perform when the enemy detects the player. If you don't want to use it, leave it empty")]
    [SerializeField] GameObject detectPlayerSound;

    [Tooltip("It will perform when the enemy returns to patrol state. If you don't want to use it, leave it empty")]
    [SerializeField] GameObject returnPatrolSound;

    [Tooltip("It will perform randomly while the enemy exists. If you don't want to use it, leave it empty")]
    [SerializeField] GameObject[] randomSounds;

    [Tooltip("Time that must pass before a new random sound performs")]
    [SerializeField] float timeBetweenRandom;

    [Tooltip("Spawn position for the particles and sound")]
    [SerializeField] Transform footstepParticlePosition;

    [Tooltip("It will appear while the enemy is moving. If you don't want to use it, leave it empty")]
    [SerializeField] GameObject footstepParticle;

    [Tooltip("It will perform while the enemy is moving. If you don't want to use it, leave it empty")]
    [SerializeField] GameObject[] footstepSounds;

    [Tooltip("Wait time to instantiate another particle and sound")]
    [SerializeField] float timeBetweenSteps;

    #endregion

    #region Private variables

    Transform playerPosition;
    private EnemyController controller;

    private bool calculateSteps = false;
    private bool hasGun = false;
    [SerializeField]private bool isFollowPlayer = false;
    private bool isAlive = true;
    private bool isJumping = false;
    private bool isGrounded;
    private bool canPerform_DetectPlayerSound = true;
    private bool canRandomSound = true;

    private int currentPatrolPoint_Index = 0;

    private float resetWaitPatrol;
    private float resetAttackDelay;
    private float resetLosePlayer;
    private float resetTimeBetweenSteps;
    private float resetRandomTime;

    #endregion
    #region Main Functions
    private void Start()
    {
        //Set start values
        resetWaitPatrol = waitPatrol;
        resetAttackDelay = attackDelay;
        resetLosePlayer = waitToLosePlayer;
        resetTimeBetweenSteps = timeBetweenSteps;
        resetRandomTime = timeBetweenRandom;
        agent.speed = patrolSpeed;
        attackDelay = 0;
        //Try to find the controller
        if (GetComponent<EnemyController>() != null)
        {
            controller = GetComponent<EnemyController>();
            hasGun = controller.SendHasGun();
        }
        if (alwaysKnow_WherePlayerIs)
        {
            Set_AlwaysKnow_WherePlayerIs();
        }
    }
    public override void Movement()
    {
        //Calculate the movement only if it's alive
        if (isAlive)
        {
            //If has looked the player, the AI will chase it
            if (isFollowPlayer)
            {
                if (playerPosition != null)
                {
                    //If reach the maximum attack distance, try to attack
                    if ((playerPosition.position - transform.position).sqrMagnitude <= (startAttackDistance * startAttackDistance))
                    {
                        //Look to the player
                        Vector3 lookPlayer = new Vector3(playerYLook.position.x, playerPosition.position.y, playerPosition.position.z);
                        playerYLook.LookAt(lookPlayer);
                        Vector3 lookPos = playerPosition.position - transform.position;
                        lookPos.y = 0;
                        Quaternion rotation = Quaternion.LookRotation(lookPos);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, (2*Time.deltaTime));
                        //Stops the AI if is to close to the player, to prevent that the attack doesn't hit the player and that tha AI doesn't push the player away of the map
                        if ((playerPosition.position - transform.position).sqrMagnitude <= (agent.stoppingDistance * agent.stoppingDistance))
                        {
                            agent.isStopped = true;
                            agent.updateRotation = true;
                        }
                        attackDelay -= Time.deltaTime;
                        if (attackDelay <= 0 && controller != null)
                        {
                            Debug.Log("try to shoot");
                            controller.OnShoot();
                            attackDelay = resetAttackDelay;
                        }
                    }
                    else
                    {
                        //Still follow the player
                        agent.isStopped = false;
                        MoveToLocation(playerPosition.position);
                        lastPlayerPosition.position = playerPosition.position;
                    }
                }
                else
                {
                    //See forward and go to the player's last known position
                    lookPosition.localEulerAngles = new Vector3(0, 0, 0);
                    waitPatrol -= Time.deltaTime;
                    if (waitPatrol <= 0)
                    {
                        //Return to patrol state
                        waitPatrol = resetWaitPatrol;
                        lastPlayerPosition.position = transform.position;
                        if(returnPatrolSound!= null)
                        {
                            Instantiate(returnPatrolSound, transform.position, transform.rotation);
                        }
                        isFollowPlayer = false;
                    }
                    else
                    {
                        MoveToLocation(lastPlayerPosition.position);
                    }
                }
            }
            else
            {
                //Patrol state
                MoveToLocation(patrolPoints[currentPatrolPoint_Index].position);
                if (patrolPoints.Count >0 && patrolPoints != null)
                {
                    //Reach the patrol point and go to the next one
                    if ((patrolPoints[currentPatrolPoint_Index].position - transform.position).sqrMagnitude <= (agent.stoppingDistance * agent.stoppingDistance))
                    {
                        currentPatrolPoint_Index++;
                        if (currentPatrolPoint_Index > patrolPoints.Count - 1)
                        {
                            currentPatrolPoint_Index = 0;
                        }
                    }
                }
            }
            //Detect ground
            if(Physics.CheckSphere(groundPosition.position, 0.2f, groundMask))
            {
                isGrounded = true;
            }
            else
            {
                //Is on air
                isGrounded = false;
                if (!isJumping && enemyAnimator != null)
                {
                    enemyAnimator.Play("JumpStart");
                }
                isJumping = true;
            }
        }

    }
    public override void Look()
    {
        //Sees only if it's alive
        if (isAlive)
        {
            //Look forward
            RaycastHit hit;
            if (Physics.SphereCast(lookPosition.position, lookingRadius, lookPosition.forward, out hit, lookingDistance, enemyDetectionMask))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    //See the player and chase it
                    agent.speed = chasingSpeed;
                    if (canPerform_DetectPlayerSound && detectPlayerSound != null && !isFollowPlayer)
                    {
                        Instantiate(detectPlayerSound, transform.position, transform.rotation);
                        canPerform_DetectPlayerSound = false;
                        StartCoroutine(Reset_DetectSound());
                    }
                    SetPlayer(hit.collider.gameObject.transform);
                    isFollowPlayer = true;

                }
                else
                {
                    //Stop seeing the player
                    if (playerPosition != null)
                    {
                        waitToLosePlayer -= Time.deltaTime;
                        if (waitToLosePlayer <= 0)
                        {
                            agent.speed = patrolSpeed;
                            waitToLosePlayer = resetLosePlayer;
                            SetPlayer(null);
                        }
                    }
                }
            }
            else if (!Physics.SphereCast(lookPosition.position, lookingRadius, lookPosition.forward, out hit, lookingDistance, enemyDetectionMask))
            {
                //Stop seeing the player (try to prevent a fake detection or continuous detection after stop seeing the player)
                if (playerPosition != null)
                {
                    waitToLosePlayer -= Time.deltaTime;
                    if (waitToLosePlayer <= 0)
                    {
                        agent.speed = patrolSpeed;
                        waitToLosePlayer = resetLosePlayer;
                        SetPlayer(null);
                    }
                }
            }
        }
    }
    public override void AnimationLogic()
    {
        if(enemyAnimator != null && isAlive)
        {
            //idle animation
            if (agent.velocity == Vector3.zero)
            {
                calculateSteps = false;
                enemyAnimator.SetInteger("AnimVal", 0);
                if (hasGun)
                {
                    enemyAnimator.SetInteger("Gun_AnimVal", 1);
                }
            }
            else
            {
                //Chase animations
                if (isFollowPlayer && agent.velocity.magnitude > patrolSpeed)
                {
                    enemyAnimator.SetInteger("AnimVal", 2);
                    if (hasGun)
                    {
                        enemyAnimator.SetInteger("Gun_AnimVal", 3);
                    }
                }
                //Patrol animations
                else
                {
                    enemyAnimator.SetInteger("AnimVal", 1);
                    if (hasGun)
                    {
                        enemyAnimator.SetInteger("Gun_AnimVal", 2);
                    }
                }
                if (!isJumping)
                {
                    calculateSteps = true;
                }

            }
            //Jumping animations
            if (isJumping && isGrounded)
            {
                isJumping = false;
                enemyAnimator.SetBool("FinishJump", true);
                StartCoroutine(WaitToJump());

            }

        }
        else if (isAlive && enemyAnimator == null)
        {
            if(agent.velocity == Vector3.zero)
            {
                calculateSteps = false;
            }
            else
            {
                if (!isJumping)
                {
                    calculateSteps = true;
                }
            }
            //Jumping animations
            if (isJumping && isGrounded)
            {
                isJumping = false;
            }
        }
        //Calculate step particles and sound
        if (calculateSteps)
        {
            if (timeBetweenSteps > 0)
            {

                timeBetweenSteps -= Time.deltaTime;

                if (timeBetweenSteps <= 0)
                {
                    if (footstepParticle != null)
                    {
                        Instantiate(footstepParticle, footstepParticlePosition.position, footstepParticlePosition.rotation);
                    }
                    if (footstepSounds != null && footstepSounds.Length > 0)
                    {
                        int random = Random.Range(0, footstepSounds.Length);
                        Instantiate(footstepSounds[random], footstepParticlePosition.position, footstepParticlePosition.rotation);
                    }
                    timeBetweenSteps = resetTimeBetweenSteps;
                }
            }
        }
        if (canRandomSound && randomSounds != null && randomSounds.Length > 0)
        {
            if (timeBetweenRandom > 0)
            {
                timeBetweenRandom -= Time.deltaTime;
                if (timeBetweenRandom <= 0)
                {
                    int random = Random.Range(0, randomSounds.Length);
                    Instantiate(randomSounds[random], transform.position, transform.rotation);
                    timeBetweenRandom = resetRandomTime;
                }
            }
        }

    }
    public override bool MoveToLocation(Vector3 target)
    {
        if (agent.SetDestination(target))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion
    #region Get Set
    public override void SetPlayer(Transform player)
    {
        playerPosition = player;
    }
    public override void ChangeIsAlive(bool imAlive)
    {
        isAlive = imAlive;
    }
    public override void ChangeHasGun(bool has_Gun)
    {
        hasGun = has_Gun;
    }
    public override Animator GetAnimator()
    {
        return enemyAnimator;
    }
    public override void Set_Waypoints(List<Transform> newWaypoints)
    {
        foreach(Transform tn in newWaypoints)
        {
            if (!patrolPoints.Contains(tn))
            {
                patrolPoints.Add(tn);
            }
        }
    }
    public override void Set_AlwaysKnow_WherePlayerIs()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        waitToLosePlayer = 999999f;
        resetLosePlayer = waitToLosePlayer;
        isFollowPlayer = true;
        alwaysKnow_WherePlayerIs = true;
        agent.speed = chasingSpeed;
        if (canPerform_DetectPlayerSound && detectPlayerSound != null && !isFollowPlayer)
        {
            Instantiate(detectPlayerSound, transform.position, transform.rotation);
            canPerform_DetectPlayerSound = false;
            StartCoroutine(Reset_DetectSound());
        }
        SetPlayer(playerPosition);
    }
    public override void TryToGoToPlayer()
    {
        GameObject tempPlayer = GameObject.FindGameObjectWithTag("Player");
        if (tempPlayer != null)
        {
            SetPlayer(tempPlayer.transform);
            isFollowPlayer = true;
        }
    }
    #endregion
    #region Coroutines
    private IEnumerator WaitToJump()
    {
        yield return new WaitForSeconds(0.5f);
        if(enemyAnimator != null)
        {
            enemyAnimator.SetBool("FinishJump", false);
        }
    }
    private IEnumerator Reset_DetectSound()
    {
        canRandomSound = false;
        yield return new WaitForSeconds(1);
        canPerform_DetectPlayerSound = true;
        canRandomSound = true;
    }
    #endregion
}
