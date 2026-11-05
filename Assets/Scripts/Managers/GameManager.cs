using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Managers")]
    public MySceneManager MySceneManager;
    public PlayerManager PlayerManager;
    public CameraManager CameraManager;
    public VirusManager VirusManager;
    public GameHUDManager GameHUDManager;
    public ObjectManager ObjectManager;

    [Header("Key Points")]
    public bool PowerIsOn = false;
    public bool BarricadeIsGone = false;
    public bool DoorIsUnlocked = false;
    public bool DoorIsOpen = false;

    [Header("Time")]
    public int Minutes;
    private int startMinutes;
    public float Seconds;
    private float startSeconds;

    [Header("Gamestate Variables ?")]
    private bool isGameRunning = true;
    private bool isPreGame = true;
    public GameObject SplashScreen;

    [Header("Endgame References")]
    public GameObject PlayerHUD_GO;
    public Image GameOverFadeIn_IMG; 
    public VideoPlayer WinCutscene;
    public VideoPlayer LoseCutscene;
    
    [Header("Debug")]
    public bool enableDebugReset = true;
    public bool skipPreGame = false;

    protected virtual void Awake()
    {
        startMinutes = Minutes;
        startSeconds = Seconds;
        Instance = this;
        //HazardManager.WakeUpManager();
    }

    public virtual void StartGame()
    {
        PlayerManager.StartManager();
        VirusManager.StartManager();
        GameHUDManager.StartHUD(PlayerManager.ActivePlayers);
        ObjectManager.StartManager();

        if (SplashScreen) Destroy(SplashScreen);
    }

    protected virtual void Update()
    {
        if (isGameRunning == false) return;
        PlayerManager.UpdateManager();
        ObjectManager.UpdateManager();
        VirusManager.UpdateManager();
        CameraManager.UpdateCamera();


        if (WinCheck() || PlayerManager.ActivePlayers.Count == 0)
        {
            GameOver();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (isGameRunning == false) return;
        PlayerManager.FixedUpdateManager();
        //HazardManager.FixedUpdateManager();
    }

    protected virtual void LateUpdate()
    {
        if (isGameRunning == false) return;
        GameHUDManager.UpdateHUD(Minutes, Seconds);
    }

    public virtual void GameOver()
    {
        isGameRunning = false;
        VirusManager.Virus.StopNavMeshAgent();
        StartCoroutine(GameOverCoroutine());
    }

    private IEnumerator GameOverCoroutine()
    {
        isGameRunning = false;
        //GameHUDManager.GameOver(PlayerManager.NoPlayerStanding());

        VideoPlayer video = PlayerManager.NoPlayerStanding() ? LoseCutscene : WinCutscene;

        video.gameObject.SetActive(true);

        yield return new WaitForSeconds((float)video.length);
        MySceneManager.Instance.LoadNextScene();
    }


    protected virtual void ResetGame()
    {
        PlayerManager.ResetManager();
        VirusManager.ResetVirus();
        Minutes = startMinutes;
        Seconds = startSeconds;
    }

    protected virtual bool WinCheck()
    {
        return DoorIsOpen;
    }
}
