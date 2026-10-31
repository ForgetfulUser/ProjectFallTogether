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

    public void GetClosestPathAndPoint(Vector3 currentPos, out int closestPathIndex, out int closestPointIndex)
    {
        List<Transform> list = new List<Transform>();
        float closestDistance = int.MaxValue;
        int pathIndex = 0;
        closestPathIndex = 0;
        int pointIndex = 0;
        closestPointIndex = 0;

        foreach(Transform path in ForestPaths)
        {
            pointIndex = 0;
            foreach (Transform point in path.GetComponentsInChildren<Transform>())
            {
                if (point == path) continue;
                float pointDistance = Vector3.Distance(currentPos, point.position);
                if(pointDistance < closestDistance)
                {
                    closestPathIndex = pathIndex;
                    closestPointIndex = pointIndex;
                    closestDistance = pointDistance;
                }
                pointIndex++;
            }
            pathIndex++;
        }
    }
/*
    public Transform GetClosestPoint(out int pointIndex)
    {
        pointIndex = 0;
        if (ForestPaths.Count == 0)
        {
            Virus.VirusState = VirusState.Detect;
            return null;
        }
        GetClosestPathAndPoint(out int path, out pointIndex);
        return ForestPaths[path].GetComponentsInChildren<Transform>()[pointIndex];
    }
*/
    public List<Transform> GetClosestPath(Vector3 currentPos, out Transform closestPoint, out int closestPointIndex)
    {
        closestPoint = null;
        closestPointIndex = 0;
        if (ForestPaths.Count == 0)
        {
            Virus.VirusState = VirusState.Detect;
            Virus.ChangeState();
            return null;
        }
        GetClosestPathAndPoint(currentPos, out int path, out closestPointIndex);
        List<Transform> list = new List<Transform>();
        foreach(Transform pointT in ForestPaths[path].GetComponentsInChildren<Transform>())
        {
            if (pointT == ForestPaths[path]) continue;
            list.Add(pointT);
        }
        closestPoint = list[closestPointIndex];
        return list;
    }
}
