using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerMovement PlayerMovement;
    public PlayerAnimationController PlayerAnimationController;
    public PlayerAttack PlayerAttack;
    public bool IsSnared = false;

    // Player Indicator
    [HideInInspector] public int PlayerID = -1;

    [Header("Stats")]
    public int MaxLives;
    [HideInInspector] public int CurrentLives;

    // Reset Variables
    [HideInInspector] public Vector3 StartPos;
    [HideInInspector] public bool IsActive = true;

    [Header("Items")]
    public List<ItemType> ItemTypes = new List<ItemType>();

    [HideInInspector] public DigitalSnare Snare;

    public virtual void StartPlayer(int playerID)
    {
        PlayerMovement.StartMovement();
        PlayerID = playerID;
        PlayerAnimationController.StartAnimation(PlayerID);
        StartPos = transform.localPosition;
        CurrentLives = MaxLives;
    }

    public virtual void UpdatePlayer()
    {
        if (IsSnared) return;
        {
            PlayerMovement.UpdateMovement(out Vector2 moveDir, out bool isAttacking);

            PlayerAnimationController.UpdateAnimation(moveDir, isAttacking);

            PlayerAttack.Attack(moveDir, isAttacking);
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

    public bool PickupItem(ItemType itemType)
    {
        if(ItemTypes.Contains(itemType)) return false;
        ItemTypes.Add(itemType);
        return true;
    }
}