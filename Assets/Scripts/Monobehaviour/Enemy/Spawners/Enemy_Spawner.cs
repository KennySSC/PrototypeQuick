using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnerMode {Random_Spawn, Ordered_Infinite, Horde};
public class Enemy_Spawner : MonoBehaviour
{
    #region Serializable Variables

    [Header("Main Settings")]


    [Tooltip("Scriptable objects with enemies prefabs that will be spawned")]
    [SerializeField] EnemyList _enemyList;

    [Tooltip("Events that will be triggered when killing a enemy that spawned from here")]
    [SerializeField] List<Event> killEnemyEvents = new List<Event>();

    [Tooltip("Events that will trigger each time that a new enemy spawns")]
    [SerializeField] List<Event> spawnEnemyEvents = new List<Event>();

    [Tooltip("Enemy patrol positions")]
    [SerializeField] List<Transform> patrolPositions = new List<Transform>();

    [Tooltip("Change the spawner behaviour. Random_Spawn = Randomly takes a enemy from the list and spawns it; Ordered_Infinite = Spawns the enemies in the list order, when the last spawns" +
        "the first enemy will be spawned next and repeat the cycle; Horde = it will spawns the enemies in the list order, but when reach the last one it will stop spawning")]
    [SerializeField] SpawnerMode spawnMode;

    [Tooltip("Positions where the enemies can spawn. Each spawn will take one randomly")]
    [SerializeField] Transform[] positions;

    [Tooltip("Max enemies allowed to be created by this spawn. No enemy will spawn until one dies")]
    [SerializeField] private int maxEnemies_AtOnce;

    [Tooltip("Time that takes the spawn to generate a new enemy when it can")]
    [SerializeField] private float timeBetweenSpawns;


    [Space]


    [Header("Burst Spawn Settigs")]


    [Tooltip("Allows the spawner to generate a certain amount of enemies ignoring wait time when the game starts")]
    [SerializeField] bool burstSpawn;

    [Tooltip("The amount ef enemies spawned in the burst. It should be less than the max enemies at once. If this value is greater, it will be automatically equal to max enemies at once")]
    [SerializeField] private int burstSpawn_Count;


    [Space]


    [Header("Sound")]


    [Tooltip("It will perform each time one enemy spawns. If you don't need it leave it empty")]
    [SerializeField] GameObject spawnSound;

    #endregion

    #region Private variables

    private Hordes_Manager manager;
    private GameObject currentEnemyToSpawn;
    [SerializeField]private List<GameObject> enemiesToSpawn = new List<GameObject>();

    private bool CanSpawn = true;
    private bool waitingNextHorde = false;

    private int currentIndex = 0;
    private int resetBurst = 0;
    private int currentEnemies = 0;

    [SerializeField]private float reset_TimeBetweenSpawns;
    private float waitTime_ToNextHorde = 0;

    #endregion

    #region Main Functions
    void Start()
    {
        //Set variables

        reset_TimeBetweenSpawns = timeBetweenSpawns;
        resetBurst = burstSpawn_Count;
        if(manager == null)
        {
            foreach (GameObject obj in _enemyList.GetEnemyList())
            {
                enemiesToSpawn.Add(obj);
            }
            StartCoroutine(NextHorde_WaitTime());
        }

    }
    private void Update()
    {
        //Wait time to spawn a new enemy
        if (timeBetweenSpawns > 0 && !waitingNextHorde)
        {
            timeBetweenSpawns -= Time.deltaTime;
            if (timeBetweenSpawns <= 0)
            {
                if (currentEnemies < maxEnemies_AtOnce && CanSpawn)
                {
                    SpawnEnemy();
                    timeBetweenSpawns = reset_TimeBetweenSpawns;
                }
            }
        }
    }
    public void SpawnEnemy()
    {
        //Set a random position of the list
        int random = Random.Range(0, positions.Length);

        //Random Spawn
        if (spawnMode == SpawnerMode.Random_Spawn)
        {
            
            int randomIndex = Random.Range(0, enemiesToSpawn.Count);
            currentEnemyToSpawn = enemiesToSpawn[randomIndex];
        }
        //Horde spawn
        else if (spawnMode == SpawnerMode.Horde)
        {
            if (enemiesToSpawn.Count > 0)
            {
                currentEnemyToSpawn = enemiesToSpawn[0];
                enemiesToSpawn.Remove(currentEnemyToSpawn);
            }
        }
        //Ordered spawn
        else if (spawnMode == SpawnerMode.Ordered_Infinite)
        {
            if (currentIndex > (enemiesToSpawn.Count - 1))
            {
                currentIndex = 0;
            }
            currentEnemyToSpawn = enemiesToSpawn[currentIndex];
        }
        if(currentEnemyToSpawn != null)
        {

            if (spawnSound != null)
            {
                Instantiate(spawnSound, positions[random].position, positions[random].rotation);
            }

            //Spawn the enemy and set events to it. Also assign this spawner
            GameObject spawn = Instantiate(currentEnemyToSpawn, positions[random].position, positions[random].rotation);

            if (spawn.GetComponent<Enemy_Base>() != null)
            {
                spawn.GetComponent<Enemy_Base>().Set_Waypoints(patrolPositions);
            }
            else if (spawn.GetComponentInChildren<Enemy_Base>() != null)
            {
                spawn.GetComponentInChildren<Enemy_Base>().Set_Waypoints(patrolPositions);
            }
            else if (spawn.GetComponentInParent<Enemy_Base>() != null)
            {
                spawn.GetComponentInParent<Enemy_Base>().Set_Waypoints(patrolPositions);
            }
            if (spawn.GetComponent<BossHealthManager>() != null)
            {
                spawn.GetComponent<BossHealthManager>().Add_DeadEvents(killEnemyEvents);
                spawn.GetComponent<BossHealthManager>().SetSpawner(this);
            }
            else if (spawn.GetComponentInParent<BossHealthManager>() != null)
            {
                spawn.GetComponentInParent<BossHealthManager>().Add_DeadEvents(killEnemyEvents);
                spawn.GetComponentInParent<BossHealthManager>().SetSpawner(this);
            }
            else if (spawn.GetComponentInChildren<BossHealthManager>() != null)
            {
                spawn.GetComponentInChildren<BossHealthManager>().Add_DeadEvents(killEnemyEvents);
                spawn.GetComponentInChildren<BossHealthManager>().SetSpawner(this);
            }
            else if (spawn.GetComponent<Health>() != null && !spawn.GetComponent<Health>().GetIsBoss())
            {
                spawn.GetComponent<Health>().Add_DeadEvents(killEnemyEvents);
                spawn.GetComponent<Health>().SetSpawner(this);
            }
            else if (spawn.GetComponentInParent<Health>() != null && !spawn.GetComponentInParent<Health>().GetIsBoss())
            {
                spawn.GetComponentInParent<Health>().Add_DeadEvents(killEnemyEvents);
                spawn.GetComponentInParent<Health>().SetSpawner(this);
            }else if (spawn.GetComponentInChildren<Health>() != null && !spawn.GetComponentInChildren<Health>().GetIsBoss())
            {
                spawn.GetComponentInChildren<Health>().Add_DeadEvents(killEnemyEvents);
                spawn.GetComponentInChildren<Health>().SetSpawner(this);
            }
            foreach (Event evt in spawnEnemyEvents)
            {
                evt.DoEvent();
            }
        }
        if (enemiesToSpawn.Count <= 0)
        {
            currentEnemyToSpawn = null;
        }

    }

