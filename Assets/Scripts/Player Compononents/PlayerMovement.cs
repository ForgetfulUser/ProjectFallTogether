using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody rb;
    public Animator animator;

    [Header("Move")]
    public float moveSpeed = 20;
    [HideInInspector] public Vector2 moveDir;
    private Vector3 vel;
    [SerializeField] private float maxForceCoolDown;
    public PlayerDetectionArea DetectionArea;
    public AttackBox AttackBox;

    [Header("Fall Speed")]
    public float maxFallSpeed = 18;

    private Player player;

    public void StartMovement()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody>();
    }

    public void UpdateMovement(out Vector2 moveDir)
    {
        moveDir = this.moveDir;
    }
    
    public void FixedUpdateMovement()
    {
        if (GetComponent<Player>().IsSnared)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }
        /*
                Vector3 force = Vector3.zero;

                force = new Vector3(
                    moveDir.x * moveSpeed * Time.fixedDeltaTime,
                    0f,
                    moveDir.y * moveSpeed * Time.fixedDeltaTime
                );
                rb.MovePosition(transform.position + force);
        */

        Vector3 move = transform.forward * moveDir.y + transform.right * moveDir.x;
        vel = (move * moveSpeed * Time.fixedDeltaTime);
        vel.y = rb.linearVelocity.y;
        rb.linearVelocity = vel;
        //rb.AddForce(force);
        LimitFallSpeed();
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }

    public void Attack(InputAction.CallbackContext context)
    {
        Debug.Log("hi");
        if (GameManager.Instance == false) return;
        if (context.canceled) return;
        if (GetComponent<Player>().IsSnared) return;
        if (AttackBox.CurrentRoutine == null) AttackBox.HighlightArea();
        GetComponent<PlayerAttack>().isAttacking = true;
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (GameManager.Instance != null)
        {
            DetectionArea.HighlightArea();
            if (player.DigitalSnare != null)
            {
                player.DigitalSnare.DestroySnare();
                player.DigitalSnare = null;
            }
        }
    }

    public void Click(InputAction.CallbackContext context)
    {
        //GameManager
    }

    private void LimitFallSpeed()
    {
        if (rb.linearVelocity.y <= -maxFallSpeed)
        {
            Vector3 newVel = rb.linearVelocity;
            newVel.y = -maxFallSpeed;
            rb.linearVelocity = newVel;
        }
    }

    public void ResetMovement()
    {
        rb.linearVelocity = Vector3.zero;
    }
}