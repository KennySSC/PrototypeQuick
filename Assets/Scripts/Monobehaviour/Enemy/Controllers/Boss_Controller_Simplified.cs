using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Boss_Controller_Simplified : MonoBehaviour
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


    [Tooltip("The distance from the player to start shooting")]
    [SerializeField] float startShotDistance;

    [Tooltip("Ammount of bullets that the boss shoot at once")]
    [SerializeField] int bulletsCount;

    [Tooltip("Ammount of empty spaces that replaces shot positions, for the player to dodge the bullets")]
    [SerializeField] int emptyBulletsCount;

    [SerializeField] float bulletSpeed;

    [SerializeField] float bulletRadius;

    [SerializeField] int bulletsDamage;

    [Tooltip("The time that need the boss to shot a new burst of bullets")]
    [SerializeField] float shotsDelay;

    [Space]


    [Header("Melee Settings")]


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


    [Space]


    [Header("Animation settings")]

    [Tooltip("If you aren't using animation leave it empty")]
    [SerializeField] Animator anim;


    [Space]


    [Header("Sound & Particles")]


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
    private List<int> emptyBulletIndex = new List<int>();

    private float resetStepsTime;
    private float resetRandomTime;
    private float resetMeleeDelay;
    private float resetShotDelay;

    private bool isDead = false;
    private bool calculateSteps = false;
    private bool calculateRandom = false;

    #endregion

    #region Main functions

    void Start()
    {
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
        resetShotDelay = shotsDelay;
        meleeDelay = 0;
        shotsDelay = 0;
        resetStepsTime = timeBetweenFootsteps;
    }

    //Do actions
    void Update()
    {
        if (!isDead)
        {
            DoAttacks();
        }
        if (calculateSteps && (footStepSounds != null && footStepSounds.Length > 0))
        {
            if (timeBetweenFootsteps > 0)
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
        if (calculateRandom && (randomSounds != null && randomSounds.Length > 0))
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
    private void DoAttacks()
    {
        //Tries to go to the player
        SetTarget(player.position);
        //Melee attacking
        if ((player.position - transform.position).sqrMagnitude <= (meleeStartDistance * meleeStartDistance))
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
        //Shots
        if ((player.position - transform.position).sqrMagnitude <= (meleeStartDistance * meleeStartDistance))
        {
            //Reached the distance to start shooting
            if (shotsDelay <= 0)
            {
                StartCoroutine(DoShot());
            }
            else
            {
                shotsDelay -= Time.deltaTime;
            }
        }
        //If it's to close to the player, stops but continue attacking
        if ((player.position - transform.position).sqrMagnitude <= (bossAgent.stoppingDistance * bossAgent.stoppingDistance))
        {
            if (anim != null)
            {
                anim.SetInteger("AnimVal", 0);
            }
            bossAgent.isStopped = true;
            FacePlayer();
        }
        //Continue following the player
        else
        {
            if (anim != null)
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
    private void FacePlayer()
    {
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
    public void Death()
    {
        isDead = true;
        calculateRandom = false;
        calculateSteps = false;
    }

    #endregion

    #region Coroutines

    //Do melee attack
    private IEnumerator DoMeleeAttack()
    {
        if (anim != null)
        {
            anim.Play("Melee_Attack");
        }
        meleeDelay = resetMeleeDelay;
        if (meleeAttackSound != null)
        {
            Instantiate(meleeAttackSound, transform.position, transform.rotation);
        }
        yield return new WaitForSeconds(meleeWaitToDamage);
        RaycastHit hit;
        if (Physics.SphereCast(meleePosition.position, meleeRadius, transform.forward, out hit, meleeRange))
        {
            //Only damage the player
            if (hit.collider.gameObject.tag == "Player")
            {
                if (hit.collider.GetComponent<Health>() != null)
                {
                    hit.collider.GetComponent<Health>().GetDamage(meleeDamage);
                }
            }
        }

    }

    //Shot
    private IEnumerator DoShot()
    {
        emptyBulletIndex.Clear();
        if (muzzleflashPrefab != null)
        {
            Instantiate(muzzleflashPrefab, bulletSpawn.position, bulletSpawn.rotation);
        }
        if (shotSound != null)
        {
            Instantiate(shotSound, bulletSpawn.position, bulletSpawn.rotation);
        }
        //Randomlly chose empty positions for the player to dodge the shots
        for (int i = 0; i < emptyBulletsCount; i++)
        {
            int random = Random.Range(0, bulletsCount);
            emptyBulletIndex.Add(random);
        }
        if (anim != null)
        {
            anim.Play("Shot");
        }
        //Shots in a 180° area in front of the shot position
        for (int i = 0; i < bulletsCount - 1; i++)
        {
            if (emptyBulletIndex.Contains(i))
            {

            }
            else
            {
                GameObject tempBullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
                float tempRotation = (bulletSpawn.transform.localEulerAngles.y + (((180 / bulletsCount) * i) - 90));
                tempBullet.transform.eulerAngles = new Vector3(tempBullet.transform.localEulerAngles.x, tempBullet.transform.localEulerAngles.y + tempRotation, tempBullet.transform.localEulerAngles.z);
                tempBullet.GetComponent<Boss_Shot>().SetValues(bulletSpeed, bulletRadius, bulletsDamage);
            }
        }
        shotsDelay = resetShotDelay;
        yield return null;
    }
    #endregion 
}
