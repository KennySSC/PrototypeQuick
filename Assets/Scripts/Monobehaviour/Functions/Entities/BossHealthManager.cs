using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthManager : MonoBehaviour
{
    #region Serializable variables

    [Header("UI Settings")]

    [SerializeField] Slider healthBar;

    [Tooltip("The slider fill")]
    [SerializeField] Image healthBar_Fill;

    [Tooltip("The colors that the fill will have depending on the slider normalized value")]
    [SerializeField] Gradient healthGradient;

    [Tooltip("Numeric representation of health points")]
    [SerializeField] TMP_Text healtPoints_Text;

    [Tooltip("Game objects with health script. Their max health will be added to create the max health of the boss. But they have their independent health ammount")]
    [SerializeField] GameObject[] damagableZones;


    [Space]


    [Header("Events Settings")]


    [Tooltip("Events that will trigger once the boss dies")]
    [SerializeField] List<Event> deadEvents = new List<Event>();


    [Space]


    [Header("Sound")]


    [Tooltip("It will perform when the health reaches 0. If you don't need it leave it empty")]
    [SerializeField] GameObject deathSound;


    [Space]


    [Header("Debug variables")]


    [SerializeField] int currentHealth = 0;
    [SerializeField] int currentReceivedDamage;

    #endregion

    #region private variables

    private Boss_Controller bossController;
    private Enemy_Spawner spawner;

    private int hpControl;

    private bool isDead = false;

    #endregion

    #region Main Functions
    void Start()
    {
        int index = 0;
        if (GetComponent<Boss_Controller>() != null)
        {
            bossController = GetComponent<Boss_Controller>();
            hpControl = bossController.GetHealth_ToShot();
        }
        else
        {
            Debug.LogError("The GameObject doesn't have a Boss_Controller Script");
        }
        //Adds all max healths to set the boss health bar
        foreach (GameObject obj in damagableZones)
        {

            if (obj.GetComponent<Health>() != null)
            {
                currentHealth += obj.GetComponent<Health>().GetMaxHealth();
                obj.GetComponent<Health>().GetBossManager(this);
            }
            else
            {
                Debug.LogError("The Damagable Zone #" + index + " doesn't have a Health Script");
            }
            index++;
        }
        if (healthBar != null && healthBar_Fill != null)
        {
            healthBar.maxValue = currentHealth;
            healthBar.value = currentHealth;
            healthBar_Fill.color = healthGradient.Evaluate(healthBar.normalizedValue);
        }
        if (healtPoints_Text != null)
        {
            healtPoints_Text.text = currentHealth.ToString("000");
        }
    }

    #endregion

    #region Get Set


    public void GetDamage(int ammount)
    {
        //Evaluates current health and if it's alive
        if (!isDead)
        {
            currentReceivedDamage += ammount;
            currentHealth -= ammount;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                if(deathSound != null)
                {
                    Instantiate(deathSound, transform.position, transform.rotation);   
                }
                //Do dead evens
                foreach(Event evt in deadEvents)
                {
                    List<GameObject> obj = new List<GameObject>();
                    obj.Add(this.gameObject);
                    obj.Add(spawner.gameObject);
                    evt.DoEvent();
                }
                //Do death animation
                bossController.Death();
            }
            //Set UI values
            if (healthBar != null && healthBar_Fill != null)
            {
                healthBar.value = currentHealth;
                healthBar_Fill.color = healthGradient.Evaluate(healthBar.normalizedValue);
            }
            if (healtPoints_Text != null)
            {
                healtPoints_Text.text = currentHealth.ToString("000");
            }
            //If the health has been reduced the ammount gived by the controller, it will become invulnerable to damage
            if (currentReceivedDamage >= hpControl && currentHealth > 0)
            {
                Invulnerable();
                bossController.ChangeShooting();
                currentReceivedDamage -= hpControl;
            }


        }
    }
    //Add externally dead events
    public void Add_DeadEvents(List<Event> newEvents)
    {
        foreach(Event evt in newEvents)
        {
            if (!deadEvents.Contains(evt))
            {
                deadEvents.Add(evt);
            }
        }
    }
    //Set it's spawner if needed
    public void SetSpawner(Enemy_Spawner newSpawner)
    {
        spawner = newSpawner;
    }
    //Get it's current spawner
    public Enemy_Spawner GetSpawner()
    {
        return spawner;
    }
    //Desactivate the vulnerable zones, making it invulnerable
    public void Invulnerable()
    {
        foreach (GameObject obj in damagableZones)
        {
            if (obj.activeSelf)
            {
                obj.SetActive(false);
            }
        }
    }
    //Activate the vulnerable zones, making it vulnerable
    public void Vulnerable()
    {
        foreach (GameObject obj in damagableZones)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
            }
        }
    }

    #endregion 
}
