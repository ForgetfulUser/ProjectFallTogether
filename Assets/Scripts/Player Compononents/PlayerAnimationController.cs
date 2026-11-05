using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public List<GameObject> PlayerSprite_GOs;
    public Animator Animator;
    private Sprite jumpSprite;
    public SpriteRenderer SpriteRenderer;
    private float lastHorzVal;

    public void StartAnimation(int playerIndex)
    {
        Animator = PlayerSprite_GOs[playerIndex].GetComponent<Animator>();
        SpriteRenderer = PlayerSprite_GOs[playerIndex].GetComponent<SpriteRenderer>();
        PlayerSprite_GOs[playerIndex].SetActive(true);

/*
        GameObject decal = null;
        switch (playerIndex)
        {
            default:
                Animator.SetBool("Is Player One", true);
                decal = Resources.Load<GameObject>("Decals/P1 Blob");
                break;
            case 1:
                Animator.SetBool("Is Player Two", true);
                decal = Resources.Load<GameObject>("Decals/P2 Blob");
                break;
            case 2:
                Animator.SetBool("Is Player Three", true);
                decal = Resources.Load<GameObject>("Decals/P3 Blob");
                break;
            case 3:
                Animator.SetBool("Is Player Four", true);
                decal = Resources.Load<GameObject>("Decals/P4 Blob");
                break;
        }

        if (decal != null && playerIndex >= 0 && playerIndex < 4)
        {
            decal = Instantiate(decal, transform);
        }
        else
        {
            Debug.LogWarning("Player Index: " + playerIndex + " is out of range.");
        }
*/
    }

    public void UpdateAnimation(Vector2 moveDir, bool isPunching)
    {
        Animator.SetFloat("Z Direction", moveDir.y);
        if(moveDir.y != 0) lastHorzVal = moveDir.y;
        Animator.SetBool("Is Back", lastHorzVal > 0);
        if (moveDir.x < -0.2 || moveDir.x > 0.2)
        {
            SpriteRenderer.flipX = (moveDir.x < 0);
            Animator.SetBool("Is Side", true);    
        }
        else
            Animator.SetBool("Is Side", false);
    }
}
