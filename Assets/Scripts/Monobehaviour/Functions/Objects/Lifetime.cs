using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    #region Serializable variables

    [Tooltip("Time before the game object that has this script will be destroyed")]
    [SerializeField] float lifeTime;

    [Tooltip("Events that trigger once the object life reaches 0. If you don't need them leave it empty")]
    [SerializeField] List<Event> deathEvents = new List<Event>();

    #endregion

    #region Main Functions

    private void Start()
    {
        StartCoroutine(DestroyMySelf());
    }

    #endregion

    #region Coroutines

    IEnumerator DestroyMySelf()
    {
        yield return new WaitForSeconds(lifeTime);
        foreach(Event evt in deathEvents)
        {
            evt.DoEvent();
        }
        Destroy(gameObject);
    }

    #endregion
}
