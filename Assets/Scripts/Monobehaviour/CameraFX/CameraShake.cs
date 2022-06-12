using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public void DoShakeCam(float duration, float magnitude, GameObject cameraObj, bool isTps)
    {
        StartCoroutine(ShakeCam(duration, magnitude, cameraObj, isTps));
    }
    private IEnumerator ShakeCam(float duration, float magnitude, GameObject cameraObj, bool isTps)
    {
        Vector3 originalPos = cameraObj.transform.localPosition;
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
                cameraObj.transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            }
            elapsed += Time.deltaTime;

            yield return null;
        }
        if (!isTps)
        {
            cameraObj.transform.localPosition = originalPos;
        }
    }
}
