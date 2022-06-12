using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[CreateAssetMenu(fileName = "BasicAim", menuName = "Scriptable Objects/Aim types/ Basic aim", order = 0)]
public class BasicAim : BaseAim
{
    #region Serializable Variables
    [Tooltip("The fov that the camera will have while aiming if fps")]
    [SerializeField] float zoomInFovFps;

    [Tooltip("The fov that the camera will have while aiming if tps")]
    [SerializeField] float zoomInFovTps;

    #endregion

    #region private variables

    private float normalFov = 1f;

    private int index = 0;

    #endregion

    #region Main Functions
    public override void StartReset(List<Renderer> playerRenderers)
    {
        
    }
    //Applies the cam fov
    public override void Aim(GameObject cam)
    {
        if (cam.GetComponent<CinemachineFreeLook>() != null)
        {
            cam.GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView = zoomInFovTps;
        }
        else if (cam.GetComponent<Camera>() != null)
        {
            cam.GetComponent<Camera>().fieldOfView = zoomInFovFps;
        }
    }
    //Resets the cam fov
    public override void CancelAim(GameObject cam)
    {
        if(cam.GetComponent<CinemachineFreeLook>()!= null)
        {
            cam.GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView = normalFov;
        }
        else if (cam.GetComponent<Camera>() != null)
        {
            cam.GetComponent<Camera>().fieldOfView = normalFov;
        }

    }
    //Get the camera default fov
    public override void BaseFov(float fov)
    {
        normalFov = fov;
    }

    #endregion

    #region Get Set

    public override void SightObject(GameObject sightObj)
    {
        
    }
    public override void GetRenderers(List<Renderer> playerRenderers, List<Renderer> gunRenderers)
    {
        
    }
    public override int GetAimType_Index()
    {
        return index;
    }

    #endregion
}
