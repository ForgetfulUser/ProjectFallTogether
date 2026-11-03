using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using TMPro.Examples;
using System.Text.RegularExpressions;

public enum VirusState
{
    Patrol, 
    Detect,
    Search,
    Chase,
    Captured,
    Attack
}

public class Virus : MonoBehaviour
{
    public NavMeshAgent NavMeshAgent;
    [Tooltip("Forces the virus into a state")]
    public VirusState ForcedVirusState;
    // Determines the virus' behaviour. Is it chasing? Patroling? Etc...
    [HideInInspector] public VirusState CurrentVirusState;
    //[Tooltip("How far from the virus is the static/distortion effect?")]
    //public float StaticAbilityRadiusEffect;
    private float DefaultMoveSpeed;
    private float DefaultRotSpeed;

    [Header("Patrolling")]
    [Tooltip("Max time the Virus will spend searching")]
    public float MaxPatrolTime;
    [Tooltip("Min time the Virus will spend searching")]
    public float MinPatrolTime;
    [Tooltip("Time the Virus will pause when reaching a point on their Forst Path")]
    public float PatrolPauseTime;
    [Tooltip("Distance the Virus will stop while Patrolling")]
    public float PatrolStopDistance;
    [Tooltip("Determines if the virus should loop, or follow a line back to the begininng")]
    public bool ShouldLoop = true;
    // The current Path the Virus is patroling
    public List<Transform> ForestPath = new List<Transform>();
    private bool isPatrolling;
    private bool patrolForward = true;
    private int pointIndex;

    [Header("Searching")]
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

    [Header("Chasing")]
    [Tooltip("How far can the player run before the chase stops?")]
    public float ChaseBreakDistance;
    [Tooltip("Distance the Virus will stop while Chasing")]
    public float ChaseStopDistance;
    [Tooltip("How fast the virus moves")]
    public float ChaseMoveSpeed;
    [Tooltip("How fast the virus rotates")]
    public float ChaseRotSpeed;

    [Header("Detection")]
    [Tooltip("Max time the Virus will spend searching")]
    public float MaxDetectTime;
    [Tooltip("Min time the Virus will spend searching")]
    public float MinDetectTime;
    [Tooltip("How far the Virus will detect while not in Detection mode")]
    public float BaseDetectionDistance;
    [Tooltip("Determines how the detection radius grows per second")]
    public float DetectionGrowRate;
    [Tooltip("Thing that Detects the players")]
    public DetectionArea DetectionArea;

    [Header("Attack")]
    [Tooltip("Time between attacks")]
    public float AttackDelay = 2;

    [Header("Snare")]
    [Tooltip("How close the virus must get before they can snare a player")]
    public float SnareDistance = 8;
    public DigitalSnare DigitalSnare_PRFB;
    public DigitalSnare CurrentSnare;

    private float currentStateTime;
    [HideInInspector] public Player DetectedPlayer;
    public Coroutine CurrentCoroutine;
    private VirusManager VirusManager;

    public void StartVirus(VirusManager virusManger)
    {
        DefaultMoveSpeed = GetComponent<NavMeshAgent>().speed;
        DefaultRotSpeed = GetComponent<NavMeshAgent>().angularSpeed;
        VirusManager = virusManger;
        ChangeState();
    }

