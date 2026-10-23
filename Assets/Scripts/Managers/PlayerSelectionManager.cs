using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerSelectionManager : MonoBehaviour
{
    //public Transform MenuOptions_TRSFM;
    public GameObject PlayerSelectionCanvas_GO;
    public List<Image> PlayerSelections_IMG = new List<Image>();
    public Dictionary<int, PlayerInput> playerInputs = new Dictionary<int, PlayerInput>(); // Device | Player ID
    public List<int> DeviceIDs = new List<int>();
    private int currentID;
    [Tooltip("Set this to true if you're playtesting.")]
    public bool IsTesting;

    public void StartGame()
    {
        MySceneManager.Instance.playerInputs = playerInputs;
        if (IsTesting == false)
        {
            MySceneManager.Instance.LoadNextScene();
        }
        else
        {
            MySceneManager.Instance.LoadTestingScene();
            Destroy(PlayerSelectionCanvas_GO);
            Destroy(gameObject);
        }
    }

    public void SelectPlayer(PlayerInput playerInput)
    {
        for (int i = 0; i < playerInput.devices.Count; i++)
        {
            if (DeviceIDs.Contains(playerInput.devices[i].deviceId)) return;
        }

        PlayerSelections_IMG[currentID].color = Color.green;
        int deviceID = playerInput.devices[0].deviceId;
        playerInputs.Add(deviceID, playerInput);
        DeviceIDs.Add(deviceID);
        currentID++;
    }
}
