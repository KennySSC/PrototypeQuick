using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Objective : MonoBehaviour
{
    //Base functions of a Objective
    public abstract void ObjectiveReset();
    public abstract void ObjectiveComplete();
    public abstract void EvaluateObjectiveProgress();
    public abstract void SetManager(ObjectiveManager newManager);
    public abstract void SetTextInfo(GameObject textObjectGame, GameObject textObjectPause);
    public abstract void AssingEvents();
    public abstract void UpdateTextInfo();
    public abstract bool GetIsComplete();
    public abstract string GetObjectiveDescription();
}
