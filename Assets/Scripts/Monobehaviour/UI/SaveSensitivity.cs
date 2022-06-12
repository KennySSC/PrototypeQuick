using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSensitivity : MonoBehaviour
{
    #region Serializable Variables

   [Header("Main Settings")]

    [Tooltip("Slider that changes sensitivity values")]
    [SerializeField] Slider sld;

    [Tooltip("Shows via text the slider values in ints. If you don't need it leave it empty")]
    [SerializeField] TMP_Text tmpText;

    [Tooltip("Shows via text the slider values in ints, use it if you aren't using TMPro. If you don't need it leave it empty")]
    [SerializeField] Text txt;

    [Tooltip("Name added of the saved variables")]
    [SerializeField] string saveName;

    [Tooltip("Changes x sensitivity or y. Off = x; On = Y")]
    [SerializeField] bool XorY;


    [Space]


    [Header("Debug variables")]


    [Tooltip("The detected base camera moves. These are the ones that its sensitivity will be changed")]
    [SerializeField]BaseCameraMove[] cameraMove;


    TpsLook tempTps;
    #endregion

    #region Main Functions
    //Sets saved sensitivity
    private void Awake()
    {
        tempTps = FindObjectOfType<TpsLook>();
        if(tempTps!= null)
        {
            tempTps.TempValuesTPS();
        }
        cameraMove = FindObjectsOfType<BaseCameraMove>();
        sld.value = PlayerPrefs.GetFloat(saveName + "Sld");
        //Finds Camera moves in the scene

        if (tmpText != null)
        {
            tmpText.text = sld.value.ToString("00");
        }
        else if (txt != null)
        {
            txt.text = sld.value.ToString("00");
        }
        if (!XorY)
        {
            if (cameraMove != null && cameraMove.Length > 0)
            {
                foreach (BaseCameraMove camMov in cameraMove)
                {
                    camMov.OnChangeSensitivityX(sld.value);
                }
            }
        }
        else
        {
            if (cameraMove != null && cameraMove.Length > 0)
            {
                foreach (BaseCameraMove camMov in cameraMove)
                {
                    camMov.OnChangeSensitivityY(sld.value);
                }
            }
        }
    }
    //Changes sensitivity
    public void Save(float value)
    {
        if (tmpText != null)
        {
            tmpText.text = sld.value.ToString("00");
        }
        else if (txt != null)
        {
            txt.text = sld.value.ToString("00");
        }
        PlayerPrefs.SetFloat(saveName + "Sld", sld.value);
        if (!XorY)
        {
            if (cameraMove != null && cameraMove.Length > 0)
            {
                foreach (BaseCameraMove camMov in cameraMove)
                {
                    camMov.OnChangeSensitivityX(sld.value);
                }
            }
        }
        else
        {
            if (cameraMove != null && cameraMove.Length > 0)
            {
                foreach (BaseCameraMove camMov in cameraMove)
                {
                    camMov.OnChangeSensitivityY(sld.value);
                }
            }
        }

    }

    #endregion
}
