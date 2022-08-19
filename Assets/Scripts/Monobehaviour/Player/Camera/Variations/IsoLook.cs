using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoLook : BaseCameraMove
{
    //You can use this script with a raw image and a render texture in the UI

    //For creating the mini map, use a raw image component and place it in your canvas. Then go to the project folders inside unity, right click and search for "RenderTexture" to create one
    //When you select the render texture you can change it's size, it should match the RawImage size of the canvas. The camera component has a slot in the output menu labeled "Output Texture"
    //you must drag the render texture there. Now, in the canvas raw image in the slot "Texture" you mus drag the render.
    //In the test, the minimap only worked by setting the camera component, environment tab in "solid color", otherwhise it only renders UI and particle elements.
    #region Serializable Variables

    [Tooltip("Game object with camera component")]
    [SerializeField] GameObject cam;

    [Tooltip("The camera must be inside a empty GameObject for the camera shake to work properly. (If used for minimap you can set the camera object here)")]
    [SerializeField] GameObject cameraContainer;

    [Tooltip("Player's body (needed)")]
    [SerializeField] Transform player;

    [Tooltip("Object that the camera will follow. (If used for a minimap, the player should be the target)")]
    [SerializeField] Transform target;

    [Tooltip("Offset from the target to the camera position (If used for minimap, the Y value is the aprox FOV in meters of the map")]
    [SerializeField] Vector3 offset;

    [Tooltip("Smooth movement. If you put 0 the camera will not move. 1 Means no smoothing (1 is recommended for minimap)")]
    [SerializeField] float smooth;

    [Tooltip("Turn on to hide mouse pointer and lock it (False is recommended for minimap)")]
    [SerializeField] private bool lockMouse = true;

    [Tooltip("Adds effects to the camera. If you don't need it leave it empty (Leave it empty for minimap)")]
    [SerializeField] CameraFX cameraEffects;

    #endregion

    #region Main Functions
    private void Start()
    {
        //Unparents the camera from any parent
        //Follows player

        cameraContainer.transform.parent = null;
        if (player != null)
        {
        cameraContainer.transform.LookAt(player);
        cameraContainer.transform.position = player.position;
        }
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
