using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StartPosition_Horizontal {left, right, middle} 
public class Move_LeftRight : MonoBehaviour
{
    #region Serializable Variables

    [Tooltip("Speed in units per second for the object to move")]
    [SerializeField] float speed;

    [Tooltip("Maximum units for the object to move from the center (it's initial position)")]
    [SerializeField] float moveUnits;

    [Tooltip("Starting position of the object path")]
    [SerializeField] StartPosition_Horizontal _startPosition;

    #endregion

    #region private variables

    private bool leftRight = true;

    private float currentXMove;

    private float speedForPlayer = 0;
    #endregion

    #region Main Functions

    void Start()
    {
        //Set starting position
        if (_startPosition == StartPosition_Horizontal.left)
        {
            transform.position = new Vector3((transform.position.x - moveUnits), transform.position.y, transform.position.z);
            leftRight = true;
            currentXMove = (-moveUnits);
        }else if(_startPosition == StartPosition_Horizontal.right)
        {
            transform.position = new Vector3((transform.position.x + moveUnits), transform.position.y, transform.position.z);
            leftRight = false;
            currentXMove = moveUnits;
        }
        else
        {
            leftRight = true;
            currentXMove = 0;
        }
    }
    void Update()
    {
        //Move the platform and change direction
        if (leftRight && currentXMove<moveUnits)
        {
            float temp = speed * Time.deltaTime;
            speedForPlayer = temp;
            currentXMove += temp;
            transform.position = new Vector3((transform.position.x + temp), transform.position.y, transform.position.z);
            if(currentXMove>= moveUnits)
            {
                leftRight = false;
            }
        }else if(!leftRight && currentXMove > (-moveUnits))
        {
            float temp = speed * Time.deltaTime;
            currentXMove -= temp;
            speedForPlayer = -temp;
            transform.position = new Vector3((transform.position.x - temp), transform.position.y, transform.position.z);
            if (currentXMove <= (-moveUnits))
            {
                leftRight = true;
            }
        }
    }

    #endregion

    #region Get Set

    public float GetSpeed()
    {
        return speedForPlayer*1.2f;
    }

    #endregion
}
