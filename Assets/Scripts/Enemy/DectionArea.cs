using System.Collections.Generic;
using UnityEngine;

public class DectionArea : MonoBehaviour
{
    public Virus Virus;
    public List<Player> Players;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            Players.Add(other.GetComponent<Player>());
            Virus.DetectPlayer(other.GetComponent<Player>());
        }
    }
}
