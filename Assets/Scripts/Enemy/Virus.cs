using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum VirusState
{
    Patrol, 
    Detect,
    Chase,
    Search,
    Captured
}

public class Virus : MonoBehaviour
{
    // Game Manager:
    //      Landmarks

    // Forest Paths
    [Tooltip("Determines the virus' behaviour. Is it chasing? Patroling? Etc...")]
    public VirusState VirusState;
    [Tooltip("How far from the virus is the static/distortion effect?")]
    public float StaticAbilityRadiusEffect;
    public NavMeshAgent NavMeshAgent;

    [Header("Search Variables")]
    [Tooltip("Max Distance from the Virus it will search")]
    public float MaxSearchDistance;
    [Tooltip("Min Distance from the Virus it will search")]
    public float MinSearchDistance;
    [Tooltip("How long the Virus stays at a point, searching")]
    public float SearchTime;
    private Vector3 searchPoint;
    private float searchDistance;
    private bool isSearching;

    [Header("Chase Variables")]
    [Tooltip("How far can the player run before the chase stops?")]
    public float ChaseBreakDistance;

    [Header("Detection Variables")]
    public Player DetectedPlayer;
    [Tooltip("Determines how the detection radius grows per second")]
    public float DetectionGrowRate;
    [Tooltip("Thing that Detects the players")]
    public DectionArea DetectionArea;

    public Coroutine CurrentCoroutine;
    private VirusManager VirusManager;
    public void StartVirus(VirusManager virusManger)
    {
        VirusManager = virusManger;
        VirusState = VirusState.Detect;
    }

    public void UpdateVirus()
    {
        if (CurrentCoroutine != null) return;

        // Determine appropiate state

        // Use Switch to call the state's functions
        switch (VirusState)
        {
            case VirusState.Patrol:
                Patrol();
                break;
            case VirusState.Detect:
                Detection();
                break;
            case VirusState.Chase:
                Chase();
                break;
            case VirusState.Search:
                Search();
                break;
            case VirusState.Captured:
                StopNavMeshAgent();
                break;
        }
    }

    private void StopNavMeshAgent()
    {
        NavMeshAgent.SetDestination(transform.position);
        NavMeshAgent.isStopped = true;
        NavMeshAgent.ResetPath();
    }

    void Patrol()
    {
        // Wander Forest Path
        // Randomly appear new a landmark
    }

    void Detection()
    {
        // Uses a detection to attempt locating a player
        Vector3 scaleAmount = new Vector3(DetectionGrowRate, DetectionGrowRate, DetectionGrowRate) * Time.deltaTime;
        DetectionArea.transform.localScale += scaleAmount;
        foreach(Player player in DetectionArea.Players)
        {
            if(Vector3.Distance(transform.position, player.transform.position) < ChaseBreakDistance)
            {
                DetectPlayer(player);
                break;
            }
        }
    }

    public void DetectPlayer(Player player)
    {
        if (Vector3.Distance(transform.position, player.transform.position) >= ChaseBreakDistance) return;
        DetectionArea.transform.localScale = Vector3.one;
        DetectionArea.Players.Clear();

        if (CurrentCoroutine != null) StopCoroutine(CurrentCoroutine);
        DetectedPlayer = player;
        if(DetectedPlayer != null)
        {
            VirusState = VirusState.Chase;

        }
        else
        {
            // TODO: Have a function to determine VirusState
            StopNavMeshAgent();
            VirusState = VirusState.Patrol;
        }
    }

    void Chase()
    {
        // Pursue detected player
        NavMeshAgent.destination = DetectedPlayer.transform.position;
        if (Vector3.Distance(transform.position, DetectedPlayer.transform.position) >= ChaseBreakDistance)
        {
            StopNavMeshAgent();
            VirusState = VirusState.Search;
        }
    }

    void Captured()
    {
        // Temporarily unable to move
    }

    void Search()
    {
        if(isSearching == false)
        {
            searchDistance = Random.Range(MinSearchDistance, MaxSearchDistance);
            Vector3 searchDirection = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
            searchPoint = (searchDirection * searchDistance) + transform.position;
            isSearching = true;
        }
        if(CurrentCoroutine == null)
        {
            CurrentCoroutine = StartCoroutine(SearchPoint());
        }
    }

    public IEnumerator SearchPoint()
    {
        NavMeshAgent.SetDestination(searchPoint);
        yield return NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance + 1;
        // Enter Searching Animation
        yield return new WaitForSeconds(SearchTime);
        isSearching = false;
        CurrentCoroutine = null;
    }

    void StaticAbility()
    {
        // Uses StaticAbilityRadiusEffect to affect nearby object with static ability
        // Increased Static and Distortion when chasing
    }
}
