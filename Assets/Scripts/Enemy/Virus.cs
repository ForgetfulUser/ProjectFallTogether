using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using TMPro.Examples;

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

    [Tooltip("Determines the virus' behaviour. Is it chasing? Patroling? Etc...")]
    public VirusState VirusState;
    [Tooltip("How far from the virus is the static/distortion effect?")]
    public float StaticAbilityRadiusEffect;
    public NavMeshAgent NavMeshAgent;

    [Header("Patrol Variable")]
    [Tooltip("Max time the Virus will spend searching")]
    public float MaxPatrolTime;
    [Tooltip("Min time the Virus will spend searching")]
    public float MinPatrolTime;
    [Tooltip("Time the Virus will pause when reaching a point on their Forst Path")]
    public float PatrolPauseTime;
    [Tooltip("Distance the Virus will stop while Patrolling")]
    public float PatrollingStopDistance;
    // The current Path the Virus is patroling
    [HideInInspector] public List<Transform> ForestPath = new List<Transform>();
    private bool isPatrolling;
    private int pointIndex;

    [Header("Search Variables")]
    [Tooltip("Max time the Virus will spend searching")]
    public float MaxSearchTime;
    [Tooltip("Min time the Virus will spend searching")]
    public float MinSearchTime;
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
    [Tooltip("Max time the Virus will spend searching")]
    public float MaxChaseTime;
    [Tooltip("Min time the Virus will spend searching")]
    public float MinChaseTime;
    [Tooltip("How far can the player run before the chase stops?")]
    public float ChaseBreakDistance;
    [Tooltip("Distance the Virus will stop while Chasing")]
    public float ChasingStopDistance;

    [Header("Detection Variables")]
    [Tooltip("Max time the Virus will spend searching")]
    public float MaxDetectTime;
    [Tooltip("Min time the Virus will spend searching")]
    public float MinDetectTime;
    [Tooltip("How far the Virus will detect while not in Detection mode")]
    public float BaseDetectionDistance;
    [Tooltip("Determines how the detection radius grows per second")]
    public float DetectionGrowRate;
    [Tooltip("Thing that Detects the players")]
    public DectionArea DetectionArea;

    [Header("Snare References")]
    [Tooltip("How close the virus must get before they can snare a player")]
    public float SnareDistance = 8;
    public DigitalSnare DigitalSnare_PRFB;
    public DigitalSnare CurrentSnare;

    [Header("Attack References")]
    [Tooltip("Time between attacks")]
    public float AttackDelay = 2;

    private float currentStateTime;
    [HideInInspector] public Player DetectedPlayer;
    public Coroutine CurrentCoroutine;
    private VirusManager VirusManager;
    public void StartVirus(VirusManager virusManger)
    {
        VirusManager = virusManger;
    }

    public void UpdateVirus()
    {

        if (currentStateTime > 0)
        {
            currentStateTime -= Time.deltaTime;
        }
        else
        {
            DetermineState();
            if (CurrentCoroutine != null)
            {
                StopCoroutine(CurrentCoroutine);
                CurrentCoroutine = null;
            }
        }
        if (CurrentCoroutine != null) return;

        // Determine appropiate state
        if(VirusState != VirusState.Detect)
            DetectionArea.transform.localScale = Vector3.one * BaseDetectionDistance;
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

    private void DetermineState()
    {
        if (VirusState == VirusState.Captured || VirusState == VirusState.Chase) return;
        VirusState = (VirusState)Random.Range(0, 5);
        if (VirusState == VirusState.Captured || VirusState == VirusState.Chase) VirusState = VirusState.Search;

        ChangeState();
    }

    public void ChangeState()
    {
        switch (VirusState)
        {
            case VirusState.Patrol:
                isPatrolling = false;
                currentStateTime = Random.Range(MinPatrolTime, MaxPatrolTime);
                break;
            case VirusState.Detect:
                StopNavMeshAgent();
                currentStateTime = Random.Range(MinDetectTime, MaxDetectTime);
                break;
            case VirusState.Chase:
                currentStateTime = Random.Range(MinChaseTime, MaxChaseTime);
                break;
            case VirusState.Search:
                isSearching = false;
                currentStateTime = Random.Range(MinSearchTime, MaxSearchTime);
                break;
            case VirusState.Captured:

                break;
        }
    }

    private void StopNavMeshAgent()
    {
        DetectedPlayer = null;
        NavMeshAgent.SetDestination(transform.position);
        NavMeshAgent.isStopped = true;
        NavMeshAgent.ResetPath();
    }

    void Patrol()
    {
        // Wander Forest Path
        if (isPatrolling == false)
        {
            NextPoint();
            CurrentCoroutine = StartCoroutine(PatrolToPoint());
        }
        // Randomly appear new a landmark
    }

    IEnumerator PatrolToPoint()
    {
        isPatrolling = true;
        yield return new WaitUntil(() => NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance);
        yield return new WaitForSeconds(PatrolPauseTime);
        isPatrolling = false;
        CurrentCoroutine = null;

    }

    void NextPoint()
    {
        if (PatrolCheck() == false) return;
        NavMeshAgent.stoppingDistance = PatrollingStopDistance;
        pointIndex++;
        if(pointIndex >= ForestPath.Count) pointIndex = 0;
        NavMeshAgent.destination = ForestPath[pointIndex].position;
    }

    bool PatrolCheck()
    {
        if (ForestPath == null)
        {
            ForestPath = VirusManager.GetClosestPath();
            if (ForestPath == null) return false;
            NavMeshAgent.destination = VirusManager.GetClosestPoint().position;
            VirusState = VirusState.Patrol;
            return false;
        }
        return true;
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
        DetectionArea.Players.Clear();

        if (CurrentCoroutine != null)
        {
            StopCoroutine(CurrentCoroutine);
            CurrentCoroutine = null;
        }
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
        NavMeshAgent.stoppingDistance = ChasingStopDistance;
        NavMeshAgent.destination = DetectedPlayer.transform.position;

        if (Vector3.Distance(transform.position, DetectedPlayer.transform.position) >= ChaseBreakDistance)
        {
            StopNavMeshAgent();
            VirusState = VirusState.Search;
        }

        if(NavMeshAgent.remainingDistance <= SnareDistance)
        {
            SpawnDigitalSnare();
            //transform.position = Vector3.zero;
            //VirusState = VirusState.Search;
        }

        if(NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance)
        {
            Attack();
        }
    }

    public void SpawnDigitalSnare()
    {
        if(DetectedPlayer == null || CurrentSnare != null || VirusState != VirusState.Chase) return;

        CurrentSnare = Instantiate(DigitalSnare_PRFB, DetectedPlayer.transform.position, Quaternion.identity);

        //DetectedPlayer = null;
    }

    void Attack()
    {
        if(DetectedPlayer == null || DetectedPlayer.CurrentLives <= 0)
        {
            VirusState = VirusState.Search;
            CurrentCoroutine = null;
            return;
        }
        if(CurrentCoroutine != null)
        {
            StopCoroutine(CurrentCoroutine);
            CurrentCoroutine = null;
        }
        int amount = 4;
        DetectedPlayer.GetBopped(amount);
        CurrentCoroutine = StartCoroutine(AttackCoroutine());   
    }

    IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(AttackDelay);
        CurrentCoroutine = null;
        if (Vector3.Distance(DetectedPlayer.transform.position, transform.position) <= NavMeshAgent.stoppingDistance && VirusState == VirusState.Chase)
            Attack();
        yield return null;
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
        yield return new WaitUntil(() => NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance + 1);
        // Enter Searching Animation
        yield return new WaitForSeconds(SearchTime);
        isSearching = false;
        CurrentCoroutine = null;
    }

    public void ToggleCapture()
    {
        if (VirusState == VirusState.Captured)
        {
            VirusState = VirusState.Detect;
        }
        else
        {
            CurrentCoroutine = null;
            VirusState = VirusState.Captured;
        }
    }

    void StaticAbility()
    {
        // Uses StaticAbilityRadiusEffect to affect nearby object with static ability
        // Increased Static and Distortion when chasing
    }
}
