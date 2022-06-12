using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTextureOffset : MonoBehaviour
{
    #region Serializable Variables

    [Header("Main Settings")]

    [Tooltip("Speed of the movement in units per second")]
    [SerializeField] Vector2 speed;


    [Space]


    [Header("Debug variables")]

    [Tooltip("The material intance that will move")]
    [SerializeField] Material mtl;


    #endregion

    #region Main Functions

    private void Start()
    {
        mtl = GetComponent<MeshRenderer>().material;
    }
    void Update()
    {
        mtl.mainTextureOffset += (speed * Time.deltaTime);
    }

    #endregion
}
