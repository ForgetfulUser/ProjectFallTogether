using NUnit.Framework.Constraints;
using NUnit.Framework.Internal.Commands;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public Player Player_PRFB;
    private List<Vector3> PlayerSpawnPosition = new List<Vector3>();
    public List<Player> ActivePlayers = new List<Player>();
    public List<Player> InactivePlayers = new List<Player>();
    public float MinimumFallHeight = -10;
    private Dictionary<int, PlayerInput> PlayerInputs = new Dictionary<int, PlayerInput>(); // Device | Player

    [Header("Items")]
    public static List<ItemType> ItemTypes = new List<ItemType>();
    [SerializeField] List<ItemType> ItemTypesList = new List<ItemType>();
    public static int NoteCount;
    [SerializeField] int noteCount;

    private void Start()
    {
        //StartManager();
    }

    public void StartManager()
    {
        ItemTypes = ItemTypesList;
        NoteCount = noteCount;
        foreach(var spawnPos in GetComponentsInChildren<Transform>())
        {
            if (spawnPos == transform) continue;
            PlayerSpawnPosition.Add(spawnPos.position);
            //Destroy(spawnPos.gameObject);
        }
        PlayerInputs = MySceneManager.Instance.playerInputs;
        SetPlayers();
    }

    public void UpdateManager()
    {
        foreach (var player in ActivePlayers)
        {
            if (player.IsActive)
            {
                player.UpdatePlayer();
                ResetTest(player);
            }
        }
    }

    public void FixedUpdateManager()
    {
        foreach (var player in ActivePlayers)
        {
            if (player.IsActive)
                player.FixedUpdatePlayer();
        }
    }

    public void ResetTest(Player player)
    {
        if(player.transform.position.y <= MinimumFallHeight)
        {
            RespawnPlayer(player);
        }
    }
    
    private void SetPlayers()
    {
        int index = 0;
        foreach(KeyValuePair<int, PlayerInput> pInputs in PlayerInputs)
        {
            Player player = pInputs.Value.GetComponent<Player>();
            player.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            Vector3 spawnPos = PlayerSpawnPosition[index];
            spawnPos.y += 0.5f;
            player.transform.position = spawnPos;
            player.StartPlayer(pInputs.Key);
            ActivePlayers.Add(player);
            index++;
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

        player.transform.position = PlayerSpawnPosition[player.PlayerID];
    }

    public bool NoPlayerStanding()
    {
        return ActivePlayers.Count <= 0;
    }

    public void ResetManager()
    {
        int index = 0;
        foreach (Player player in ActivePlayers)
        {
            player.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            Vector3 spawnPos = PlayerSpawnPosition[index];
            spawnPos.y += 0.5f;
            player.transform.position = spawnPos;
            index++;
        }
    }

    public static bool PickUpItem(ItemType itemType)
    {
        if (itemType != ItemType.Note && ItemTypes.Contains(itemType)) return false;
        if (NoteCount >= 3) return false;

        ItemTypes.Add(itemType);
        if (itemType == ItemType.Note) NoteCount++;

        return true;
    }
}
