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
    private bool angelSelected;
    [Header("Items")]
    public static List<ItemType> ItemTypes = new List<ItemType>();
    [SerializeField] List<ItemType> ItemTypesList = new List<ItemType>();
    public static int NoteCount;
    [SerializeField] int noteCount;
    public static bool HasBluePowerCell;
    public static bool HasGreenPowerCell;

    public void StartManager()
    {
        foreach(var spawnPos in GetComponentsInChildren<Transform>())
        {
            if (spawnPos == transform) continue;
            PlayerSpawnPosition.Add(spawnPos.position);
        }
        PlayerInputs = MySceneManager.Instance.playerInputs;
        SetPlayers();
    }

    public void UpdateManager()
    {
        ItemTypesList = ItemTypes;
        noteCount = NoteCount;

        foreach (var player in ActivePlayers)
        {
            if (player.IsActive)
            {
                player.UpdatePlayer();
            }
            else
            {
                InactivePlayers.Add(player);
            }
        }
        foreach(Player player in InactivePlayers)
        {
            if(ActivePlayers.Contains(player))
            {
                ActivePlayers.Remove(player);
            }
        }
        if (NoPlayerStanding()) GameManager.Instance.GameOver();
    }

    public void FixedUpdateManager()
    {
        foreach (var player in ActivePlayers)
        {
            if (player.IsActive)
                player.FixedUpdatePlayer();
        }
    }

    private void SetPlayers()
    {
        int index = 0;
        foreach(KeyValuePair<int, PlayerInput> pInputs in PlayerInputs)
        {
            pInputs.Value.SwitchCurrentActionMap("Player");
            Player player = pInputs.Value.GetComponent<Player>();
            player.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            Vector3 spawnPos = PlayerSpawnPosition[index];
            spawnPos.y += 0.5f;
            player.transform.position = spawnPos;
            player.StartPlayer(pInputs.Key, index);
            ActivePlayers.Add(player);
            index++;
        }
    }

    public bool IsAngel()
    {
        if (angelSelected) return false;
        angelSelected = Random.Range(0, 100) < 50;
        return angelSelected;
    }

    /*
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
*/
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

    public void UpdatePlayerLives(Player player)
    {
        if(player.CurrentLives >= 0) return;
        ActivePlayers.Remove(player);
        InactivePlayers.Add(player);
    }

    public static bool PickUpItem(ItemType itemType)
    {
        if (ItemTypes.Contains(itemType)) return false;

        ItemTypes.Add(itemType);
        if (itemType == ItemType.Crowbar) GameManager.Instance.GameHUDManager.DisplayCrowbar();
        return true;
    }

    public static bool PickUpItem(int noteID)
    {
        if (NoteCount >= 3) return false;

        ItemTypes.Add(ItemType.Note);
        NoteCount++;
        GameManager.Instance.GameHUDManager.DisplayNote(noteID);
        return true;
    }

    public static bool PickUpItem(bool isBlue)
    {
        ItemTypes.Add(ItemType.PowerCell);
        if (isBlue == true && HasBluePowerCell == false) HasBluePowerCell = true;
        else if (isBlue == false && HasGreenPowerCell == false) HasGreenPowerCell = true;
        else return false;
        GameManager.Instance.GameHUDManager.DisplayPowerCell(isBlue);
        return true;
    }

    public static bool HasPowerCells()
    {
        return HasBluePowerCell && HasGreenPowerCell;
    }
}
