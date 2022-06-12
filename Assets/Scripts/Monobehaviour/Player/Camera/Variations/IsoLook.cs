using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoLook : BaseCameraMove
{
    #region Serializable Variables

    [Tooltip("Game object with camera component")]
    [SerializeField] GameObject cam;
    [SerializeField] GameObject cameraContainer;

    [Tooltip("Player's body")]
    [SerializeField] Transform player;

    [Tooltip("Object that the camera will follow")]
    [SerializeField] Transform target;

    [Tooltip("Offset from the target to the camera position")]
    [SerializeField] Vector3 offset;

    [Tooltip("Smooth movement")]
    [SerializeField] float smooth;

    [Tooltip("Turn on to hide mouse pointer and lock it")]
    [SerializeField] private bool lockMouse = true;

    [Tooltip("Adds effects to the camera. If you don't need it leave it empty")]
    [SerializeField] CameraFX cameraEffects;

    #endregion

    #region Main Functions
    private void Start()
    {
        //Unparents the camera from any parent
        //Follows plater
        cameraContainer.transform.LookAt(player);
        cameraContainer.transform.parent = null;
        cameraContainer.transform.position = player.position;
    }
    private void Update()
    {
        //look at player smoothly
        Vector3 lookPos = player.position - cameraContainer.transform.position;
        lookPos.x = 0;
        lookPos.z = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        cameraContainer.transform.rotation = Quaternion.Slerp(cameraContainer.transform.rotation, rotation, (2 * Time.deltaTime));


        //follows player smoothly
        Vector3 originalPos = cameraContainer.transform.position;
        Vector3 targetPos = Vector3.Lerp(originalPos, target.position + offset, smooth);
        cameraContainer.transform.position = targetPos;
    }
    public override void Look(Vector2 look)
    {
        //Rotates the player 
        Vector3 mouseWorldPos = Camera.main.WorldToViewportPoint(player.position);
        Vector3 mouseScreen = (Vector2)Camera.main.ScreenToViewportPoint(look);

        Vector3 lookAtDir = mouseScreen - mouseWorldPos;

        float angle = Mathf.Atan2(lookAtDir.y, lookAtDir.x) * Mathf.Rad2Deg - 90f;
        player.rotation = Quaternion.Euler(new Vector3(0, -angle, 0));


    }

    #endregion

    #region Get Set

    public override GameObject GetCamera()
    {
        return cam;
    }
    public override void OnChangeSensitivityX(float newXSensitivity)
    {
        
    }
    public override void OnChangeSensitivityY(float newYSensitivity)
    {

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
