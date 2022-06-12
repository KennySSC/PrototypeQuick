using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TpsLook : BaseCameraMove
{
    #region Serializable Variables

    [Tooltip("Player's camera")]
    [SerializeField] CinemachineFreeLook freeLookCam;

    [Tooltip("Turn on to hide mouse pointer and lock it")]
    [SerializeField] private bool lockMouse = true;

    [Tooltip("Base horizontal sensitivity. It will override the cinemachine free look cam values. Think it as the value that is applied when the slider value in the settings is 1")]
    [SerializeField] private float sensivityX = 20f;

    [Tooltip("Base vertical sensitivity. It will override the cinemachine free look cam values. Think it as the value that is applied when the slider value in the settings is 1")]
    [SerializeField] private float sensivityY = 0.15f;

    [Tooltip("Adds effects to the camera. If you don't need it leave it empty")]
    [SerializeField] CameraFX cameraEffects;

    #endregion

    #region Private Variables



    #endregion

    #region Main Functions
    private void Awake()
    {

    }
    private void Start()
    {

    }
    public override void Look(Vector2 look)
    {
        
    }

    #endregion

    #region Get Set

    public override GameObject GetCamera()
    {
        return freeLookCam.gameObject;
    }
    public override void OnChangeSensitivityX(float newXSensitivity)
    {
        if (freeLookCam != null)
        {
            freeLookCam.m_XAxis.m_MaxSpeed = sensivityX * newXSensitivity;
        }
    }
    public override void OnChangeSensitivityY(float newYSensitivity)
    {
        if (freeLookCam != null)
        {
            freeLookCam.m_YAxis.m_MaxSpeed = sensivityY * newYSensitivity;
        }
    }
    public override bool GetLockMouse()
    {
        return lockMouse;
    }
    public override CameraFX GetCameraFX()
    {
        return cameraEffects;
    }
    public void TempValuesTPS()
    {

    }
    #endregion
}
