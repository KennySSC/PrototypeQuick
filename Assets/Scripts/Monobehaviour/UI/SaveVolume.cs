using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SaveVolume : MonoBehaviour
{
    #region Serializable Variables
    [Header("Main Settings")]

    [Tooltip("Name of the saved variable. It should be the same as the exposed parameter in the audio mixer, if not, it will not work")]
    [SerializeField] string saveName;

    [Tooltip("Mixer group that you want to change the volume")]
    [SerializeField] AudioMixerGroup mixer;

    [Tooltip("The slider that changes this volume levels")]
    [SerializeField] Slider sld;

    [Tooltip("Shows the percentage of the volume. If you don't need it, leave it empty")]
    [SerializeField] TMP_Text tmpText;

    [Tooltip("Shows the percentage of the volume, use this if you aren't using TMPro. If you don't need it, leave it empty")]
    [SerializeField] Text txt;

    
    [Space]


    [Header("Optional")]


    [Tooltip("If you are using a UI different audio mixer and you want it to function with the master volume, fill this. If you don't need it, leave it empty")]
    [SerializeField] AudioMixer uiMixer;

    #endregion

    #region Main Functions
    private void Awake()
    {
        //Sets saved volume level
        sld.value = PlayerPrefs.GetFloat((saveName + "Sld"), 1);
        float volume = Mathf.Log10(sld.value) * 20;
        mixer.audioMixer.SetFloat(saveName, volume);
        if (tmpText != null)
        {
            tmpText.text = (sld.value*100).ToString("00");
        }
        else if(txt != null)
        {
            txt.text = (sld.value*100).ToString("00");
        }
        if (uiMixer != null)
        {
            uiMixer.SetFloat("Volume", volume);
        }
    }
    //Changes volume and saves it
    public void Save(float value)
    {
        if (tmpText != null)
        {
            tmpText.text = (value*100).ToString("00");
        }
        else if (txt != null)
        {
            txt.text = (value*100).ToString("00");
        }
        PlayerPrefs.SetFloat(saveName + "Sld", value);
        float volume = Mathf.Log10(value) * 20;
        PlayerPrefs.SetFloat(saveName+"Vlm", volume);
        mixer.audioMixer.SetFloat(saveName, volume);
        if (uiMixer != null)
        {
            uiMixer.SetFloat("Volume", PlayerPrefs.GetFloat((saveName + "Vlm"), (Mathf.Log10(1) * 20)));
        }
    }
    public void Load()
    {
        //Sets mixers volume externally
        float volume = Mathf.Log10(sld.value) * 20;
        mixer.audioMixer.SetFloat(saveName, volume);
        if (uiMixer != null)
        {
            uiMixer.SetFloat("Volume", volume);
        }
    }
    #endregion
}
