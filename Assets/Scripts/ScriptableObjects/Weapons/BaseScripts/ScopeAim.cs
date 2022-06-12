using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[CreateAssetMenu(fileName = "ScopeAim", menuName = "Scriptable Objects/Aim types/ Scope aim", order = 1)]
public class ScopeAim : BaseAim
{
    #region Serialized Variables

    [Tooltip("The fov that the camera will have while aiming if fps")]
    [SerializeField] float zoomInFovFps;

    [Tooltip("The fov that the camera will have while aiming if tps")]
    [SerializeField] float zoomInFovTps;

    #endregion

    #region Private variables
    private GameObject sight;
    private List<Renderer> renderers = new List<Renderer>();
    private BaseGun currentGun;

    private float normalFov = 1f;

    private int index = 1;

    #endregion

    #region Main Functions

    public override void StartReset(List<Renderer> playerRenderers)
    {
        //If has a sight set, it will hide it
        if (sight != null && sight.activeSelf)
        {
            sight.SetActive(false);
        }
        //If has the player renderers set, it will show them
        foreach(Renderer render in playerRenderers)
        {
            if (render!= null && render.enabled == false)
            {
                render.enabled = true;
            }
        }
        currentGun = FindObjectOfType<BaseGun>();
    }
    public override void Aim(GameObject cam)
    {
        //Applies the fov and show the scope canvas
        if (cam.GetComponent<CinemachineFreeLook>() != null)
        {
            cam.GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView = zoomInFovTps;
        } else if (cam.GetComponent<Camera>() != null)
        {
            cam.GetComponent<Camera>().fieldOfView = zoomInFovFps;
        }
        if(sight != null && !sight.activeSelf)
        {
            sight.SetActive(true);
        }
        if(renderers != null && renderers.Count > 0)
        {
            foreach(Renderer render in renderers)
            {
                if (render!= null && render.enabled == true)
                {
                    render.enabled = false;
                }
            }
        }
    }
    public override void CancelAim(GameObject cam)
    {
        //Reset the fov and hide the scope canvas
        if (cam.GetComponent<CinemachineFreeLook>() != null)
        {
            cam.GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView = normalFov;
        }
        else if (cam.GetComponent<Camera>() != null)
        {
            cam.GetComponent<Camera>().fieldOfView = normalFov;
        }
        if (sight != null && sight.activeSelf)
        {
            sight.SetActive(false);
        }
        if (renderers != null && renderers.Count > 0)
        {
            foreach (Renderer render in renderers)
            {
                if (render!= null && render.enabled == false)
                {
                    render.enabled = true;
                }
            }
        }
    }

    #endregion 

    #region Get Set
    //Set the default camera fov
    public override void BaseFov(float fov)
    {
        normalFov = fov;
    }
    //Set the scope canvas
    public override void SightObject(GameObject sightObj)
    {
        sight = sightObj;
    }
    //Set the player and gun renderers
    public override void GetRenderers(List<Renderer> playerRenderers, List<Renderer> gunRenderers)
    {
        if(playerRenderers != null || playerRenderers.Count>0)
        {
            foreach (Renderer render in playerRenderers)
            {
                if (!renderers.Contains(render))
                {
                    renderers.Add(render);
                }
            }
        }
        if(gunRenderers != null || gunRenderers.Count > 0)
        {
            foreach (Renderer rndr in gunRenderers)
            {
                if (!renderers.Contains(rndr))
                {
                    renderers.Add(rndr);
                }
            }
        }
    }
    public override int GetAimType_Index()
    {
        return index;
    }
    #endregion 
}
