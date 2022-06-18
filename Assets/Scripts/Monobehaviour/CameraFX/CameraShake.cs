using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] Transform cameraObject;

    private Vector3 originalPosition;
    private void Start()
    {
        originalPosition = cameraObject.transform.localPosition;
    }
    public void DoShakeCam(float duration, float magnitude, GameObject cameraObj, bool isTps)
    {
        StartCoroutine(ShakeCam(duration, magnitude, cameraObj, isTps));
    }
    private IEnumerator ShakeCam(float duration, float magnitude, GameObject cameraObj, bool isTps)
    {
        
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            if (isTps)
            {
                cameraObj.transform.localPosition = new Vector3(cameraObj.transform.localPosition.x + x, cameraObj.transform.localPosition.y + y, cameraObj.transform.localPosition.z);
            }
            else
            {
                cameraObj.transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);
            }
            elapsed += Time.deltaTime;

            yield return null;
        }
        if (!isTps)
        {
            cameraObj.transform.localPosition = originalPosition;
        }
    }
}
