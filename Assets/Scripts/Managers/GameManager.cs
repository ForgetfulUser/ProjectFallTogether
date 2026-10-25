using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public MySceneManager MySceneManager;
    public PlayerManager PlayerManager;
    public CameraManager CameraManager;
    public VirusManager VirusManager;
    public GameHUDManager GameHUDManager;

    public int Minutes;
    private int startMinutes;
    public float Seconds;
    private float preGameTimer = 3f;
    private float preGameTimerMulti = 1.2f;
    private float startSeconds;
    private bool isGameRunning = true;
    private bool isPreGame = true;
    public GameObject SplashScreen;
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

        if (skipPreGame)
        {
            preGameTimer = 0f;
        }
    }

    protected virtual void Update()
    {
        //TODO: Make a Coroutine to replace the timer. Replace it with: if(isPreGame) return;
        if (preGameTimer >= 0)
        {
            preGameTimer -= Time.deltaTime * preGameTimerMulti;

            if (preGameTimer <= 1f)
            {
                isPreGame = false;
            }
            GameHUDManager.UpdatePreGameTimer(preGameTimer);
        }

        if (SplashScreen) Destroy(SplashScreen);
        if (isGameRunning == false) return;

        PlayerManager.UpdateManager();
        VirusManager.UpdateManager();

        if (enableDebugReset && Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            VirusManager.ResetVirus();
            Debug.Log("Hazards reset.");
        }

        if (WinCheck() != null)
        {
            Seconds = 0;
            Minutes = 0;
        }

        Seconds -= Time.deltaTime;
        if (Seconds < 0)
        {
            Minutes--;
            Seconds = 60;
        }
        if (Minutes < 0)
        {
            isGameRunning = false;
            GameOver();
        }

        CameraManager.UpdateCamera();
    }

    protected virtual void FixedUpdate()
    {
        if (isGameRunning == false || isPreGame == true) return;
        PlayerManager.FixedUpdateManager();
        //HazardManager.FixedUpdateManager();
    }

    protected virtual void LateUpdate()
    {
        GameHUDManager.UpdateHUD(Minutes, Seconds);
    }

    protected virtual void GameOver()
    {
        StartCoroutine(GameOverCoroutine());
    }

    private IEnumerator GameOverCoroutine()
    {
        isGameRunning = false;
        int finalPlayerIndex = PlayerManager.lastPlayerStanding ? PlayerManager.lastPlayerStanding.PlayerID : -1;
        GameHUDManager.GameOver(finalPlayerIndex);
        yield return new WaitForSeconds(0.7f);
        MySceneManager.Instance.LoadNextScene();
    }


    protected virtual void ResetGame()
    {
        PlayerManager.ResetManager();
        VirusManager.ResetVirus();
        Minutes = startMinutes;
        Seconds = startSeconds;
    }

    protected virtual Player WinCheck()
    {
        return PlayerManager.LastPlayerStanding();
    }
}
