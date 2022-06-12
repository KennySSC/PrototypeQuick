using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjectSpawn : Event
{
    #region Serializable Variables
    [Tooltip("The GameObjects to spawn")]
    [SerializeField] List<GameObject> objectsToSpawn;

    [SerializeField] Transform spawnPosition;

    [Tooltip("One number for each objet. It can be any integer above 0. If the number is less than 0, it will become positive. " +
        "If the weights list it's shorter than objects list, the exceding objects will have a 5% chance to spawn. " +
        "If the weights list it's longer than objects list, those weights will be ignored" +
        "The greater the number, the more probability to spawn. Greater numbers will result in more randomness but less performant. " +
        "If the weight it's set to 0, the item with the same index will be removed from the list")]
    [SerializeField] List<int> weights;

    [Tooltip("The times that a random number will be calculated to spawn a item")]
    [SerializeField] int instantiateAttempts = 1;

    [Tooltip("If greater than 0, there is a chance for nothing to spawn")]
    [SerializeField] int SpawnNothingWeight = 0;

    #endregion }

    #region Private variables

    private int inferiorLimit = 0;
    private int superiorLimit = 0;
    private int randomNumber = 0;
    private int totalWeight = 0;

    #endregion

    #region Main Functions
    void Start()
    {
        //If one of this conditions applies, there will not be spawning anything
        if (objectsToSpawn != null || objectsToSpawn.Count > 0 || instantiateAttempts > 0)
        {
            //Verifies if weights list is shorter than the objects list, if not it fills the list with a 5% of the total of the weights list 
            if (weights.Count < objectsToSpawn.Count)
            {
                int tempTotalWeight = 0;
                for(int i = 0; i<weights.Count; i++)
                {
                    tempTotalWeight += weights[i];
                }
                tempTotalWeight += SpawnNothingWeight;
                if (tempTotalWeight > 0)
                {
                    int tempNewWeight = tempTotalWeight / 20;
                    int i = weights.Count + 1;
                    do
                    {
                        weights.Add(tempNewWeight);
                    } while (i < objectsToSpawn.Count);
                }
                else
                {
                    return;
                }
                for(int i = 0; i<weights.Count; i++)
                {
                    if(weights[i] < 0)
                    {
                        weights[i] *= -1;
                    }
                }
                int y = objectsToSpawn.Count;
                //If the weight it's 0, it will remove the object with that weight and the weight itself
                do
                {
                    if (weights[y] == 0)
                    {
                        weights.RemoveAt(y);
                        objectsToSpawn.RemoveAt(y);
                    }
                    y--;
                } while (y >= 0);

            }
            foreach(int x in weights)
            {
                totalWeight += x;
            }
            totalWeight += SpawnNothingWeight;
            if (SpawnNothingWeight > 0)
            {
                weights.Add(SpawnNothingWeight);
            }

        }
    }

    public void SpawnRandom(Transform spawnPosition)
    {
        //Calculates a random weight number and spawns an object or not if the result is in the spawnNothing weight range
        if (instantiateAttempts > 0)
        {
            int x = 0;
            do
            {
                randomNumber = Random.Range(0, totalWeight);
                for(int i = 0; i<weights.Count; i++)
                {
                    if(i == 0)
                    {
                        inferiorLimit = 0;
                    }
                    else
                    {
                        for (int z = i-1; z >= 0; z--)
                        {
                            int tempAdd = weights[z];
                            inferiorLimit += tempAdd;
                        }
                    }
                    superiorLimit = inferiorLimit + weights[i];
                    if(randomNumber > inferiorLimit && randomNumber <=superiorLimit && i<objectsToSpawn.Count)
                    {
                        Instantiate(objectsToSpawn[i], spawnPosition.position, spawnPosition.rotation);
                        i = objectsToSpawn.Count;
                    }
                }
                x++;
            } while (x < instantiateAttempts);
        }
    }
    public override void DoEvent()
    {
        SpawnRandom(spawnPosition);
    }
    public override void GetObjects(List<GameObject> newObjects)
    {
        
    }


    #endregion
}
