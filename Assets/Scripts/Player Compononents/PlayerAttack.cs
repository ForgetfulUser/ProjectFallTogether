using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public float AttackDistance;
    public AttackBox AttackBox;
    public FirewallCage FirewallCage;
    public bool isAttacking = true;

    public void Attack(Vector3 moveDir, out bool _isAttacking)
    {
        moveDir.z = moveDir.y;
        Vector3 newPos = moveDir * AttackDistance + transform.position;
        newPos.y = AttackBox.transform.position.y;
        AttackBox.transform.position = newPos;
        _isAttacking = isAttacking;
        if (isAttacking == false) return;
        isAttacking = false;
        if (AttackBox.VirusDetected == null) return;

        Vector3 spawnPos = AttackBox.VirusDetected.transform.position;
        spawnPos.y -= 1.0f; // Prevents from spawning on the ground for collision
        Instantiate(FirewallCage, spawnPos, Quaternion.identity);
    }
}
