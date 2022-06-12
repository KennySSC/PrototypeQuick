using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicOclussion : MonoBehaviour
{
    //This script culls a not static object to reduce performance cost.
    //Recommended for animated skinned characters.
    //If used in simple objects it could be less performant than not using this script.

    #region Private variables

    private List<Renderer> renderers = new List<Renderer>();
    #endregion
    private void Awake()
    {
        //Set not UI renderers
        if (GetComponent<MeshRenderer>() != null)
        {
            Renderer[] tempRenderers = GetComponents<MeshRenderer>();
            foreach(Renderer rnd in tempRenderers)
            {
                if (!renderers.Contains(rnd))
                {
                    renderers.Add(rnd);
                }
            }
        }
        if (GetComponentInChildren<MeshRenderer>() != null)
        {
            Renderer[] tempRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (Renderer rnd in tempRenderers)
            {
                if (!renderers.Contains(rnd))
                {
                    renderers.Add(rnd);
                }
            }
        }
        if (GetComponentInParent<MeshRenderer>() != null)
        {
            Renderer[] tempRenderers = GetComponentsInParent<MeshRenderer>();
            foreach (Renderer rnd in tempRenderers)
            {
                if (!renderers.Contains(rnd))
                {
                    renderers.Add(rnd);
                }
            }
        }
        
        if (GetComponent<SkinnedMeshRenderer>() != null)
        {
            Renderer[] tempRenderers = GetComponents<SkinnedMeshRenderer>();
            foreach (Renderer rnd in tempRenderers)
            {
                if (!renderers.Contains(rnd))
                {
                    renderers.Add(rnd);
                }
            }
        }
        if (GetComponentInChildren<SkinnedMeshRenderer>() != null)
        {
            Renderer[] tempRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (Renderer rnd in tempRenderers)
            {
                if (!renderers.Contains(rnd))
                {
                    renderers.Add(rnd);
                }
            }
        }
        if (GetComponentInParent<SkinnedMeshRenderer>() != null)
        {
            Renderer[] tempRenderers = GetComponentsInParent<SkinnedMeshRenderer>();
            foreach (Renderer rnd in tempRenderers)
            {
                if (!renderers.Contains(rnd))
                {
                    renderers.Add(rnd);
                }
            }
        }
    }

    //Activates renderers a camera sees it
    private void OnBecameVisible()
    {
        foreach (Renderer rnd in renderers)
        {
            rnd.enabled = true;
        }
    }
    //Desactivates rendereres when no camera sees it
    private void OnBecameInvisible()
    {
        foreach (Renderer rnd in renderers)
        {
            rnd.enabled = false;
        }
    }
}
