using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float AttackDistance;
    public AttackBox AttackBox;
    public FirewallCage FirewallCage;

    public void Attack(Vector3 moveDir, bool isAttacking)
    {
        moveDir.z = moveDir.y;
        moveDir.y = 0; 
        AttackBox.transform.position = moveDir * AttackDistance + transform.position;
        if (isAttacking == false) return;
        GetComponent<Player>().PlayerMovement.didAttack = false;
        if (AttackBox.VirusDetected == null) return;

        Vector3 spawnPos = AttackBox.VirusDetected.transform.position;
        spawnPos.y += 0.5f; // Prevents from spawning on the ground for collision
        Instantiate(FirewallCage, spawnPos, Quaternion.identity);
    }
}
