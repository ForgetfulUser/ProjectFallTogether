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
    [HideInInspector] public int PlayerIndex = -1;

    [Header("Stats")]
    public int MaxLives;
    /*[HideInInspector] */public int CurrentLives;
    [HideInInspector] public DigitalSnare DigitalSnare;

    // Reset Variables
    [HideInInspector] public Vector3 StartPos;
    [HideInInspector] public bool IsActive = true;


    public virtual void StartPlayer(int playerID, int playerIndex)
    {
        PlayerMovement.StartMovement();
        PlayerID = playerID;
        PlayerIndex = playerIndex;
        if  (GameManager.Instance.PlayerManager.IsAngel())
            PlayerAnimationController.StartAnimation(4);
        else
            PlayerAnimationController.StartAnimation(PlayerIndex);

        StartPos = transform.localPosition;
        CurrentLives = MaxLives;
    }

    public virtual void UpdatePlayer()
    {
        if (IsSnared) return;
        {
            PlayerMovement.UpdateMovement(out Vector2 moveDir);
            PlayerAttack.Attack(moveDir, out bool isAttacking);

            PlayerAnimationController.UpdateAnimation(moveDir, isAttacking);

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

    public void Snare(DigitalSnare snare)
    {
        IsSnared = true;
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        transform.position = transform.position;
        DigitalSnare = snare;
    }

    public void GetBopped()
    {
        CurrentLives--;
    }
}