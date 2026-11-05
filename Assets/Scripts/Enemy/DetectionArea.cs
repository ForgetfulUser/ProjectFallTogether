using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem.Utilities;

public class DetectionArea : MonoBehaviour
{
    public Virus Virus;
    public List<Player> DetectedPlayers;
    public List<Player> RemovedPlayers;
    bool goingNext;

    public Player GetNextTarget()
    {
        goingNext = true;
        if (RemovedPlayers.Count > 0 && DetectedPlayers.Count > 0)
        {
            for (int i = 0; i < RemovedPlayers.Count; i++)
            {
                if (i >= DetectedPlayers.Count) break;

                if (DetectedPlayers[i] == null)
                    DetectedPlayers.RemoveAt(i);
                else if (RemovedPlayers[i] == null) continue;
                else if (DetectedPlayers[i].name == RemovedPlayers[i].name)
                    DetectedPlayers.Remove(DetectedPlayers[i]);
            }
            RemovedPlayers.Clear();
        }

        float closestDistance = int.MaxValue;
        Player closestPlayer = null;
        foreach(Player player in DetectedPlayers)
        {
            if(Vector3.Distance(player.transform.position, transform.position) < closestDistance)
            {
                closestDistance = Vector3.Distance(player.transform.position, transform.position);
                closestPlayer = player;
            }
        }
        goingNext = false;
        return closestPlayer;
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player && DetectedPlayers.Contains(player) == false) 
        {
            DetectedPlayers.Add(player);
            Virus.DetectPlayer(player);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player && DetectedPlayers.Contains(player) == false) 
        {
            DetectedPlayers.Add(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            RemovedPlayers.Add(other.GetComponent<Player>());
        }
    }


}
