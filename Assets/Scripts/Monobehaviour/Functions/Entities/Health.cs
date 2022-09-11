using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Health : MonoBehaviour
{
    #region Serializable variables

    [Header("Main Settings")]


    [SerializeField] int maxHealth;

    [SerializeField] bool canReceiveDamage = true;

    [SerializeField] bool destroyAfterDeath = true;

    [SerializeField] bool isPlayer = false;

    [Space]

    [Header("Boss settings")]
    [Tooltip("Only applies with a object that has boss health manager script. If you are using the included simplified boss this goes off")]
    [SerializeField] bool isBoss = false;

    [Tooltip("Renderer of the piece of the boss. Its material will be changed when the health reaches 0")]
    [SerializeField] Renderer vulnerablePiece;

    [Tooltip("The material that will be applied to the vulnerable piece")]
    [SerializeField] Material deadPieceMaterial;

    [Space]


    [Header("UI settings")]


    [Tooltip("The Gameobject canvas that contains all the healthbar components")]
    [SerializeField] GameObject healthBarCanvas;

    [Tooltip("Slider component")]
    [SerializeField] Slider healthBar;

    [Tooltip("Slider's fill")]
    [SerializeField] Image healthBar_Fill;

    [SerializeField] Gradient healthGradient;

    [SerializeField] TMP_Text healthPoints;


    [Space]


    [Header("Healing by time settings")]


    [Tooltip("Activate autohealing by time, only applies for minions (not bosses)")]
    [SerializeField] bool healingByTime;

    [Tooltip("Healing points per second")]
    [SerializeField] int healingRate;

    [Tooltip("Wait time without damage to start auto healing")]
    [SerializeField] float waitTimeToHealing;


    [Space]


    [Header("Respawn Settings")]

    [Tooltip("Turn on if you want to override the death events and instead of losing, respawning the player. This only works if the bool *isPlayer* is also turned on ")]
    [SerializeField] bool canRespawn = false;

    [SerializeField] Transform respawnPosition;

    [Tooltip("The wait time before the player can move again after respawning")]
    [SerializeField] float respawnTime = 1f;

    [Tooltip("Events that will trigger when the player health reach 0. Think them as the ones that will trigger instead of the death events.If you don't need them leave it empty")]
    [SerializeField] List<Event> preRespawnEvents = new List<Event>();

    [Tooltip("Events that will trigger when the player health respawns. If you don't need them leave it empty")]
    [SerializeField] List<Event> respawnEvents = new List<Event>();

    [Tooltip("This events will trigger when you call the function *TriggerLoseEvents*, these are the alternative 'death' events when you want the player to lose")]
    [SerializeField] List<Event> loseEvents = new List<Event>();

    [Space]



    [Header("Death effects settings")]


    [Tooltip("Dissolve effect after dying duration. If you allow respawning, after the animation the player will be teleported to the respawn position.")]
    [SerializeField] float dissolveDuration;

    [Tooltip("Animation duration before dissolving. It will be used for pre respawning to")]
    [SerializeField] float deathAnimation_Time = 1f;


    [Space]


    [Header("Camera Shake")]

    [Tooltip("Turn on if it's player and you wan't to shake the camera when it receive damage")]
    [SerializeField] bool shakeCamera = false;

    [Tooltip("Shake strength")]
    [SerializeField] float shakeMagnitude;

    [Tooltip("Time to stop shaking")]
    [SerializeField] float shakeTime;

    [Space]


    [Header("Death events")]


    [Tooltip("Any script that inherits from Event, if you want to trigger a event when this health script reaches 0")]
    [SerializeField] List<Event> deathEvents = new List<Event>();


    [Space]


    [Header("Sound")]


    [Tooltip("It will perform each time the object receives damage. If you don't need it leave it empty")]
    [SerializeField] GameObject damageSound;

    [Tooltip("It will perform when health reaches 0. If you don't need it leave it empty")]
    [SerializeField] GameObject deathSound;

    [Tooltip("It will perform each time the object receives health. If you don't need it leave it empty")]
    [SerializeField] GameObject healingSound;


    [Space]


    [Header("Debug Variables")]

    [SerializeField] private float currentHealth;

    #endregion

    #region Private variables

    private Animator anim;

    private List<Material> materials = new List<Material>();
    private MeshRenderer[] renderers;
    private SkinnedMeshRenderer[] skinRenderers;

    private Enemy_Base enemy;
    private BossHealthManager bossHpManager;
    private Enemy_Spawner spawner;

    private CameraFX cameraEffects;

    private bool isDying = false;

    private float resetWaitTime;

    private float healthNormalized;

    #endregion

    #region Main Functions

    private void Start()
    {
        currentHealth = maxHealth;
        resetWaitTime = waitTimeToHealing;
        if (!isBoss)
        {
            //If it has a slider health bar, it will set it to the max health values
            if (healthBar != null && healthPoints != null)
            {
                healthBar.maxValue = maxHealth;
                healthBar.value = maxHealth;
                //Show current health text
                healthPoints.text = currentHealth.ToString("00");
                //Re paint the slider fill
                healthGradient.Evaluate(1f);
            }
            //Get the animator if it has one
            if (GetComponent<Animator>() != null)
            {
                anim = GetComponent<Animator>();
            }
        }
        //Get all materials for dissolve effect
        if (GetComponent<MeshRenderer>() != null)
        {
            renderers = GetComponents<MeshRenderer>();
            foreach (MeshRenderer render in renderers)
            {
                Material[] mtls = render.materials;
                foreach (Material mtl in mtls)
                {
                    if (mtl != null && !materials.Contains(mtl))
                    {
                        materials.Add(mtl);
                    }
                }
            }

        }
        renderers = null;
        if (GetComponentInChildren<MeshRenderer>() != null)
        {
            renderers = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer render in renderers)
            {
                Material[] mtls = render.materials;
                foreach (Material mtl in mtls)
                {
                    if (mtl != null && !materials.Contains(mtl))
                    {
                        materials.Add(mtl);
                    }
                }
            }
        }
        renderers = null;
        if (GetComponentInParent<MeshRenderer>() != null)
        {
            renderers = GetComponentsInParent<MeshRenderer>();
            foreach (MeshRenderer render in renderers)
            {
                Material[] mtls = render.materials;
                foreach (Material mtl in mtls)
                {
                    if (mtl != null && !materials.Contains(mtl))
                    {
                        materials.Add(mtl);
                    }
                }
            }
        }

        if (GetComponent<SkinnedMeshRenderer>() != null)
        {
            skinRenderers = GetComponents<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer render in skinRenderers)
            {
                Material[] mtls = render.materials;
                foreach (Material mtl in mtls)
                {
                    if (mtl != null && !materials.Contains(mtl))
                    {
                        materials.Add(mtl);
                    }
                }
            }

        }
        skinRenderers = null;
        if (GetComponentInChildren<SkinnedMeshRenderer>() != null)
        {
            skinRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer render in skinRenderers)
            {
                Material[] mtls = render.materials;
                foreach (Material mtl in mtls)
                {
                    if (mtl != null && !materials.Contains(mtl))
                    {
                        materials.Add(mtl);
                    }
                }
            }
        }
        skinRenderers = null;
        if (GetComponentInParent<SkinnedMeshRenderer>() != null)
        {
            skinRenderers = GetComponentsInParent<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer render in skinRenderers)
            {
                Material[] mtls = render.materials;
                foreach (Material mtl in mtls)
                {
                    if (mtl != null && !materials.Contains(mtl))
                    {
                        materials.Add(mtl);
                    }
                }
            }
        }
        if (!isBoss)
        {
            //Get enemy IA
            if (GetComponent<Enemy_Base>() != null)
            {
                enemy = GetComponent<Enemy_Base>();
            }
        }
    }
    private void Update()
    {
        if (!isBoss)
        {
            HealingByTime();
        }
    }
    public void GetDamage(int damage)
    {
        //Reset no combat time to heal
        waitTimeToHealing = resetWaitTime;
        //Get damage
        if (canReceiveDamage)
        {
            if (isBoss)
            {
                if ((currentHealth - damage) >= 0)
                {
                    bossHpManager.GetDamage(damage);
                }
                else if ((currentHealth - damage) < 0 && currentHealth > 0)
                {
                    bossHpManager.GetDamage((int)currentHealth);
                }
                if (damageSound != null)
                {
                    Instantiate(damageSound, transform.position, transform.rotation);
                }
            }
            if (enemy != null)
            {
                enemy.TryToGoToPlayer();
            }
            currentHealth -= damage;
            //if camera effects applies values
            healthNormalized = (currentHealth / maxHealth);
            if (healthNormalized < 0)
            {
                healthNormalized = 0;
            }
            if (cameraEffects != null)
            {
                if (shakeCamera)
                {
                    cameraEffects.DoCameraShake(shakeTime, shakeMagnitude);
                }
                cameraEffects.CalculateVignette(healthNormalized);
                cameraEffects.CalculateSaturation(healthNormalized);
                cameraEffects.SetHealthState(0);
            }
            //If dies, start die events
            if (currentHealth <= 0 && !isBoss)
            {
                currentHealth = 0;
                if (deathSound != null)
                {
                    Instantiate(deathSound, transform.position, transform.rotation);
                }
                if (!isDying && !isBoss)
                {
                    if (enemy != null)
                    {
                        enemy.ChangeIsAlive(false);
                    }
                    //Triggers the death
                    StartCoroutine(StartDeath());


                }
            }
            if (currentHealth <= 0 && isBoss)
            {
                vulnerablePiece.material = deadPieceMaterial;
            }
            if (!isBoss)
            {
                //If it has healthbar, sets it's values
                if (healthBar != null && healthPoints != null)
                {
                    healthBar.value = currentHealth;
                    healthPoints.text = currentHealth.ToString("00");
                    healthBar_Fill.color = healthGradient.Evaluate(healthBar.normalizedValue);
                }
            }
        }

    }
    public void GetHealing(int health)
    {
        //heals
        currentHealth += health;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        //if camera effects applies values
        healthNormalized = (currentHealth / maxHealth);
        if (cameraEffects != null)
        {
            cameraEffects.CalculateVignette(healthNormalized);
            cameraEffects.CalculateSaturation(healthNormalized);
            cameraEffects.SetHealthState(1);
        }
        //If it has healthbar, sets it's values
        if (healthBar != null && healthPoints != null)
        {
            healthBar.value = currentHealth;
            healthPoints.text = currentHealth.ToString("00");
            healthBar_Fill.color = healthGradient.Evaluate(healthBar.normalizedValue);
        }
        if (healingSound != null)
        {
            Instantiate(healingSound, transform.position, transform.rotation);
        }

    }
    public int GetMaxHealth()
    {
        return maxHealth;
    }
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    public float GetCurrentHealth_Normalized()
    {
        return (currentHealth / maxHealth);
    }
    public void GetBossManager(BossHealthManager manager)
    {
        bossHpManager = manager;
    }
    public bool GetIsBoss()
    {
        return isBoss;
    }
    public void SetSpawner(Enemy_Spawner newSpawner)
    {
        spawner = newSpawner;
    }
    public void SetCanReceiveDamage(bool newValue)
    {
        canReceiveDamage = newValue;
    }
    public void SetCameraFX(CameraFX newFXCam)
    {
        StartCoroutine(WaitSetCameraFX(newFXCam));
    }
    public void SetRespawnPosition(Transform newRespawnPos)
    {
        respawnPosition = newRespawnPos;
    }
    public Enemy_Spawner GetSpawner()
    {
        return spawner;
    }
    //Add events externally to the list. Call this if you need to add new ones.
    public void Add_DeadEvents(List<Event> newEvents)
    {
        foreach (Event evt in newEvents)
        {
            if (!deathEvents.Contains(evt))
            {
                deathEvents.Add(evt);
            }
        }
    }
    //Externally trigger lose events when you want the player to lose (recommended when respawn is enabled)
    public void TriggerLoseEvents()
    {
        foreach (Event evt in loseEvents)
        {
            evt.DoEvent();
            List<GameObject> obj = new List<GameObject>();
            obj.Add(this.gameObject);
            evt.GetObjects(obj);
        }
    }
    private void HealingByTime()
    {
        //Auto heals if hasn't receive damage for some time
        if (healingByTime && !isDying)
        {
            if(currentHealth < maxHealth)
            {
                waitTimeToHealing -= Time.deltaTime;
                if (waitTimeToHealing <= 0)
                {
                    currentHealth += healingRate * Time.deltaTime;
                    //Stops healing if it reaches it's max health
                    if (currentHealth > maxHealth)
                    {
                        currentHealth = maxHealth;
                        waitTimeToHealing = resetWaitTime;
                    }
                    //If camera effects, applies values
                    healthNormalized = (currentHealth / maxHealth);
                    if (cameraEffects != null)
                    {
                        cameraEffects.CalculateVignette(healthNormalized);
                        cameraEffects.CalculateSaturation(healthNormalized);
                        cameraEffects.SetHealthState(1);
                    }
                    //If it has healthbar, sets it's values
                    if (healthBar!= null && healthPoints != null)
                    {
                        healthBar.value = (int)currentHealth;
                        healthPoints.text = currentHealth.ToString("00");
                        healthBar_Fill.color = healthGradient.Evaluate(healthBar.normalizedValue);
                    }
                }
            }
        }
    }
    #endregion
    #region Coroutines
    private IEnumerator StartDeath()
    {
        isDying = true;
        //If has death event, do it
        if (preRespawnEvents != null && preRespawnEvents.Count > 0 && canRespawn && isPlayer)
        {
            if (GetComponent<PlayerMovement>() != null)
            {
                GetComponent<PlayerMovement>().GetMovement().ChangeRun(false);
                GetComponent<PlayerMovement>().GetMovement().ChangeAiming(false);
                GetComponent<PlayerMovement>().enabled = false;
            }
            foreach (Event evt in preRespawnEvents)
            {
                evt.DoEvent();
                List<GameObject> obj = new List<GameObject>();
                obj.Add(this.gameObject);
                evt.GetObjects(obj);
            }

        }
        else if (deathEvents != null && deathEvents.Count > 0)
        {
            foreach (Event evt in deathEvents)
            {
                evt.DoEvent();
                List<GameObject> obj = new List<GameObject>();
                obj.Add(this.gameObject);
                if (spawner != null)
                {
                    obj.Add(spawner.gameObject);
                }
                evt.GetObjects(obj);
            }
        }
        if(spawner != null)
        {
            spawner.KillEnemy();
        } 
        //Death animation
        if (anim != null)
        {
            anim.Play("Death",2);
        } 
        //Stop showing health bar if has one
        if(healthBarCanvas!= null && healthBarCanvas.activeSelf)
        {
            healthBarCanvas.SetActive(false);
        }
        yield return new WaitForSeconds(deathAnimation_Time);
        if(cameraEffects != null)
        {
            cameraEffects.ResetColors();
        }
        StartCoroutine(DeathAnim());
    }
    private IEnumerator DeathAnim()
    {
        //Dissolve Effect
        float elapsed = 0f;
        float dissolveFull = 0f;
        if (dissolveDuration < 0)
        {
            dissolveDuration *= -1;
        }else if(dissolveDuration== 0)
        {
            dissolveDuration = 0.1f;
        }
        while(elapsed < dissolveDuration)
        {
            elapsed += Time.deltaTime;
            if(materials!= null && materials.Count > 0)
            {
                dissolveFull += (Time.deltaTime / dissolveDuration);
                foreach (Material mtl in materials)
                {
                    mtl.SetInt("_Dissolving", 1);
                    mtl.SetFloat("_Dissolve", dissolveFull);
                }
            }
            yield return null;
        }

        StartCoroutine(FinishDeath());
    }
    private IEnumerator FinishDeath()
    {
        //Wait time to destroy object completely
        yield return new WaitForSeconds(0.1f);
        if (destroyAfterDeath)
        {
            Destroy(gameObject);
        }
        if (canRespawn && isPlayer)
        {
            if(respawnPosition != null)
            {
                transform.position = respawnPosition.position;
                canReceiveDamage = false;
                if (materials != null && materials.Count > 0)
                {
                    foreach (Material mtl in materials)
                    {
                        mtl.SetInt("_Dissolving", 0);
                        mtl.SetFloat("_Dissolve", 0);
                    }
                }
                StartCoroutine(RespawnAnimation_Events());
            }
        }
    }

    private IEnumerator ReappearAnimation()
    {
        float elapsed = 0f;
        float dissolveFull = 1f;
        if (dissolveDuration < 0)
        {
            dissolveDuration *= -1;
        }
        else if (dissolveDuration == 0)
        {
            dissolveDuration = 0.1f;
        }
        while (elapsed < dissolveDuration)
        {
            elapsed += Time.deltaTime;
            if (materials != null && materials.Count > 0)
            {
                dissolveFull -= (Time.deltaTime / dissolveDuration);
                foreach (Material mtl in materials)
                {
                    mtl.SetInt("_Dissolving", 1);
                    mtl.SetFloat("_Dissolve", dissolveFull);
                }
            }
            yield return null;
        }
        if (materials != null && materials.Count > 0)
        {
            foreach (Material mtl in materials)
            {
                mtl.SetInt("_Dissolving", 0);
                mtl.SetFloat("_Dissolve", 1);
            }
        }
        //Shows health bar if has one
        if (healthBarCanvas != null && !healthBarCanvas.activeSelf)
        {
            healthBarCanvas.SetActive(true);
        }
    }
    private IEnumerator RespawnAnimation_Events()
    {
        isDying = false;
        if (respawnEvents != null && respawnEvents.Count > 0 && canRespawn)
        {
            foreach (Event evt in respawnEvents)
            {
                evt.DoEvent();
                List<GameObject> obj = new List<GameObject>();
                obj.Add(this.gameObject);
                evt.GetObjects(obj);
            }
        }
        StartCoroutine(ReappearAnimation());
        yield return new WaitForSeconds(respawnTime);
        GetHealing(999999);
        canReceiveDamage = true;
        if(anim != null)
        {
            anim.SetInteger("AnimVal", 0);
            anim.SetInteger("Gun_AnimVal", 0);
            anim.Play("Idle", 0);
            anim.Play("Default", 1);
        }
        if (GetComponent<PlayerMovement>() != null)
        {
            GetComponent<PlayerMovement>().enabled = true;
        }

    }
    private IEnumerator WaitSetCameraFX(CameraFX newFXCam)
    {
        yield return new WaitForSeconds(0.1f);
        healthNormalized = 1;
        cameraEffects = newFXCam;
        if (cameraEffects != null)
        {
            cameraEffects.CalculateVignette(healthNormalized);
            cameraEffects.CalculateSaturation(healthNormalized);
        }
    }

    
    #endregion
}
