using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//This for the class to be serializable, create and assign hordes for each spawn independently
[System.Serializable]
public class Hordes_Holder
{
    #region Hordes_Holder Serializable variables

    [Tooltip("Enemy lists that will be used as hordes")]
    [SerializeField] public List<EnemyList> enemyHordes;

    [Tooltip("The spawner that will have the hordes")]
    [SerializeField] public Enemy_Spawner spawner;

    #endregion
}
public class Hordes_Manager : MonoBehaviour
{
    #region Hordes_Manager Serializable variables
    [Header("Main Settings")]


    [Tooltip("Create one for each spawner that you will use")]
    [SerializeField] Hordes_Holder[] enemySpawners_WithHordes;

    [Tooltip("Time that takes the spawn to generate a new enemy when it can")]
    [SerializeField] float timeBetweenSpawns;

    [Tooltip("Allows the spawners to generate a certain amount of enemies ignoring wait time when the game starts, but waiting the next horde time")]
    [SerializeField] bool burstSpawn;

    [Tooltip("The amount ef enemies spawned in the burst. It should be less than the max enemies at once. If this value is greater, it will be automatically equal to max enemies at once")]
    [SerializeField] int burstSpawnCount;

    [Tooltip("Time that must pass after finishing a horde to start a new one")]
    [SerializeField] float timeBetweenHordes;

    [Tooltip("Max enemies that can be in the map at once, overriding the spawners amount")]
    [SerializeField] int maxEnemies_AtOnce;

    [Space]


    [Header("Events Settings")]


    [Tooltip("Event that will tell to the spawner of the killed enemy that one has been killed")]
    [SerializeField] KillEnemy_Manager_Event killEvent;

    [Tooltip("Event that will tell to the manager that a enemy has spawned")]
    [SerializeField] SpawnEnemy_Event spawnEvent;

    [Tooltip("Events that the enemies will trigger when killed. If you don't need them, leave it empty")]
    [SerializeField] List<Event> enemiesKillEvents = new List<Event>();

    [Tooltip("Events that the spawners will trigger each time that they spawn")]
    [SerializeField] List<Event> enemiesSpawnEvents = new List<Event>();

    [Tooltip("Events that will trigger after each finished horde")]
    [SerializeField] List<Event> finishHordeEvents = new List<Event>();

    [Tooltip("Events that will trigger when the game finish")]
    [SerializeField] List<Event> finishGameEvents = new List<Event>();


    [Space]


    [Header("UI settings")]


    [SerializeField] TMP_Text hordesCount_Text;
    [SerializeField] TMP_Text enemiesLeft_Text;

    #endregion

    #region Private variables

    private Transform playerPosition;

    int currentEnemies = 0;
    int currentHorde = 0;
    int enemiesLeft =0;
    int hordeCount;

    #endregion

    #region Main Functions
    private void Awake()
    {
        //Get hordes count
        foreach(Hordes_Holder hh in enemySpawners_WithHordes)
        {
            if (hh.enemyHordes.Count > hordeCount)
            {
                hordeCount = hh.enemyHordes.Count;
            }
        }
        if (burstSpawnCount > maxEnemies_AtOnce)
        {
            burstSpawnCount = maxEnemies_AtOnce;
        }
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        AssignHordes();

    }
    private void AssignHordes()
    {
        //Assign hordes and events to the spawners
        enemiesLeft = 0;
        foreach(Hordes_Holder hh in enemySpawners_WithHordes)
        {
            if(currentHorde < hordeCount)
            {
                if (hh.enemyHordes.Count <= hordeCount)
                {
                    List<Transform> tempPlayerPosition = new List<Transform>();
                    hh.spawner.SetManager(this);
                    tempPlayerPosition.Add(playerPosition);
                    enemiesLeft += hh.enemyHordes[currentHorde].GetEnemyList().Count;
                    hh.spawner.SetWaitTime_NextHorde(timeBetweenHordes);
                    List<Event> tempList = enemiesKillEvents;
                    if (!tempList.Contains(killEvent))
                    {
                        tempList.Add(killEvent);
                    }
                    if (!tempList.Contains(spawnEvent))
                    {
                        tempList.Add(spawnEvent);
                    }
                    hh.spawner.Add_KillEnemyEvent(tempList);
                    hh.spawner.Add_EnemySpawnEvent(enemiesSpawnEvents);
                    hh.spawner.SetMaxEnemies_AtOnce(maxEnemies_AtOnce);
                    hh.spawner.SetPatrolPoints(tempPlayerPosition);
                    hh.spawner.SetBurstSpawn(burstSpawn);
                    hh.spawner.SetBurstSpawnCount(burstSpawnCount);
                    hh.spawner.SetTimeBetweenSpawns(timeBetweenSpawns);
                    if (hh.enemyHordes[currentHorde] != null)
                    {
                        hh.spawner.SetEnemyList(hh.enemyHordes[currentHorde]);
                    }

                }
            }
        }
        currentHorde++;
        UpdateTexts();

    }
    #endregion

    #region Get Set

    private void UpdateTexts()
    {
        hordesCount_Text.text = "Horde " + currentHorde.ToString() + "/" + hordeCount.ToString();
        enemiesLeft_Text.text = "Enemies Left: " + enemiesLeft.ToString();
    }
    //Call this everytime a enemy spawns
    public void AddNewEnemy(int enemyAdd)
    {
        currentEnemies += enemyAdd;
        UpdateSpawns();
    }
    //Updates spawns to generate or stop generating enemies
    private void UpdateSpawns()
    {
        if (currentEnemies >= maxEnemies_AtOnce)
        {
            foreach (Hordes_Holder hh in enemySpawners_WithHordes)
            {
                hh.spawner.SetCanSpawn(false);
            }
        }
        else
        {
            foreach (Hordes_Holder hh in enemySpawners_WithHordes)
            {
                hh.spawner.SetCanSpawn(true);
            }
        }
    }
    //Call this every time a enemy has been killed
    public void KillEnemy()
    {
        enemiesLeft--;
        currentEnemies--;
        UpdateTexts();
        UpdateSpawns();
        if(enemiesLeft <= 0)
        {
            foreach(Event evt in finishHordeEvents)
            {
                evt.DoEvent();
            }
            if(currentHorde < hordeCount)
            {
                AssignHordes();
            }
            else
            {
                foreach(Event evt in finishGameEvents)
                {
                    evt.DoEvent();
                }
            }

        }
    }

    #endregion
}
