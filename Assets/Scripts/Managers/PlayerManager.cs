using NUnit.Framework.Internal.Commands;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Player Player_PRFB;
    private List<Vector3> PlayerSpawnPosition = new List<Vector3>();
    public List<Player> ActivePlayers = new List<Player>();
    public List<Player> InactivePlayers = new List<Player>();
    public float MinimumFallHeight = -10;
    [Header("Play Test Variables")]
    public Player TestingPlayer;
    private Dictionary<int, int> PlayerIDs = new Dictionary<int, int>(); // Device | Player
    public Player lastPlayerStanding;

    private void Start()
    {
        StartManager();
    }

    public void StartManager()
    {
        foreach(var spawnPos in GetComponentsInChildren<Transform>())
        {
            if (spawnPos == transform) continue;
            PlayerSpawnPosition.Add(spawnPos.position);
            Destroy(spawnPos.gameObject);
        }
        PlayerIDs = MySceneManager.Instance.playerIDs;
        SpawnPlayers();
        if (TestingPlayer) 
            Player_PRFB.StartPlayer(0);
    }

    public void UpdateManager()
    {
        if (TestingPlayer == null)
        {
            foreach(var player in ActivePlayers)
            {
                if (player.IsActive)
                {
                    player.UpdatePlayer();
                    ResetTest(player);
                }
            }
        }
        else
        {
            TestingPlayer.UpdatePlayer();
            ResetTest(TestingPlayer);
        }
    }

    public void FixedUpdateManager()
    {
        if (TestingPlayer == null)
        {
            foreach (var player in ActivePlayers)
            {
                if(player.IsActive)
                    player.FixedUpdatePlayer();
            }
        }
        else
        {
            TestingPlayer.FixedUpdatePlayer();
        }
    }

    public void ResetTest(Player player)
    {
        if(player.transform.position.y <= MinimumFallHeight)
        {
            RespawnPlayer(player);
        }
    }

    private void SpawnPlayers()
    {
        if (TestingPlayer) return;
        for(int i = 0; i < PlayerIDs.Count ; i++)
        {
            Player player = Instantiate(Player_PRFB, PlayerSpawnPosition[i], Quaternion.identity);
            player.StartPlayer(i);
            ActivePlayers.Add(player);
        }
    }

    public void RespawnPlayer(Player player)
    {
        if (player.IsActive == false) return;
        
        player.CurrentLives--;
        if (player.CurrentLives <= 0)
        {
            player.gameObject.SetActive(false);
            player.IsActive = false;
            if(InactivePlayers.Contains(player) == false)
                InactivePlayers.Add(player);
        }

        player.transform.position = PlayerSpawnPosition[player.PlayerIndex];
    }

    public Player LastPlayerStanding()
    {
        if (InactivePlayers.Count + 1 < ActivePlayers.Count) return null;

        foreach (Player player in ActivePlayers)
        {
            if(InactivePlayers.Contains(player)) continue;
            lastPlayerStanding = player;
        }
        return lastPlayerStanding;
    }
}
