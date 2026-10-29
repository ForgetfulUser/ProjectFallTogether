using UnityEngine;

public class DigitalSnare : MonoBehaviour
{
    public Virus MyVirus;
    public Player snaredPlayer;
    private void OnTriggerEnter(Collider other)
    {
        if (snaredPlayer != null) return;

        if(other.GetComponent<Player>() != null)
        {
            Player player = other.GetComponent<Player>();
            player.IsSnared = true;
            player.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            snaredPlayer = player;
        }
    }

    public void DestroySnare()
    {
        if(snaredPlayer != null) 
            snaredPlayer.IsSnared = false;

        MyVirus.CurrentSnare = null;
        Destroy(gameObject);
    }
}
