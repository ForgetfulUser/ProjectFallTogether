using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MySceneManager : MonoBehaviour
{
    public GameObject Video_GO;
    public PlayerSelectionManager PlayerSelectionManager;
    [SerializeField] private float fadeDuration = 1.0f;
    public GameObject TipScreen_GO;
    public GameObject StartScreen_GO;
    public GameObject Background_Go;
    public static MySceneManager Instance { get; private set; }
    //public List<int> PlayerIDs = new List<int>();
    public Dictionary<int, PlayerInput> playerInputs = new Dictionary<int, PlayerInput>(); // Device | Player Input
    public int sceneIndex;
    bool isLoading = false;
    public GameManager GameManager_PRFB;
    void OnEnable()
    {
        // Subscribe to the event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isLoading = false;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(int sceneIndex)
    {
        if (isLoading) return;
        isLoading = true;
        this.sceneIndex = sceneIndex;
        SceneManager.LoadScene(sceneIndex);
    }


    public void LoadNextScene()
    {
        if (isLoading) return;
        isLoading = true;
        sceneIndex++;
        if (sceneIndex >= SceneManager.sceneCountInBuildSettings) sceneIndex = 0;
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadTestingScene()
    {
        StartCoroutine(StartGameRoutine());
    }

    public IEnumerator StartGameRoutine()
    {
        // Play Vid
        StartScreen_GO.SetActive(false);
        Background_Go.SetActive(false);
 
        Video_GO.SetActive(true);
        double duration = Video_GO.GetComponent<VideoPlayer>().length;
        yield return new WaitForSeconds((float)duration);

        // Display Text
        Destroy(Video_GO);
        TipScreen_GO.SetActive(true);
/*

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;

            // Calculate new alpha and apply it back to the renderer
            float newAlpha = Mathf.Lerp(0, 1, timeElapsed / fadeDuration);
            TipScreen_GO.GetComponent<CanvasGroup>().alpha = newAlpha;

            yield return null;
        }

        TipScreen_GO.GetComponent<CanvasGroup>().alpha = 1;
*/
        yield return new WaitForSeconds(1.7f);
        // Fade it out
        GameManager_PRFB.gameObject.SetActive(true);
        GameManager_PRFB.StartGame();

        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;

            // Calculate new alpha and apply it back to the renderer
            float newAlpha = Mathf.Lerp(1, 0, timeElapsed / fadeDuration);
            TipScreen_GO.GetComponent<CanvasGroup>().alpha = newAlpha;

            yield return null;
        }

        TipScreen_GO.GetComponent<CanvasGroup>().alpha = 0;

        // Players fall
        Destroy(PlayerSelectionManager.PlayerSelectionCanvas_GO);
        Destroy(PlayerSelectionManager.gameObject);
        yield return null;
    }
}