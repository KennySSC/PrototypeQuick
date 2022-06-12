using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Procedural_Manager : MonoBehaviour
{
    #region Serializable Variables
    [Header("Main Settings")]


    [Tooltip("All mini level prefabs that you want to use. All of them must have the same z size and have its pivot on the center for a correct positioning")]
    [SerializeField] GameObject[] prefabs;

    [Tooltip("z size of the prefabs")]
    [SerializeField] int prefabSize;

    [Tooltip("Spawns at the start an amount of prefabs")]
    [SerializeField] int initialBurst;

    [Tooltip("The max mini levels that can be at once. If one more spawns, the first one will be destroyed. It should be less than the initial burst")]
    [SerializeField] int maxPieces_AtOnce;


    [Space]


    [Header("UI Settings")]


    [Tooltip("Text that will be displayed to the player indicating how many mini levels have been completed")]
    [SerializeField] TMP_Text points_Text;


    [Space]


    [Header("Sound")]


    [Tooltip("It will perform each time the player get a point. It should have infinite range. If you don't need it, leave it empty")]
    [SerializeField] GameObject pointSound;

    [Tooltip("It will perform each time a mini level is destroyed. It should have double range than prefab size. If you don't need it, leave it empty")]
    [SerializeField] GameObject destroyMiniLevelSound;

    #endregion

    #region private Variables

    List<GameObject> currentList = new List<GameObject>();
    private int currentPoints = 0;

    #endregion

    #region Main Functions

    void Start()
    {
        //Spawn the initial burst of prefabs
        if (initialBurst > maxPieces_AtOnce)
        {
            initialBurst = maxPieces_AtOnce;
        }
        for(int i = 0; i<initialBurst; i++)
        {
            SpawnOther();
        }
        UpdateText();
    }
    #endregion

    #region Get Set

    private void UpdateText()
    {
        points_Text.text = "Points: " +currentPoints.ToString();
    }

    public void SpawnOther()
    {
        //Spawn a new prefab and verifies that the current ones are less than the max. If there are more, it destroys the first one
        int random = Random.Range(0, prefabs.Length);
        if (currentList.Count > 0)
        {
            GameObject obj = Instantiate(prefabs[random], (currentList[currentList.Count -1].transform.position), transform.rotation);
            obj.transform.position = obj.transform.position + new Vector3(0, 0, prefabSize);
            currentList.Add(obj);
            if (currentList.Count > maxPieces_AtOnce)
            {
                GameObject temporalToDestroy = currentList[0];
                currentList.RemoveAt(0);
                if(destroyMiniLevelSound!= null)
                {
                    Instantiate(destroyMiniLevelSound, temporalToDestroy.transform.position, temporalToDestroy.transform.rotation);
                }
                Destroy(temporalToDestroy);
            }
            
        }
        else
        {
            GameObject obj = Instantiate(prefabs[random], transform.position, transform.rotation);
            currentList.Add(obj);
        }
    }
    //Call this to indicate that one mini level has been completed
    public void AddPoints(int points)
    {
        currentPoints += points;
        if(pointSound != null)
        {
            Instantiate(pointSound, transform.position, transform.rotation);
        }
        UpdateText();
    }

    #endregion 
}
