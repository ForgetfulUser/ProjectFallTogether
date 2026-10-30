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

    [Header("Fall Speed")]
    public float maxFallSpeed = 18;

    private Player player;
    [HideInInspector] public bool didAttack;

    public void StartMovement()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody>();
    }

    public void UpdateMovement(out Vector2 moveDir, out bool didAttack)
    {
        moveDir = this.moveDir;
        didAttack = this.didAttack;
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
        if (GameManager.Instance == false) return;
        if (context.canceled) return;
        if (player.IsSnared) return;
        didAttack = true;
        //if (punchingCoroutine == null) { punchingCoroutine = StartCoroutine(PunchingCoroutine()); }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (player.IsSnared && GameManager.Instance == null) return;

        if(player.Snare != null)
        {
            player.Snare.DestroySnare();
            player.Snare = null;
        }
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