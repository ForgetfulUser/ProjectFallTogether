using System.Collections.Generic;
using UnityEngine;

public class UnlockableObject : MonoBehaviour
{
    public ItemType Type;
    public List<GameObject> Activating_GOs;
    public List<GameObject> Deactivating_GOs;
    public AudioSource AudioSource;

    private ObjectManager ObjectManager;
    private List<Player> players = new List<Player>();
    private List<Player> removedPlayers = new List<Player>();
    private List<Player> removingPlayers = new List<Player>();

    public void StartObject(ObjectManager objectManager)
    {
        ObjectManager = objectManager;
    }

    public void UpdateObject()
    {
        removedPlayers = removingPlayers;
        foreach (Player p in removedPlayers)
        {
            players.Remove(p);
        }
        removedPlayers.Clear();
        removingPlayers.Clear();
        if (players.Count > 0)
        {
            if (UnlockCheck())
            {
                UnlockObject();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player)
        {
            if (players.Contains(player)) return;
            players.Add(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player)
        {
            if (removingPlayers.Contains(player)) return;
            removingPlayers.Add(player);
        }
    }

    public virtual bool UnlockCheck()
    {
        foreach (ItemType type in PlayerManager.ItemTypes)
        {
            if (type == Type)
            {
                return true;
            }
        }
        return false;
    }

    public virtual void UnlockObject()
    {
        foreach(GameObject go in Activating_GOs)
        {
            go.SetActive(true);
        }

        foreach(GameObject go in Deactivating_GOs)
        {
            go.SetActive(false);
        }

        switch (Type)
        {
            case ItemType.Crowbar:
                GameManager.Instance.BarricadeIsGone = true;
                break;
            case ItemType.PowerCell:
                GameManager.Instance.PowerIsOn = true;
                break;
            case ItemType.Note:
                GameManager.Instance.DoorIsUnlocked = true;
                break;

        }
        //if audio, play audio
    }
}
