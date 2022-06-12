using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCameraMove : MonoBehaviour
{
    //Base functions of a camera look
    public abstract void Look(Vector2 look);
    public abstract GameObject GetCamera();

    public abstract void OnChangeSensitivityX(float newSensitivityX);
    public abstract void OnChangeSensitivityY(float newSensitivityY);
    public abstract bool GetLockMouse();
    public abstract CameraFX GetCameraFX();
}
