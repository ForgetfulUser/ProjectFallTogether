using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerSelectionManager : MonoBehaviour
{
    //public Transform MenuOptions_TRSFM;
    public GameObject PlayerSelectionCanvas_GO;
    public GameObject CreditsCanvas_GO;
    public Button StartGame_BTN;
    public Button Credit_BTN;
    public Button CreditBack_BTN;
    public Color Color;
    public List<Image> PlayerSelections_IMG = new List<Image>();
    public List<Image> PlayerProfile_IMG = new List<Image>();
    public Dictionary<int, PlayerInput> playerInputs = new Dictionary<int, PlayerInput>(); // Device | Player ID
    public List<int> DeviceIDs = new List<int>();
    private int currentID;
    [Tooltip("Set this to true if you're playtesting.")]
    public bool IsTesting;
    private void Start()
    {
        ColorBlock colorBlock = CreditBack_BTN.colors;
        colorBlock.selectedColor = Color;
        CreditBack_BTN.colors = colorBlock;
        Credit_BTN.colors = colorBlock;
        StartGame_BTN.colors = colorBlock;
    }

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
        }
    }

    public void SelectPlayer(PlayerInput playerInput)
    {
        for (int i = 0; i < playerInput.devices.Count; i++)
        {
            if (DeviceIDs.Contains(playerInput.devices[i].deviceId)) return;
        }
        //StartGame_BTN.Select();
        //StartGame_BTN.image.color = StartGame_BTN.colors.selectedColor;
        PlayerSelections_IMG[currentID].color = Color.green;
        PlayerProfile_IMG[currentID].color = Color.white;
        int deviceID = playerInput.devices[0].deviceId;
        playerInputs.Add(deviceID, playerInput);
        DeviceIDs.Add(deviceID);
        currentID++;
    }

    public void ToggleCredits()
    {
        bool isCurrentlyCredits = CreditsCanvas_GO.activeSelf;
        PlayerSelectionCanvas_GO.SetActive(isCurrentlyCredits);
        CreditsCanvas_GO.SetActive(!isCurrentlyCredits);
        if (isCurrentlyCredits) StartGame_BTN.Select();
        else CreditBack_BTN.Select();
    }
}
