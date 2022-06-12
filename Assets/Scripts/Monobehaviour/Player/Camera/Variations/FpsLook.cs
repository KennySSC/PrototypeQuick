using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsLook : BaseCameraMove
{
    #region Serialized variables

    [Header("Requiered components")]


    [Tooltip("The player main body")]
    [SerializeField] Transform playerBody;

    [Tooltip("Player's camera")]
    [SerializeField] GameObject cam;

    [Tooltip("Adds effects to the camera. If you don't need it leave it empty")]
    [SerializeField] CameraFX cameraEffects;


    [Space]


    [Header("Camera look settings")]


    [SerializeField] private float mouseSensitivityX;

    [SerializeField] private float mouseSensitivityY;

    [SerializeField] private float maxLookYAngle;

    [SerializeField] private float minLookYAngle;

    [Tooltip("Turn on to hide mouse pointer and lock it")]
    [SerializeField] private bool lockMouse = true;

    #endregion

    #region Private variables

    private float mouseX;

    private float mouseY;

    private float lookX;

    private float lookY;

    private float camYRotation;

    #endregion

    #region Main Functions

    public override void Look(Vector2 look)
    {
        mouseX = look.x;
        mouseY = look.y;
        //Multiplies the values sent by the mouse and the sensivity, the fps given by the pc doesn't matter
        lookX = mouseX * mouseSensitivityX * Time.deltaTime;
        lookY = mouseY * mouseSensitivityY * Time.deltaTime;

        //Rotates the body of the player on x
        playerBody.Rotate(Vector3.up, lookX);
        //Rotates only the camera on y
        camYRotation -= lookY;
        camYRotation = Mathf.Clamp(camYRotation, minLookYAngle, maxLookYAngle);
        Vector3 targetRotation = transform.eulerAngles;
        targetRotation.x = camYRotation;
        cam.transform.eulerAngles = targetRotation;
    }
    #endregion

    #region Get Set
    public override GameObject GetCamera()
    {
        return cam;
    }
    public override void OnChangeSensitivityX(float newXSensitivity)
    {
        mouseSensitivityX = newXSensitivity;
    }
    public override void OnChangeSensitivityY(float newYSensitivity)
    {
        mouseSensitivityY = newYSensitivity;
    }
    public override bool GetLockMouse()
    {
        return lockMouse;
    }
    public override CameraFX GetCameraFX()
    {
        return cameraEffects;
    }
    #endregion
}
