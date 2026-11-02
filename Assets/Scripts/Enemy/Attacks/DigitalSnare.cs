using UnityEngine;

public class DigitalSnare : MonoBehaviour
{
    public Virus MyVirus;
    public Player SnaredPlayer;
/*
    private void OnTriggerEnter(Collider other)
    {
        if (snaredPlayer != null) return;

        if(other.GetComponent<Player>() != null)
        {
            Player player = other.GetComponent<Player>();
            player.Snare(this);
        }
    }
*/

    public void Snare(Virus myVirus, Player snaredPlayer)
    {
        MyVirus = myVirus;
        SnaredPlayer = snaredPlayer;
    }

    public void DestroySnare()
    {
        if(SnaredPlayer != null) 
            SnaredPlayer.IsSnared = false;

        MyVirus.CurrentSnare = null;
        Destroy(gameObject);
    }
}
