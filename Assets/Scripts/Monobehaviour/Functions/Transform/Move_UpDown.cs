using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StartPosition_Vertical { up,down, middle }
public class Move_UpDown : MonoBehaviour
{
    #region Serializable Variables


    [Tooltip("Speed in units per second for the object to move")]
    [SerializeField] float speed;

    [Tooltip("Maximum units for the object to move from the center (it's initial position)")]
    [SerializeField] float moveUnits;

    [Tooltip("Starting position of the object path")]
    [SerializeField] StartPosition_Vertical _startPosition;

    #endregion

    #region private variables

    private bool upDown = true;

    private float currentYMove;

    private float speedForPlayer = 0;

    #endregion

    #region Main Functions

    void Start()
    {
        //Set initial position
        if (_startPosition == StartPosition_Vertical.up)
        {
            transform.position = new Vector3(transform.position.x, (transform.position.y+moveUnits), transform.position.z);
            upDown = false;
            currentYMove = moveUnits;
        }
        else if (_startPosition == StartPosition_Vertical.down)
        {
            transform.position = new Vector3(transform.position.x, (transform.position.y-moveUnits), transform.position.z);
            upDown = true;
            currentYMove = (-moveUnits);
        }
        else
        {
            upDown = true;
            currentYMove = 0;
        }
    }
    void Update()
    {
        //Move the object and change direction
        if (upDown && currentYMove < moveUnits)
        {
            float temp = speed * Time.deltaTime;
            currentYMove += temp;
            transform.position = new Vector3(transform.position.x , (transform.position.y+temp), transform.position.z);
            speedForPlayer = temp;
            if (currentYMove >= moveUnits)
            {
                upDown = false;
            }
        }
        else if (!upDown && currentYMove > (-moveUnits))
        {
            float temp = speed * Time.deltaTime;
            currentYMove -= temp;
            speedForPlayer = temp;
            transform.position = new Vector3(transform.position.x, (transform.position.y-temp), transform.position.z);
            if (currentYMove <= (-moveUnits))
            {
                upDown = true;
            }
        }
    }

    #endregion

    #region Get Set

    public float GetSpeed()
    {
        return speedForPlayer;
    }

    #endregion
}
