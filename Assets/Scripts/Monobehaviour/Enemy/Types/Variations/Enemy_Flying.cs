using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Flying : Enemy_Base
{
    #region Serializable variables
    [Header("Requiered components")]
    [SerializeField] NavMeshAgent agent;

    [SerializeField] Transform agentTransform;

    [SerializeField] EnemyController controller;

    [SerializeField] Animator enemyAnimator;

    [Tooltip("Gameobjects where the AI will move while isn't chasing the player")]
    [SerializeField] List<Transform> patrolPoints = new List<Transform>();

    [Tooltip("Turret pivot transform")]
    [SerializeField] Transform turret;

    [Tooltip("AI eyes, forward direction of this object is where the AI will detect the player")]
    [SerializeField] Transform lookPosition;

    [Tooltip("An object that will the player on x rotation, allowing the AI attacking the player if the player isn't in the same level, but it's visible")]
    [SerializeField] Transform playerYLook;

    [Tooltip("Layer where the player is, should be diferent from this gameobject to work correctly")]
    [SerializeField] LayerMask playerLayer;

    [Header("Main settings")]
    [SerializeField] float patrolSpeed;

    [SerializeField] float chasingSpeed;

    [Tooltip("Max looking distance")]
    [SerializeField] float lookingDistance;

    [Tooltip("Max looking radius")]
    [SerializeField] float lookingRadius;

    [Tooltip("The time after stop seeing the player that AI still knowns where the player is. Useful in contexts where there is an object blocking AI's sight, preventing that it to stop moving, " +
        "so the AI feel less dumb")]
    [SerializeField] float waitToLosePlayer;

    [Tooltip("The distance from the player to start attacking it")]
    [SerializeField] float startAttackDistance;

    [Tooltip("Delay between attacks")]
    [SerializeField] float attackDelay;

    [SerializeField] int damage;

    [Tooltip("The time that takes the AI to return to it's patrol state after losing the player")]
    [SerializeField] float waitPatrol;

    [Tooltip("If on, always chases the player, no matter where it is. When off, uses the patrol behaviour")]
    [SerializeField] bool alwaysKnow_WherePlayerIs;


    [Header("Sound & Particles settings")]

    [Tooltip("It will perform when it detects the player. If you don't want to use it, leave it empty")]
    [SerializeField] GameObject detectPlayerSound;

    [Tooltip("It will perform when returning to patrol state. If you don't want to use it, leave it empty")]
    [SerializeField] GameObject returnPatrolSound;

    [Tooltip("Child of the object, gameobject of the particle")]
    [SerializeField] GameObject fireParticle;

    [Tooltip("It will perform while shooting. If you don't want to use it, leave it empty")]
    [SerializeField] GameObject shotSoundPrefab;

    [Tooltip("It will appear while shooting. If you don't want to use it, leave it empty")]
    [SerializeField] GameObject shotParticlePrefab;


    [Header("Player position settings")]


    [Tooltip("Transform that will have the same position of the player, requiered")]
    [SerializeField] Transform lastPlayerPosition;


    #endregion
    #region Private variables
    private Transform shotPosition;
    private Transform playerPosition;

    private bool isFollowPlayer = false;
    private bool isAlive = true;
    private bool canPerform_DetectPlayerSound = true;

    private int currentPatrolPoint_Index = 0;

    private float resetWaitPatrol;
    private float resetAttackDelay;
    private float resetLosePlayer;
    private float lookPosXRotation_Reset;
    #endregion
    #region Main Functions
    private void Start()
    {
        //Set start values
        resetWaitPatrol = waitPatrol;
        resetAttackDelay = attackDelay;
        resetLosePlayer = waitToLosePlayer;
        agent.speed = patrolSpeed;
        shotPosition = controller.GetShotPosition();
        lookPosXRotation_Reset = lookPosition.eulerAngles.x;
        attackDelay = 0;
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
                    if ((playerPosition.position - transform.position).sqrMagnitude + transform.localPosition.y <= (startAttackDistance * startAttackDistance))
                    {
                        turret.LookAt(playerPosition);
                       // turret.localEulerAngles = new Vector3(Mathf.Clamp(turret.localEulerAngles.x, -90,25), Mathf.Clamp(turret.localEulerAngles.y, -90, 90), turret.localEulerAngles.z);
                        //Look to the player
                        Vector3 lookPlayer = new Vector3(playerYLook.position.x, playerPosition.position.y, playerPosition.position.z);
                        playerYLook.LookAt(lookPlayer);
                        Vector3 lookPos = playerPosition.position - transform.position;
                        lookPos.y = 0;
                        Quaternion rotation = Quaternion.LookRotation(lookPos);
                        agentTransform.rotation = Quaternion.Slerp(agentTransform.rotation, rotation, (2 * Time.deltaTime));
                        //Stops the AI if is to close to the player, to prevent that the attack doesn't hit the player and that tha AI doesn't push the player away of the map
                        if ((playerPosition.position - transform.position).sqrMagnitude - transform.localPosition.y - 0.3f <= (agent.stoppingDistance * agent.stoppingDistance) + transform.localPosition.y - 0.5f)
                        {
                            agent.isStopped = true;
                            agent.updateRotation = true;
                        }
                        attackDelay -= Time.deltaTime;
                        if (attackDelay <= 0 && controller != null)
                        {
                            Debug.Log("try to shoot");
                            if(shotParticlePrefab != null)
                            {
                                GameObject particle = Instantiate(shotParticlePrefab, shotPosition.position, shotPosition.rotation);
                                particle.transform.parent = shotPosition;
                            }
                            if(shotSoundPrefab != null)
                            {
                                Instantiate(shotSoundPrefab, shotPosition.position, shotPosition.rotation);
                            }
                            RaycastHit hit;
                            if(Physics.Raycast(shotPosition.position, shotPosition.forward, out hit,startAttackDistance + 0.3f))
                            {
                                if(hit.collider.gameObject.tag == "Player" && hit.collider != GetComponent<Collider>())
                                {
                                    hit.collider.gameObject.GetComponent<Health>().GetDamage(damage);
                                }
                            }
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
                    lookPosition.localEulerAngles = new Vector3(lookPosXRotation_Reset, 180, 0);
                    waitPatrol -= Time.deltaTime;
                    if (waitPatrol <= 0)
                    {
                        //Return to patrol state
                        waitPatrol = resetWaitPatrol;
                        lastPlayerPosition.position = transform.position;
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
                if (patrolPoints.Count > 0 && patrolPoints != null)
                {
                    //Reach the patrol point and go to the next one
                    if ((patrolPoints[currentPatrolPoint_Index].position - transform.position).sqrMagnitude - transform.localPosition.y -0.3f<= (agent.stoppingDistance * agent.stoppingDistance)+transform.localPosition.y-0.5f)
                    {
                        currentPatrolPoint_Index++;
                        if (currentPatrolPoint_Index > patrolPoints.Count - 1)
                        {
                            currentPatrolPoint_Index = 0;
                        }
                    }
                }
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
            if (Physics.SphereCast(lookPosition.position, lookingRadius, lookPosition.forward, out hit, lookingDistance, playerLayer))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    //See the player and chase it
                    agent.speed = chasingSpeed;
                    if(canPerform_DetectPlayerSound  && detectPlayerSound!= null)
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
                            if(returnPatrolSound != null)
                            {
                                Instantiate(returnPatrolSound, transform.position, transform.rotation);
                            }
                            waitToLosePlayer = resetLosePlayer;
                            SetPlayer(null);
                        }
                    }
                }
            }
        }
    }
    public override void AnimationLogic()
    {
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
        if (!isAlive)
        {
            if(fireParticle!= null && fireParticle.activeSelf)
            {
                fireParticle.SetActive(false);
            }
        }
    }
    public override void ChangeHasGun(bool has_Gun)
    {
       
    }
    public override Animator GetAnimator()
    {
        return enemyAnimator;
    }
    public override void Set_Waypoints(List<Transform> newWaypoints)
    {
        foreach (Transform tn in newWaypoints)
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

    private IEnumerator Reset_DetectSound()
    {
        yield return new WaitForSeconds(1);
        canPerform_DetectPlayerSound = true;
    }

    #endregion
}
