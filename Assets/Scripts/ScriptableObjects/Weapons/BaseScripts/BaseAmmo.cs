using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoType", menuName = "Scriptable Objects/Ammo Type", order = 0)]
public class BaseAmmo : ScriptableObject
{
    #region Serializable variables

    [Tooltip("Max capacity for the player to hold this ammo type")]
    [SerializeField] int maxInventory;

    [Tooltip("The name of the ammo")]
    [SerializeField] string ammoName;

    [Tooltip("Icon to display to the player")]
    [SerializeField] Sprite ammoIcon;

    #endregion

    #region Get Set
    public int GetInventory()
    {
        return maxInventory;
    }
    public string GetAmmoName()
    {
        return ammoName;
    }
    public Sprite GetAmmoIcon()
    {
        return ammoIcon;
    }
    #endregion
}
