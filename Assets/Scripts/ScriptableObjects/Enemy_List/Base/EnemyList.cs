using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_List", menuName = "Scriptable Objects/Enemy List", order = 3)]
public class EnemyList : ScriptableObject
{
    #region Serializable variables

    [Tooltip("Group of enemies that can be assigned to a spawner. You can repeat enemies to increase the chance or spawn multiple times one enemy")]
    [SerializeField] List<GameObject> enemies = new List<GameObject>();

    #endregion

    #region Get Set
    public List<GameObject> GetEnemyList()
    {
        return enemies;
    }
    #endregion 
}
