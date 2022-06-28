using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraFX : MonoBehaviour
{
    #region Serializable Variables
    [Header("Main Settings")]

    [Tooltip("Set camera fx to behave for non fps cameras")]
    [SerializeField] private bool isTps = false;

    [Tooltip("Camera shake script that will controll that effect")]
    [SerializeField] CameraShake camShake;

    [Tooltip("Object that will shake. For best results, the camera should be in a empty game object and camera local position should be 0,0,0")]
    [SerializeField] GameObject cameraObject;


    [Space]


    [Header("Vignette effects settings")]


    [Tooltip("The color of the vignette while the player is healing")]
    [SerializeField] Color vignette_Healing_Color;

    [Tooltip("The color of the vignette while the player is losing health")]
    [SerializeField] Color vignette_Dying_Color;

    [Tooltip("The color of the vignette while the player have low health and is healing")]
    [SerializeField] Color vignette_HealingAndDying_Color;

    [Tooltip("The max vignette intensity used for effects")]
    [Range(0.01f, 1f)] [SerializeField] float vignetteIntensity_MaxValue;

    [Tooltip("The vignette intensity value while player is healing")]
    [Range(0.01f, 1f)] [SerializeField] float vignetteIntensity_HealingValue;

    [Tooltip("The health % that the player must have to start calculating the vignette effect")]
    [Range(0.05f, 1f)] [SerializeField] float vignetteDamage_StartingPercentage;

    [Tooltip("The health % that the player must have to reach the max vignette effect values")]
    [Range(0.0f, 0.95f)] [SerializeField] float vignetteDamage_MinPercentage;


    [Header("Saturation effect settings")]

    [Tooltip("The health % that the player must have to start calculating the saturation effect")]
    [Range(0.0f, 0.95f)] [SerializeField] float saturationDamage_StartingPercentage;

    [Tooltip("The health % that the player must have to reach the max saturation effect values")]
    [Range(0.0f, 0.95f)] [SerializeField] float saturationDamage_MinPercentage;

    #endregion

    #region private variables

    private Volume _volume;
    private Vignette _vignette;
    private ColorAdjustments _colorAdjustments;
    private Health hp;

    private Color startingVignetteColor;

    private int healthState = 0;

    private float maxSat;
    
    private float vignetteStartValue;

    private float tempSaturationRange = 0;
    private float tempVignetteRange = 0;

    #endregion

    #region Main Functions
    private void Awake()
    {
        _volume = FindObjectOfType<Volume>();
        if (vignetteDamage_StartingPercentage < vignetteDamage_MinPercentage)
        {
            Debug.LogError("Starting vignette percentage must be greater than minimum");
        }
        if (saturationDamage_StartingPercentage < saturationDamage_MinPercentage)
        {
            Debug.LogError("Starting saturation percentage must be greater than minimum");
        }
        if (_volume.profile.TryGet<Vignette>(out _vignette))
        {
            startingVignetteColor = _vignette.color.value;
            vignetteStartValue = _vignette.intensity.value;
            if (vignetteStartValue > vignetteIntensity_MaxValue)
            {
                Debug.Log("The current vignette value it's greater that max value. The max value will set to vignette current value. Starting vignette will set to 0");
                vignetteIntensity_MaxValue = vignetteStartValue;
                _vignette.intensity.value = 0;
            }
        }
        if (_volume.profile.TryGet<ColorAdjustments>(out _colorAdjustments))
        {
            maxSat = _colorAdjustments.saturation.value;
        }
        float tempSat = maxSat;
        while (tempSat > -100)
        {
            tempSat -= 1;
            tempSaturationRange += 1;
        }
        float tempVig = vignetteStartValue;
        while (tempVig < vignetteIntensity_MaxValue)
        {
            tempVig += 0.01f;
            tempVignetteRange += 0.01f;
        }
    }
    public void DoCameraShake(float duration, float magnitude)
    {
        camShake.DoShakeCam(duration, magnitude, cameraObject, isTps);
    }
    public void CalculateVignette(float value)
    {
        float range = (vignetteDamage_StartingPercentage - vignetteDamage_MinPercentage);
        if (value > vignetteDamage_StartingPercentage)
        {
            if(_volume.profile.TryGet<Vignette>(out _vignette))
            {
                if (healthState == 1)
                {
                    _vignette.intensity.value = vignetteIntensity_HealingValue;
                }
                else
                {
                    _vignette.intensity.value = vignetteStartValue;
                }
                if(value == 1)
                {
                    SetHealthState(0);
                    StartCoroutine(HealingVignette_SmoothRemove());
                    StartCoroutine(LerpColors());
                }

            }
        }else if (value < vignetteDamage_StartingPercentage && value >= vignetteDamage_MinPercentage)
        {
            if (_volume.profile.TryGet<Vignette>(out _vignette))
            {
                //normalizes values to set the needed ones
                float newValue = ((1-(value / range)) * tempVignetteRange);
                _vignette.intensity.value = vignetteStartValue + newValue;
                if(healthState == 1)
                {
                    SetHealthState(2);
                }
                if (healthState == 2 && _vignette.intensity.value < vignetteIntensity_HealingValue)
                {
                    _vignette.intensity.value = vignetteIntensity_HealingValue;
                }
            }
        }else if(value < vignetteDamage_MinPercentage)
        {
            _vignette.intensity.value = vignetteIntensity_MaxValue;
        }
    }
    //calculates saturation fx
    public void CalculateSaturation(float value)
    {
        float range = (saturationDamage_StartingPercentage - saturationDamage_MinPercentage);
        if (value> saturationDamage_StartingPercentage)
        {
            if (_volume.profile.TryGet<ColorAdjustments>(out _colorAdjustments))
            {
                _colorAdjustments.saturation.value = maxSat;
            }
        }
        else if (value < saturationDamage_StartingPercentage && value>= saturationDamage_MinPercentage)
        {
            //normalizes values to set the needed ones
            if (_volume.profile.TryGet<ColorAdjustments>(out _colorAdjustments))
            {
                float newValue = ((1-(value/range)))*tempSaturationRange;
                _colorAdjustments.saturation.value = maxSat - newValue;

            }
        }else if (value < saturationDamage_MinPercentage)
        {
            if (_volume.profile.TryGet<ColorAdjustments>(out _colorAdjustments))
            {
                _colorAdjustments.saturation.value = -100;
            }
        }
    }

    //Resets post process default values
    private void OnDestroy()
    {
        if (_volume != null)
        {
            if (_volume.profile.TryGet<Vignette>(out _vignette))
            {
                Debug.Log("Aki ando");
                _vignette.intensity.value = vignetteStartValue;
                _vignette.color.value = startingVignetteColor;
            }
            if (_volume.profile.TryGet<ColorAdjustments>(out _colorAdjustments))
            {
                _colorAdjustments.saturation.value = maxSat;
            }
        }
    }
    private void OnApplicationQuit()
    {
        if (_volume != null)
        {
            if (_volume.profile.TryGet<Vignette>(out _vignette))
            {
                _vignette.intensity.value = vignetteStartValue;
                _vignette.color.value = startingVignetteColor;
            }
            if (_volume.profile.TryGet<ColorAdjustments>(out _colorAdjustments))
            {
                _colorAdjustments.saturation.value = maxSat;
            }
        }
    }
    public void ResetColors()
    {
        if (_volume != null)
        {
            if (_volume.profile.TryGet<Vignette>(out _vignette))
            {
                _vignette.intensity.value = vignetteStartValue;
                _vignette.color.value = startingVignetteColor;
            }
            if (_volume.profile.TryGet<ColorAdjustments>(out _colorAdjustments))
            {
                _colorAdjustments.saturation.value = maxSat;
            }
        }
    }
    #endregion

    #region Get Set
    //In case of changing to a new post process preset, resets values
    public void RefreshPostProcess_Values()
    {
        tempSaturationRange = 0;
        tempVignetteRange = 0;


        if(_volume.profile.TryGet<Vignette>(out _vignette))
        {
            vignetteStartValue = _vignette.intensity.value;
            startingVignetteColor = _vignette.color.value;
        }
        if(_volume.profile.TryGet<ColorAdjustments>(out _colorAdjustments))
        {
            maxSat = _colorAdjustments.saturation.value;
        }
        float tempSat = maxSat;
        while (tempSat > -100)
        {
            tempSat -= 1;
            tempSaturationRange += 1;
        }
        float tempVig = vignetteStartValue;
        while (tempVig < vignetteIntensity_MaxValue)
        {
            tempVig += 0.01f;
            tempVignetteRange += 0.01f;
        }
        if(hp!= null)
        {
            CalculateSaturation(hp.GetCurrentHealth_Normalized());
            CalculateVignette(hp.GetCurrentHealth_Normalized());
        }
    }
    //Sets health states to know how to show the vignette
    public void SetHealthState(int stateIndex)
    {
        healthState = stateIndex;
        if(stateIndex == 0)
        {
            if(_volume.profile.TryGet<Vignette>(out _vignette))
            {
                _vignette.color.value = vignette_Dying_Color;
            }
        }
        else if(stateIndex == 1)
        {
            if (_volume.profile.TryGet<Vignette>(out _vignette))
            {
                _vignette.color.value = vignette_Healing_Color;
            }
        }
        else if (stateIndex == 2)
        {
            if (_volume.profile.TryGet<Vignette>(out _vignette))
            {
                _vignette.color.value = vignette_HealingAndDying_Color;
            }
        }

    }
    public void SetHP(Health newHP)
    {
        hp = newHP;
    }
    #endregion

    #region Coroutines
    //Vignette healing intensity lerps to vignette original intensity
    IEnumerator HealingVignette_SmoothRemove()
    {
        float elapsed = 0f;
        bool isless = false;
        float diference = 0f;

        if(vignetteIntensity_HealingValue < vignetteStartValue)
        {
            isless = true;
            diference = vignetteStartValue - vignetteIntensity_HealingValue;
        }
        else if(vignetteIntensity_HealingValue > vignetteStartValue)
        {
            isless = false;
            diference = vignetteIntensity_HealingValue - vignetteStartValue;
        }
        else if (vignetteIntensity_HealingValue == vignetteStartValue)
        {
            if (_volume.profile.TryGet<Vignette>(out _vignette))
            {
                _vignette.color.value = startingVignetteColor;
            }
            yield break;
        }

        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            if (isless)
            {
                if (_volume.profile.TryGet<Vignette>(out _vignette))
                {
                    _vignette.intensity.value += (diference * (Time.deltaTime / 2));
                    if (_vignette.intensity.value < vignetteStartValue)
                    {
                        _vignette.intensity.value = vignetteStartValue;
                    }
                }
            }
            else
            {
                if (_volume.profile.TryGet<Vignette>(out _vignette))
                {
                    _vignette.intensity.value -= (diference * (Time.deltaTime / 2));
                    if (_vignette.intensity.value < vignetteStartValue)
                    {
                        _vignette.intensity.value = vignetteStartValue;
                    }
                }
            }
            
            yield return null;
        }
    }

    //If finished healing, vignette lerps between healing color and vignette original color
    IEnumerator LerpColors()
    {
        for(float t =0.01f; t<0.5f; t += 0.1f)
        {
            if (_volume.profile.TryGet<Vignette>(out _vignette))
            {
                _vignette.color.value = Color.Lerp(vignette_Healing_Color, startingVignetteColor, t / 0.5f);
                yield return null;
            }
        }
    }

    #endregion

}
