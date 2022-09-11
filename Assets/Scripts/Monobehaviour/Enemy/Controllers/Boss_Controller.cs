using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss_Controller : MonoBehaviour
{
    #region Serializable variables
    [Header("Requiered components")]


    [SerializeField] NavMeshAgent bossAgent;

    [Tooltip("Game object that will be activated when the boss activates")]
    [SerializeField] GameObject canvas_Hp;


    [Space]


    [Header("Shot settings")]

    [Tooltip("Where the bullets will spawn")]
    [SerializeField] Transform bulletSpawn;

    [Tooltip("The projectile that the boss will shoot")]
    [SerializeField] GameObject bulletPrefab;

    [Tooltip("Particle that spawns each time that the boos shoots")]
    [SerializeField] GameObject muzzleflashPrefab;

    [Tooltip("Ammount of bullets that the boss shoot at once")]
    [SerializeField] int bulletsCount;

    [Tooltip("Ammount of empty spaces that replaces shot positions, for the player to dodge the bullets")]
    [SerializeField] int emptyBulletsCount;

    [SerializeField] float bulletSpeed;

    [SerializeField] float bulletRadius;

    [SerializeField] int bulletsDamage;

    [Tooltip("The time that need the boss to shot a new burst of bullets")]
    [SerializeField] float shotsDelay;

    [Tooltip("Time for the boss to aim to the player before start shooting after reaching a shot position")]
    [SerializeField] float shotsWaitToStart;

    [Tooltip("Ammount of burst of bullets that must be shooted to return to the chase phase")]
    [SerializeField] int bulletBurstsCount;

    [Tooltip("Positions where the boss will go randomlly when changes to shot phase")]
    [SerializeField] Transform[] movePositions;

    [Tooltip("The speed of the boss while going to a shot position")]
    [SerializeField] float changePosSpeed;


    [Space]


    [Header("Chase settings")]

    [Tooltip("The speed of the boss while chasing the player")]
    [SerializeField] float chaseSpeed;

    [Tooltip("Radius of the melee attack")]
    [SerializeField] float meleeRadius;

    [Tooltip("Range of the melee attack")]
    [SerializeField] float meleeRange;

    [Tooltip("The distance from the player to start melee attacking")]
    [SerializeField] float meleeStartDistance;

    [Tooltip("Time between melee attacks")]
    [SerializeField] float meleeDelay;

    [Tooltip("Time for the player to dodge the melee attack")]
    [SerializeField] float meleeWaitToDamage;

    [SerializeField] int meleeDamage;

    [Tooltip("The position where the melee attack will be performed")]
    [SerializeField] Transform meleePosition;

    [Tooltip("Ammount of damage that the boss must receive to change to shooting")]
    [SerializeField] int healthToShot;


    [Space]


    [Header("Animation settings")]

    [Tooltip("If you aren't using animation leave it empty")]
    [SerializeField] Animator anim;

    [Tooltip("Time that takes the boss to activate and start receiving and dealing damage. If you aren't using animation set 0")]
    [SerializeField] float startAnimationTime;


    [Space]


    [Header("Sound & Particles")]

    [Tooltip("It will perform when te boss is activating. If you don't want to use it, leave it empty")]
    [SerializeField] GameObject activateSound;

    [Tooltip("It will perform each time the boss changes to the shooting face. If you don't want to use it, leave it empty")]
    [SerializeField] GameObject changeToShootingSound;

    [Tooltip("It will perform each time the boss shoots. If you don't want to use it, leave it empty")]
    [SerializeField] GameObject shotSound;

    [Tooltip("It will perform each time the boss use a melee attack. If you don't want to use it, leave it empty")]
    [SerializeField] GameObject meleeAttackSound;

    [Tooltip("It will perform randomly during the chase phase. If you don't want to use it, leave it empty")]
    [SerializeField] GameObject[] randomSounds;

    [Tooltip("Time that must pass before a random sound performs")]
    [SerializeField] float timeBetweenRandomSounds;

    [Tooltip("Position where the footstep sound & particle will perform")]
    [SerializeField] Transform footstepPosition;

    [Tooltip("It will appear while the boss is walking or running. If you don't want to use it, leave it empty")]
    [SerializeField] GameObject footstepParticle;

    [Tooltip("It will perform while the boss is walking or running. If you don't want to use it, leave it empty")]
    [SerializeField] GameObject[] footStepSounds;

    [Tooltip("Time that must pass before a footstep sound performs")]
    [SerializeField] float timeBetweenFootsteps;

    [Space]


    [Header("Debug variables")]
    [Tooltip("Where the boss is going")]
    [SerializeField] private Transform target;

    #endregion


    #region private variables
    private Transform player;
    private BossHealthManager hpManager;
    private List<int> emptyBulletIndex = new List<int>();

    private float resetStepsTime;
    private float resetRandomTime;
    private float resetMeleeDelay;

    private int resetBursts;
    private int cntTemp = 0;

    private bool activated = false;
    private bool isDead = false;
    private bool isShooting = false;
    private bool doingShot = false;
    private bool isPositioning = false;
    private bool calculateSteps = false;
    private bool calculateRandom = false;

    #endregion

    #region Main functions

    void Start()
    {
        //Get the hp manager for the boss
        if (GetComponent<BossHealthManager>() != null)
        {
            hpManager = GetComponent<BossHealthManager>();
            hpManager.Invulnerable();
        }
        else
        {
            Debug.LogError("The GameObject doesn't have a BossHealthManager Script");
        }
        //If the canvas is active, it will desactivate it
        if (canvas_Hp.activeSelf)
        {
            canvas_Hp.SetActive(false);
        }

        //Get animator
        anim = GetComponent<Animator>();
        //Find the player
        player = GameObject.FindGameObjectWithTag("Player").transform;

        //Reset variables
        target = player;
        resetMeleeDelay = meleeDelay;
        resetBursts = bulletBurstsCount;
        bossAgent.speed = chaseSpeed;
        meleeDelay = 0;
        resetStepsTime = timeBetweenFootsteps;
    }

    //Do actions
    void Update()
    {
        if (activated && !isDead)
        {
            if (!isShooting)
            {
                MeleeAttacking();
            }
            else
            {
                Shooting();
            }
        }
        if (calculateSteps && (footStepSounds != null && footStepSounds.Length>0))
        {
            if(timeBetweenFootsteps > 0)
            {
                timeBetweenFootsteps -= Time.deltaTime;
                if (timeBetweenFootsteps <= 0)
                {
                    int random = Random.Range(0, footStepSounds.Length);
                    Instantiate(footStepSounds[random], footstepPosition.position, footstepPosition.rotation);
                    Instantiate(footstepParticle, footstepPosition.position, footstepPosition.rotation);
                    timeBetweenFootsteps = resetStepsTime;
                }
            }
        }
        if(calculateRandom && (randomSounds != null && randomSounds.Length > 0))
        {
            if (timeBetweenRandomSounds > 0)
            {
                timeBetweenRandomSounds -= Time.deltaTime;
                if (timeBetweenRandomSounds <= 0)
                {
                    int random = Random.Range(0, randomSounds.Length);
                    Instantiate(randomSounds[random], transform.position, transform.rotation);
                    timeBetweenRandomSounds = resetRandomTime;
                }
            }
        }
    }
    private void MeleeAttacking()
    {
        //Tries to go to the player
        SetTarget(target.position);
        if ((target.position-transform.position).sqrMagnitude <= (meleeStartDistance*meleeStartDistance))
        {
            //Reached the distance to start attacking and attacks
            if (meleeDelay <= 0)
            {
                StartCoroutine(DoMeleeAttack());
            }
            else
            {
                meleeDelay -= Time.deltaTime;
            }
        }
        //If it's to close to the player, stops but continue attacking
        if((target.position - transform.position).sqrMagnitude <= (bossAgent.stoppingDistance * bossAgent.stoppingDistance))
        {
            if(anim!= null)
            {
                anim.SetInteger("AnimVal", 0);
            }
            bossAgent.isStopped = true;
            FacePlayer();
        }
        //Continue following the player
        else
        {
            if(anim!= null)
            {
                anim.SetInteger("AnimVal", 1);
            }
            bossAgent.isStopped = false;
        }
        if (!bossAgent.isStopped)
        {
            calculateSteps = true;
        }
        else
        {
            calculateSteps = false;
        }
    }
    private void Shooting()
    {

        if ((target.position - transform.position).sqrMagnitude <= (bossAgent.stoppingDistance * bossAgent.stoppingDistance))
        {
            if (cntTemp < 1)
            {
                isPositioning = false;
            }
            //Reach position
            if (!isPositioning)
            {
                FacePlayer();
                if (!doingShot)
                {
                    StartCoroutine(ShotStartDelay());
                }
            }
            calculateSteps = false;
            if (anim != null)
            {
                anim.SetInteger("AnimVal", 0);
            }
            cntTemp++;
        }
        else
        {
            //Go to a shooting position
            SetTarget(target.position);
            if(anim != null)
            {
                anim.SetInteger("AnimVal", 2);
            }
            calculateSteps = true;
        }
    }
    private void FacePlayer()
    {
        Vector3 lookPlayer = new Vector3(player.position.x, player.position.y, bulletSpawn.position.z);
        bulletSpawn.LookAt(lookPlayer);
        //See the player
        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, (2 * Time.deltaTime));
    }

    //Set where the boss will go
    private bool SetTarget(Vector3 target)
    {
        if (target != null)
        {
            if (bossAgent.SetDestination(target))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return true;
        }
    }

    #endregion

    #region Get Set

    public void ActivateBoss()
    {
        StartCoroutine(StartAnimation());
    }
    public void Death()
    {
        isDead = true;
        calculateRandom = false;
        calculateSteps = false;
    }
    public int GetHealth_ToShot()
    {
        return healthToShot;
    }
    public void ChangeShooting()
    {
        //Choose a random shot position and change values. The boss can't receive damage while in shooting phase
        isShooting = true;
        bossAgent.isStopped = false;
        int randomPos = Random.Range(0, movePositions.Length);
        target = movePositions[randomPos];
        bossAgent.speed = changePosSpeed;
        isPositioning = true;
        if (changeToShootingSound != null)
        {
            Instantiate(changeToShootingSound, transform.position, transform.rotation);
        }
    }

    #endregion

    #region Coroutines

    //Activating animation
    private IEnumerator StartAnimation()
    {
        if (anim != null)
        {
            anim.SetBool("HasActivated", true);
        }
        if (!canvas_Hp.activeSelf)
        {
            canvas_Hp.SetActive(true);
        }
        if(activateSound!= null)
        {
            Instantiate(activateSound, transform.position, transform.rotation);
        }
        yield return new WaitForSeconds(startAnimationTime);
        //The boss can receive damage
        hpManager.Vulnerable();
        if (anim != null)
        {
            anim.SetInteger("AnimVal", 1);
        }
        activated = true;
        calculateRandom = true;
    }
    //Do melee attack
    private IEnumerator DoMeleeAttack()
    {
        if (anim != null)
        {
            anim.Play("Melee_Attack");
        }
        meleeDelay = resetMeleeDelay;
        if(meleeAttackSound!= null)
        {
            Instantiate(meleeAttackSound, transform.position, transform.rotation);
        }
        yield return new WaitForSeconds(meleeWaitToDamage);
        RaycastHit hit;
        if (Physics.SphereCast(meleePosition.position, meleeRadius, transform.forward, out hit, meleeRange))
        {
            //Only damage the player
            if(hit.collider.gameObject.tag == "Player")
            {
                if(hit.collider.GetComponent<Health>()!= null)
                {
                    hit.collider.GetComponent<Health>().GetDamage(meleeDamage);
                }
            }
        }

    }
    //Time to aim player
    private IEnumerator ShotStartDelay()
    {
        bossAgent.isStopped = true;
        doingShot = true;
        if(anim!= null)
        {
            anim.SetInteger("Gun_AnimVal", 1);
        }
        yield return new WaitForSeconds(shotsWaitToStart);
        StartCoroutine(DoShot());
    }
    //Shot
    private IEnumerator DoShot()
    {
        emptyBulletIndex.Clear();
        if(muzzleflashPrefab != null)
        {
            Instantiate(muzzleflashPrefab, bulletSpawn.position, bulletSpawn.rotation);
        }
        if(shotSound!= null)
        {
            Instantiate(shotSound, bulletSpawn.position, bulletSpawn.rotation);
        }
        //Randomlly chose empty positions for the player to dodge the shots
        for(int i = 0; i<emptyBulletsCount; i++)
        {
            int random = Random.Range(0, bulletsCount);
            emptyBulletIndex.Add(random);
        }
        if (anim != null)
        {
            anim.Play("Shot");
        }
        //Shots in a 180° area in front of the shot position
        for(int i = 0; i<bulletsCount-1; i++)
        {
            if (emptyBulletIndex.Contains(i))
            {

            }
            else
            {
                GameObject tempBullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
                float tempRotation = (bulletSpawn.transform.localEulerAngles.y + (((180 / bulletsCount) * i) -90));
                tempBullet.transform.localEulerAngles = new Vector3(tempBullet.transform.localEulerAngles.x, tempBullet.transform.localEulerAngles.y + tempRotation, tempBullet.transform.localEulerAngles.z);
                tempBullet.GetComponent<Boss_Shot>().SetValues(bulletSpeed, bulletRadius, bulletsDamage);
            }
        }
        yield return new WaitForSeconds(shotsDelay);
        bulletBurstsCount--;
        //Repeat shooting
        if(bulletBurstsCount > 0)
        {
            StartCoroutine(DoShot());
        }
        else
        {
            //Changes values to chase phase
            bulletBurstsCount = resetBursts;
            target = player;
            bossAgent.speed = chaseSpeed;
            //Makes the boss vulnerable to damage
            hpManager.Vulnerable();
            bossAgent.isStopped = false;
            if (anim != null)
            {
                anim.SetInteger("Gun_AnimVal", 0);
                anim.SetInteger("AnimVal", 1);
            }
            cntTemp = 0;
            doingShot = false;
            isPositioning = false;
            isShooting = false;
        }
    }
    #endregion
}
