using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class BaseMovement : MonoBehaviour
{
    //Base functions for a movement and a weapon controller
    public abstract void ResetValues();
    public abstract void Movement(Vector2 inputAxis);
    public abstract void StopMoving();
    public abstract void Jump();
    public abstract void Crouch();
    public abstract void Run();
    public abstract void ChangeRun(bool isRunning);
    public abstract void ChangeAiming(bool isAiming);
    public abstract void Interact_1();
    public abstract void Interact_2();
    public abstract void SearchInteractable();
    public abstract void TryToStandUp(bool willStand);
    public abstract void ReadGround(bool jump);
    public abstract void AnimationLogic();
    public abstract void IsOnCoyote(bool isCoyote);
    public abstract void ChangeHasGun(bool change);
    public abstract void SetSound_Particles(GameObject[] footstepsSound, GameObject jumpSound, GameObject footstepParticle, Transform particlePosition, float stepsTime);
    public abstract float GetCoyoteTime();
    public abstract bool IsJumping();
    public abstract bool CanSpawnShotObject();
    public abstract void ReverseJumping(bool jump);
    public abstract void RotateCharacter();
    public abstract void SetPlayerMovement(PlayerMovement newPlayerMovement);
    public abstract Transform GetShotPosition();
    public abstract Animator GetPlayerAnimator();
    public abstract Collider GetPlayerCollider();
    public abstract void SetUIInfo(TMP_Text tmpTxt_1, TMP_Text tmpTxt_2, Text newtxt_1, Text newtxt_2, Image newimg1, Image newimg2);
    //Image is the most important, if it's null the text will not show
    public abstract void SetSpritesInfo(Sprite sprt1, Sprite sprt2);
    public abstract void SetExtraInfo(string info1, string info2);
}
