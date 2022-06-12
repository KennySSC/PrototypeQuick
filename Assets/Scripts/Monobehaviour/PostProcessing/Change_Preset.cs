using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Change_Preset : MonoBehaviour
{
    [Tooltip("Post processing profile that you want to use")]
    [SerializeField] VolumeProfile profile;

    private Set_Preset volume;
    private Change_Preset[] allPresetsButtons;

    private void Start()
    {
        volume = FindObjectOfType<Set_Preset>();
        allPresetsButtons = FindObjectsOfType<Change_Preset>();
    }

    public void OnChangePreset()
    {
        if (volume != null)
        {
            volume.SetNewPreset(profile);
            foreach(Change_Preset ps in allPresetsButtons)
            {
                if (ps != this)
                {
                    ps.GetComponent<Button>().interactable = true;
                }
                else
                {
                    ps.GetComponent<Button>().interactable = false;
                }
            }
        }
        else
        {
            Debug.LogError("The scene doesn't contain a Post Process Volume with a Set_Preset script");
        }
    }
    public VolumeProfile GetPreset()
    {
        return profile;
    }
}
