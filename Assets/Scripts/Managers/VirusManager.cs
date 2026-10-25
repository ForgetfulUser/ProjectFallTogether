using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VirusManager : MonoBehaviour
{
    public List<List<Transform>> ForestPaths = new List<List<Transform>>();
    public List<Player> CurrentActivePlayers;
    public Virus Virus;

    public void StartManager()
    {
        UpdateActivePlayers();
        Virus.StartVirus(this);
    }

    public void UpdateManager()
    {
        Virus.UpdateVirus();
    }

    public void UpdateActivePlayers()
    {
        CurrentActivePlayers = GameManager.Instance.PlayerManager.ActivePlayers;
    }

    public void ResetVirus()
    {

    }

    public void GetClosestPathAndPoint(out int closestPathIndex, out int closestPointIndex)
    {
        List<Transform> list = new List<Transform>();
        float closestDistance = 0;
        int pathIndex = 0;
        closestPathIndex = 0;
        int pointIndex = 0;
        closestPointIndex = 0;

        foreach(List<Transform> path in ForestPaths)
        {
            foreach(Transform point in path)
            {
                float pointDistance = Vector3.Distance(transform.parent.position, point.position);
                if(pointDistance < closestDistance)
                {
                    closestPathIndex = pathIndex;
                    closestPointIndex = pointIndex;
                }
                pointIndex++;
            }

            pointIndex = 0;
            pathIndex++;
        }
    }
}
