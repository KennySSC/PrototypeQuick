using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    //Base functions of a interactable object
    public abstract void Found(GameObject whoFind);
    public abstract void Miss(GameObject whoMiss);
    public abstract void Use_1(GameObject whoInteracted);
    public abstract void Use_2(GameObject whoInteracted);
    public abstract void Add_Events_Use1(List<Event> newEvents);
    public abstract void Add_Events_Use2(List<Event> newEvents);
    public abstract void ExternallyChange_Sprites(Sprite newSprt1, Sprite newSprt2);
    public abstract void ExternallyChange_Info(string newInfo1, string newInfo2);
}
