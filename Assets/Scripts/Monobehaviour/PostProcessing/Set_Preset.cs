using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class Set_Preset : MonoBehaviour
{
    #region Serializable variables
    [Tooltip("All post process profiles available")]
    [SerializeField] List<VolumeProfile> presets = new List<VolumeProfile>();

    [Tooltip("Post Processing volume that contains the current profile")]
    [SerializeField] Volume _volume;

    #endregion

    #region private variables

    int currentPP = 0;

    private CameraFX[] camFX;

    private Change_Preset[] allPresetsButtons;

    #endregion

    #region Main Functions
    private void Start()
    {
        currentPP = PlayerPrefs.GetInt("PP_Set",0);
        SetNewPreset(presets[currentPP]);
        allPresetsButtons = FindObjectsOfType<Change_Preset>();

        foreach(Change_Preset ps in allPresetsButtons)
        {
            if(presets[currentPP] == ps.GetPreset())
            {
                ps.GetComponent<Button>().interactable = false;
            }
            else
            {
                ps.GetComponent<Button>().interactable = true;
            }
        }
    }
    #endregion

    #region Get Set
    public void SetNewPreset(VolumeProfile changePreset)
    {
        if (presets.Contains(changePreset))
        {
            _volume.profile = changePreset;
            currentPP = presets.IndexOf(changePreset);
            PlayerPrefs.SetInt("PP_Set", currentPP);

            camFX = FindObjectsOfType<CameraFX>();

            foreach(CameraFX cam in camFX)
            {
                cam.RefreshPostProcess_Values();
            }
        }
        else
        {
            Debug.LogError("The presets list doesn't contains the one that you are trying to send. Add it to the list");
        }
    }

    #endregion
}
