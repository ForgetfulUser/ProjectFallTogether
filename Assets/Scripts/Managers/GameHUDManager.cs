using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameHUDManager : MonoBehaviour
{
    [Header("Player Lives")]
    public List<Image> JustinLivesSpriteRender = new List<Image>();
    public List<Image> LucyLivesSpriteRender = new List<Image>();
    public List<Image> TristenLivesSpriteRender = new List<Image>();
    public List<Image> VanessaLivesSpriteRender = new List<Image>();

    private Dictionary<int, List<Image>> allPlayerLives = new Dictionary<int, List<Image>>();

    public TMP_Text Timer_TXT;
    public Image ReadySetGo_IMG;
    public GameObject GameOver_GO;
    public TMP_Text GameOver_TXT;
    private List<Player> players;

    public void StartHUD(List<Player> players)
    {
        Screen.SetResolution(1920, 1080, true);
        this.players = players;
        allPlayerLives.Add(0, JustinLivesSpriteRender);
        allPlayerLives.Add(1, LucyLivesSpriteRender);
        allPlayerLives.Add(2, TristenLivesSpriteRender);
        allPlayerLives.Add(3, VanessaLivesSpriteRender);
    }

    public void UpdateHUD(int minutes, float seconds)
    {
        //UpdateTimer(minutes, seconds);

        //UpdatePlayerLives();
    }

    public void UpdateTimer(int minutes, float seconds)
    {
        int secs = Mathf.RoundToInt(seconds);
        if (secs == 60) secs = 59;
        if (minutes < 0) minutes = 0;
        string text = minutes + ":";
        if (secs < 10) text += "0" + secs;
        else text += secs;
        Timer_TXT.text = text;
    }

    public void UpdatePlayerLives(Player player)
    {
        int playerIndex = player.PlayerIndex;

        List<Image> playerLives = this.allPlayerLives[playerIndex];
        for(int i = 2; i >= 0; i--)
        {
            if(i >= player.CurrentLives)
            {
                playerLives[i].color = Color.red;
            }
        }
    }

    public void GameOver(bool noPlayerStanding)
    {
        GameOver_TXT.text = noPlayerStanding 
            ? "Y'ALL MADE IT!!!"
            : "THE VIRUS GOT Y'ALL!!";
        GameOver_GO.SetActive(true);
    }
}