    #endregion

    #region Get Set

    //Call this to externally add events that will trigger when any enemy generated by this spawner dies
    public void Add_KillEnemyEvent(List<Event> eventsAdd)
    {
        foreach(Event evt in eventsAdd)
        {
            if (!killEnemyEvents.Contains(evt))
            {
                killEnemyEvents.Add(evt);
            }
        }
    }
    //Call this to externally add events that will trigger when this spawner generate a new enemy
    public void Add_EnemySpawnEvent(List<Event> eventsAdd)
    {
        foreach (Event evt in eventsAdd)
        {
            if (!spawnEnemyEvents.Contains(evt))
            {
                spawnEnemyEvents.Add(evt);
            }
        }
    }
    //Externally changes the enemy list and reset values
    public void SetEnemyList(EnemyList newList)
    {
        _enemyList = newList;
        currentEnemyToSpawn = null;
        enemiesToSpawn.Clear();
        foreach (GameObject obj in _enemyList.GetEnemyList())
        {
            enemiesToSpawn.Add(obj);
        }
        StartCoroutine(NextHorde_WaitTime());
    }
    public void SetTimeBetweenSpawns(float newTime)
    {
        timeBetweenSpawns = newTime;
        reset_TimeBetweenSpawns = newTime;
    }
    public void SetBurstSpawnCount(int newCount)
    {
        burstSpawn_Count = newCount;
        resetBurst = newCount;
    }
    //Tell to this spawner that a enemy has killed
    public void KillEnemy()
    {
        currentEnemies--;
    }
    //Set horde manager externally
    public void SetManager(Hordes_Manager newManager)
    {
        manager = newManager;
    }
    //Set can spawn externally
    public void SetCanSpawn(bool newValue)
    {
        CanSpawn = newValue;
    }
    //Set can burst spawn externally
    public void SetBurstSpawn(bool newValue)
    {
        burstSpawn = newValue;
    }
    //Set a wait time between hordes externally. DOESN'T AFFECT TIME BETWEEN SPAWNS
    public void SetWaitTime_NextHorde(float newTime)
    {
        waitTime_ToNextHorde = newTime;
    }

    public void SetMaxEnemies_AtOnce(int newMaxEnemies)
    {
        maxEnemies_AtOnce = newMaxEnemies;
    }
    public void SetPatrolPoints(List<Transform> newPatrolPoints)
    {
        foreach(Transform tn in newPatrolPoints)
        {
            if (!patrolPositions.Contains(tn))
            {
                patrolPositions.Add(tn);
            }
        }
    }
    #endregion

    #region Coroutines

    IEnumerator NextHorde_WaitTime()
    {
        waitingNextHorde = true;
        yield return new WaitForSeconds(waitTime_ToNextHorde);
        if (burstSpawn)
        {
            if (burstSpawn_Count > enemiesToSpawn.Count)
            {
                burstSpawn_Count = enemiesToSpawn.Count;
            }
            for (int i = 0; i < burstSpawn_Count; i++)
            {
                SpawnEnemy();
            }
        }
        else
        {
            SpawnEnemy();
        }
        waitingNextHorde = false;
    }

    #endregion 
}
