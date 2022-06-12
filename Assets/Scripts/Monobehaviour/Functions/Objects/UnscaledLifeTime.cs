using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnscaledLifeTime : MonoBehaviour
{
    //Used when the game is freezed and you need to destroy something
    #region Serializable variables

    [Tooltip("Time before the game object that has this script will be destroyed")]
    [SerializeField] float lifeTime;

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
        float elapsed = 0f;
        while(elapsed < lifeTime)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    #endregion
}
