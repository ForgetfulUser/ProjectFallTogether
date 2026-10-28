using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerMovement PlayerMovement;
    public PlayerAnimationController PlayerAnimationController;
    public bool IsSnared = false;
    //public PlayerCanvasManager PlayerCanvasManager;

    [Header("Player Indicator")]
    public int PlayerID = -1;

    [Header("Stats")]
    public int CurrentLives;
    public int MaxLives;

    [Header("Reset Variables")]
    public Vector3 StartPos;
    public bool IsActive = true;

    [Header("Items")]
    public bool HasCrowbar;

    public virtual void StartPlayer(int playerID)
    {
        PlayerMovement.StartMovement();
        PlayerID = playerID;
        PlayerAnimationController.StartAnimation(PlayerID);
        StartPos = transform.localPosition;
    }

    public virtual void UpdatePlayer()
    {
        if (IsSnared) return;
        {
            PlayerMovement.UpdateMovement(out Vector2 moveDir, out bool isJumping, out bool isPunching);

            PlayerAnimationController.UpdateAnimation(moveDir, isJumping, isPunching);
        }
    }

    public virtual void FixedUpdatePlayer()
    {
        if (IsSnared) return;
        PlayerMovement.FixedUpdateMovement();
    }

    public virtual void ResetPlayer()
    {
        transform.localPosition = StartPos;
        PlayerMovement.ResetMovement();
    }

    public void GetBopped(int boppedAmount)
    {
        CurrentLives--;
    }

    public void PickupItem(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Crowbar:
                HasCrowbar = true;
                break;
        }
    }

    public virtual void TagPlayer()
    {

    }
}