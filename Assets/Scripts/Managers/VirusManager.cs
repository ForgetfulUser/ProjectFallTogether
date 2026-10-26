using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VirusManager : MonoBehaviour
{
    public List<Transform> ForestPaths = new List<Transform>();
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

        foreach(Transform path in ForestPaths)
        {
            Debug.Log("Checkign");
            foreach(Transform point in path.GetComponentsInChildren<Transform>())
            {
                float pointDistance = Vector3.Distance(transform.parent.position, point.position);
                if(pointDistance < closestDistance)
                {
                    Debug.Log("Path Index: " + pathIndex + " Point Index: " + pointIndex);
                    closestPathIndex = pathIndex;
                    closestPointIndex = pointIndex;
                }
                pointIndex++;
            }

            pointIndex = 0;
            pathIndex++;
        }
    }

    public Transform GetClosestPoint()
    {
        GetClosestPathAndPoint(out int path, out int point);
        return ForestPaths[path].GetComponentsInChildren<Transform>()[point];
    }

    public List<Transform> GetClosestPath()
    {
        GetClosestPathAndPoint(out int path, out int point);
        List<Transform> list = new List<Transform>();
        foreach(Transform pointT in ForestPaths[path].GetComponentsInChildren<Transform>())
        {
            list.Add(pointT);
        }
        return list;
    }
}
