using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappearing_Platform : MonoBehaviour
{
    #region Serializable variables

    [Tooltip("The time that the platform will stay normal")]
    [SerializeField] float appearTime;

    [Tooltip("Time that the player has to leave the platform before disappearing")]
    [SerializeField] float flickeringTime;

    [Tooltip("The time that the platform will be desactivated")]
    [SerializeField] float disappearTime;

    [Tooltip("Off = Initial state of the platform is activated; On = Initial state of the platform is desactivated")]
    [SerializeField] bool startDisappeared;

    #endregion

    #region Private Variables

    private Collider platformCollider;
    private List<Renderer> renderers = new List<Renderer>();
    private Renderer[] tempRenderer;

    #endregion

    #region Main Functions

    void Start()
    {
        //Set variables
        platformCollider = GetComponent<Collider>();

        //Get all platform renderes
        tempRenderer = GetComponents<Renderer>();
        if(tempRenderer != null)
        {
            foreach(Renderer rnd in tempRenderer)
            {
                if (!renderers.Contains(rnd))
                {
                    renderers.Add(rnd);
                }
            }
        }
        tempRenderer = GetComponentsInChildren<Renderer>();
        if (tempRenderer != null)
        {
            foreach (Renderer rnd in tempRenderer)
            {
                if (!renderers.Contains(rnd))
                {
                    renderers.Add(rnd);
                }
            }
        }
        tempRenderer = GetComponentsInParent<Renderer>();
        if (tempRenderer != null)
        {
            foreach (Renderer rnd in tempRenderer)
            {
                if (!renderers.Contains(rnd))
                {
                    renderers.Add(rnd);
                }
            }
        }
        //Set initial state
        if (startDisappeared)
        {
            StartCoroutine(DisappearTime());
        }
        else
        {
            StartCoroutine(AppearTime());
        }

    }

    #endregion

    #region Coroutines
    IEnumerator AppearTime()
    {
        //Activates the renderers and collider
        foreach (Renderer rnd in renderers)
        {
            rnd.enabled = true;
        }
        platformCollider.enabled = true;
        yield return new WaitForSeconds(appearTime);
        StartCoroutine(Flickering());
    }
    IEnumerator Flickering()
    {
        //Turns off and on the renderers to flickering effect
        float elapsed = 0;
        while (elapsed < flickeringTime)
        {
            elapsed += Time.deltaTime;
            int random = Random.Range(0, 101);
            if (random < 51)
            {
                foreach(Renderer rnd in renderers)
                {
                    rnd.enabled = false;
                }
            }
            else if(random>51)
            {
                foreach (Renderer rnd in renderers)
                {
                    rnd.enabled = true;
                }
            }
            yield return null;
        }
        StartCoroutine(DisappearTime());

    }
    IEnumerator DisappearTime()
    {
        //Desactivate renderers and collider
        foreach (Renderer rnd in renderers)
        {
            rnd.enabled = false;
        }
        platformCollider.enabled = false;
        yield return new WaitForSeconds(disappearTime);
        StartCoroutine(AppearTime());
    }

    #endregion
}
