using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody rb;
    public Animator animator;

    [Header("Move")]
    public float extraGeneralGravity;
    public float extraDecsentGravity;
    public float moveSpeed = 20;
    [HideInInspector] public Vector2 moveDir;
    [SerializeField] private float maxForceCoolDown;

    [Header("Stun")]
    private float stunTimer;


    [Header("Fall Speed")]
    public float maxFallSpeed = 18;

    private Player player;
    private Coroutine appliedForceRoutine;
    private Coroutine punchingCoroutine;
    private bool isPunching;

    public void StartMovement()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody>();
        stunTimer = 0f;
    }

    public void UpdateMovement(out Vector2 moveDir, out bool isJumping, out bool isPunching)
    {
        //GroundCheck();

        moveDir = this.moveDir;
        isJumping = false;
        isPunching = this.isPunching;
    }
    
    public void FixedUpdateMovement()
    {
        if (stunTimer > 0f)
        {
            stunTimer -= Time.fixedDeltaTime;
        }

        Vector3 force = Vector3.zero;

        if(stunTimer <= 0){
            force = new Vector3(
                moveDir.x * moveSpeed * Time.deltaTime,
                0f,
                moveDir.y * moveSpeed * Time.deltaTime
            );
        }

        rb.MovePosition(transform.position + force);
        //ExtraGravity();
        if(rb.linearVelocity.y <= -maxFallSpeed)
        {
            Vector3 newVel = rb.linearVelocity;
            newVel.y = -maxFallSpeed;
            rb.linearVelocity = newVel;
        }
    }

    public void ApplyForce(Vector3 force, float stunDuration, ForceMode forceMode)
    {
        if(stunDuration > 0 && stunTimer <= 0)
        {
            Stun(stunDuration);
        }

        if (appliedForceRoutine != null) return;
        rb.AddForce(force, forceMode);
        appliedForceRoutine = StartCoroutine(ApplyForceRoutine());
    }

    private IEnumerator ApplyForceRoutine()
    {
        yield return new WaitForSeconds(maxForceCoolDown);
        appliedForceRoutine = null;
        rb.linearVelocity = Vector3.zero;
    }

    private IEnumerator PunchingCoroutine()
    {
        isPunching = true;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        isPunching = false;
        punchingCoroutine = null;
    }

    public void Stun(float duration)
    {
        if (duration > stunTimer)
        {
            stunTimer = duration;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.canceled) return;
        if (player.IsSnared) return;

        //if (punchingCoroutine == null) { punchingCoroutine = StartCoroutine(PunchingCoroutine()); }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        Debug.Log("HIT");
        if (player.IsSnared) return;

        if(player.Snare != null)
        {
            player.Snare.DestroySnare();
            player.Snare = null;
        }
    }

    private void ExtraGravity()
    {
        //if (rb.linearVelocity.y >= 0) return;
        Vector3 velocity = rb.linearVelocity;
        velocity.y -= extraGeneralGravity * Time.deltaTime;
        if(velocity.y < 0) velocity.y -= extraDecsentGravity * Time.deltaTime;
        rb.linearVelocity = velocity;
        LimitFallSpeed();
    }

    private void LimitFallSpeed()
    {
        if (rb.linearVelocity.y < -maxFallSpeed)
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                -maxFallSpeed,
                rb.linearVelocity.z
            );
        }
    }

    public void ResetMovement()
    {
        if (appliedForceRoutine != null)
        {
            StopCoroutine(appliedForceRoutine);
            appliedForceRoutine = null;
        }
        rb.linearVelocity = Vector3.zero;
        stunTimer = -1;
    }
}