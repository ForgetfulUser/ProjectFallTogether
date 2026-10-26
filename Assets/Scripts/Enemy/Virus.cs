using NUnit.Framework.Internal.Commands;
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
            VirusState = VirusState.Patrol;
        }
    }

    void Chase()
    {
        // Pursue detected player
        NavMeshAgent.destination = DetectedPlayer.transform.position;
    }

    void Captured()
    {
        // Temporarily unable to move
    }

    void Search()
    {
        // Occurs if the player escapes their detection or if the player 
        // Investigates player's last known position
    }

    void StaticAbility()
    {
        // Uses StaticAbilityRadiusEffect to affect nearby object with static ability
        // Increased Static and Distortion when chasing
    }
}