    public void UpdateVirus()
    {
        if(CurrentVirusState != ForcedVirusState)
        {
            CurrentVirusState = ForcedVirusState;
            ChangeState();
        }

        if (CurrentVirusState == VirusState.Captured) return;
        if (currentStateTime > 0)
        {
            currentStateTime -= Time.deltaTime;
        }
        else if(CurrentVirusState == VirusState.Search || CurrentVirusState == VirusState.Detect)
        {
            DetermineState();

            if (CurrentCoroutine != null) return;
        }

        // Determine appropiate state
        if(CurrentVirusState != VirusState.Detect)
            DetectionArea.transform.localScale = Vector3.one * BaseDetectionDistance;
        // Use Switch to call the state's functions
        switch (CurrentVirusState)
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
            case VirusState.Attack:
                Attack();
                break;
        }
    }

    private void DetermineState()
    {
        if (CurrentCoroutine != null)
        {
            StopCoroutine(CurrentCoroutine);
            CurrentCoroutine = null;
        }

        if (CurrentVirusState == VirusState.Captured || CurrentVirusState == VirusState.Chase) return;
        CurrentVirusState = (VirusState)Random.Range(0, 3);
        //if ((CurrentVirusState == VirusState.Captured || CurrentVirusState == VirusState.Chase) && CurrentVirusState != VirusState.Captured) CurrentVirusState = VirusState.Search;
        //VirusState = VirusState.Patrol;
        
        ChangeState();
    }

    public void ChangeState()
    {
        NavMeshAgent.angularSpeed = DefaultRotSpeed;
        NavMeshAgent.speed = DefaultMoveSpeed;
        //if (CurrentVirusState == VirusState.Chase || CurrentVirusState == VirusState.Attack || CurrentVirusState == VirusState.Detect) CurrentVirusState = VirusState.Search;
        ForcedVirusState = CurrentVirusState;
        switch (CurrentVirusState)
        {
            case VirusState.Patrol:
                isPatrolling = true;
                NavMeshAgent.stoppingDistance = PatrolStopDistance;
                ForestPath = VirusManager.GetClosestPath(transform.position, out Transform closestPoint, out pointIndex);
                if (ForestPath == null) DetermineState();
                NavMeshAgent.destination = closestPoint.position;
                patrolForward = true;
                if(CurrentCoroutine != null) StopCoroutine(CurrentCoroutine); 
                CurrentCoroutine = StartCoroutine(PatrolToPoint());
                currentStateTime = Random.Range(MinPatrolTime, MaxPatrolTime);
                break;
            case VirusState.Detect:
                StopNavMeshAgent();
                currentStateTime = Random.Range(MinDetectTime, MaxDetectTime);
                break;
            case VirusState.Chase:
                NavMeshAgent.angularSpeed = ChaseRotSpeed;
                NavMeshAgent.speed = ChaseMoveSpeed;
                NavMeshAgent.stoppingDistance = ChaseStopDistance;
                break;
            case VirusState.Search:
                NavMeshAgent.stoppingDistance = PatrolStopDistance;
                isSearching = false;
                currentStateTime = Random.Range(MinSearchTime, MaxSearchTime);
                break;
            case VirusState.Captured:
                StopNavMeshAgent();
                //Capture();
                break;
            case VirusState.Attack:
                
                if (CurrentCoroutine != null)
                {
                    StopCoroutine(CurrentCoroutine);
                    CurrentCoroutine = null;
                }
                break;
        }
    }

    public void StopNavMeshAgent()
    {
        DetectedPlayer = null;
        if(CurrentSnare  != null)
        {
            Destroy(CurrentSnare.gameObject);
            CurrentSnare = null;
        }
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
        yield return new WaitUntil(() => Vector3.Distance(ForestPath[pointIndex].position, transform.position) <= NavMeshAgent.stoppingDistance);
        yield return new WaitForSeconds(PatrolPauseTime);
        isPatrolling = false;
        CurrentCoroutine = null;

    }

    void NextPoint()
    {
        if (PatrolCheck() == false) return;

        if (ShouldLoop == false)
        {
            if (pointIndex >= ForestPath.Count - 1 || pointIndex <= 0) patrolForward = !patrolForward;
        }
        else if (pointIndex >= ForestPath.Count - 1) pointIndex = -1;

        if (patrolForward) pointIndex++;
            else pointIndex--;

        NavMeshAgent.destination = ForestPath[pointIndex].position;
    }

    bool PatrolCheck()
    {
        if (ForestPath.Count <= 0)
        {
            CurrentVirusState = VirusState.Patrol;
            return false;
        }
        return true;
    }

    void Detection()
    {
        // Uses a detection to attempt locating a player
        Vector3 scaleAmount = new Vector3(DetectionGrowRate, DetectionGrowRate, DetectionGrowRate) * Time.deltaTime;
        DetectionArea.transform.localScale += scaleAmount;
        foreach(Player player in DetectionArea.DetectedPlayers)
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
        if(player.CurrentLives <= 0) return;
        if (Vector3.Distance(transform.position, player.transform.position) >= ChaseBreakDistance) return;
        DetectionArea.DetectedPlayers.Clear();

        if (CurrentCoroutine != null)
        {
            StopCoroutine(CurrentCoroutine);
            CurrentCoroutine = null;
        }
        DetectedPlayer = player;
        if(DetectedPlayer != null)
        {
            CurrentVirusState = VirusState.Chase;
            ChangeState();
        }
        else
        {
            StopNavMeshAgent();
            DetermineState();
        }
    }

    void Chase()
    {
        // Change targets or change states
        if (Vector3.Distance(transform.position, DetectedPlayer.transform.position) >= ChaseBreakDistance)
        {
            Player nextTarget = DetectionArea.GetNextTarget();
            if (nextTarget)
            {
                DetectedPlayer = nextTarget;
            }
            else
            {
                StopNavMeshAgent();
                CurrentVirusState = VirusState.Search;
                ChangeState();
                return;
            }
        }

        // Pursue detected player
        NavMeshAgent.destination = DetectedPlayer.transform.position;

        if (Vector3.Distance(transform.position, DetectedPlayer.transform.position) <= ChaseStopDistance)
        {
            CurrentVirusState = VirusState.Attack;
            ChangeState();
            SpawnDigitalSnare();
        }

    }

    public void SpawnDigitalSnare()
    {
        if(DetectedPlayer == null || CurrentSnare != null) return;

        CurrentSnare = Instantiate(DigitalSnare_PRFB, DetectedPlayer.transform);//.position, Quaternion.identity);
        DetectedPlayer.Snare(CurrentSnare);
        CurrentSnare.Snare(this, DetectedPlayer);
        //DetectedPlayer = null;
    }

    void Attack()
    {
        if(DetectedPlayer != null && Vector3.Distance(transform.position, DetectedPlayer.transform.position) <= ChaseBreakDistance) 
            NavMeshAgent.destination = DetectedPlayer.transform.position;
        else 
            DetermineState();

        if (NavMeshAgent.remainingDistance >= NavMeshAgent.stoppingDistance) return;

        if (DetectedPlayer == null || DetectedPlayer.CurrentLives <= 0)
        {
            CurrentVirusState = VirusState.Search;
            if (CurrentCoroutine != null)
            {
                StopCoroutine(CurrentCoroutine);
                CurrentCoroutine = null;
            }
            if (CurrentSnare != null)
            {
                Destroy(CurrentSnare.gameObject);
                CurrentSnare = null;
            }
            return;
        }

        if (CurrentCoroutine != null) return;
        Debug.Log("Attack");
        DetectedPlayer.GetBopped();
        CurrentCoroutine = StartCoroutine(AttackCoroutine());   
    }

    IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(AttackDelay);
        CurrentCoroutine = null;
        if (Vector3.Distance(DetectedPlayer.transform.position, transform.position) <= NavMeshAgent.stoppingDistance && CurrentVirusState == VirusState.Chase)
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
        if (CurrentVirusState == VirusState.Captured)
        {
            CurrentVirusState = VirusState.Detect;
        }
        else
        {
            CurrentCoroutine = null;
            CurrentVirusState = VirusState.Captured;
            ChangeState();
        }
    }

    void StaticAbility()
    {
        // Uses StaticAbilityRadiusEffect to affect nearby object with static ability
        // Increased Static and Distortion when chasing
    }
}
