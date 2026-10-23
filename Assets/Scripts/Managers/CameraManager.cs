using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject CameraFollow_GO;

    public void UpdateCamera()
    {
        Vector3 followPos = Vector3.zero;
        List<Vector3> playerPositions = new List<Vector3>();
        foreach(var player in GameManager.Instance.PlayerManager.ActivePlayers)
        {
            playerPositions.Add(player.transform.position);
        }
        foreach(var playerPos in playerPositions)
        {
            followPos += playerPos;
        } 
        followPos /= playerPositions.Count;
        CameraFollow_GO.transform.position = followPos;
    }
}
