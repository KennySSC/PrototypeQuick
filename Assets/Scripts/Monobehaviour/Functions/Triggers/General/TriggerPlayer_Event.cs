using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TriggerPlayer_Event : MonoBehaviour
{
    #region Serializable Variables
    [Tooltip("Events that will execute when a player enter to this trigger")]
    [SerializeField] Event[] events;

    [Tooltip("Off = do events once; On = do events each time a player enters to the trigger")]
    [SerializeField] bool doItInfinite = false;

    #endregion

    #region Main Functions
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            foreach (Event evt in events)
            {
                evt.DoEvent();
            }
            if (!doItInfinite)
            {
                if (gameObject.activeSelf)
                {
                    gameObject.SetActive(false);
                }
            }
        }

    }
    #endregion
}
