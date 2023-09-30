using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public enum AIStates
{
    Patrol,
    Pursuit
}

public class Enemy : MonoBehaviour
{
    //public SteamId steamId = 0;

    [Header("AI")]
    public GameObject targetPlayer;
    public System.Type targetDetection = null;
    public AIStates state = AIStates.Patrol;
    [SerializeField] private float pursuitAftertime;
    [SerializeField] private int patrolSearchPoints;
    [SerializeField] private float patrolSearchTime;
    [SerializeField] private int pursuitSearchPoints;
    [SerializeField] private float pursuitSearchTime;
    //[SerializeField] private float attackDistance = 1f;

    [Header("Movement Constants")]
    [SerializeField] private float MoveSpeed = 4f;
    [SerializeField] private float SprintSpeed = 8f;

    [Header("Movement")]
    [SerializeField] private float targetSpeed;
    [SerializeField] private Vector2 moveDirection;
    [SerializeField] private Vector3 moveVelocity;
    [SerializeField] private Vector2 animVelocity;
    [SerializeField] private bool sprint;

    [Header("Debug")]
    [SerializeField] private bool standStill;
    [SerializeField] private bool pursuitTransform;
    [SerializeField] private Transform destination;

    NavMeshAgent agent;
    Animator animator;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        StartCoroutine(Patrol());
    }

    void Update()
    {
        if (!standStill)
        {
            if (pursuitTransform) agent.SetDestination(destination.position);

            Move();
            Animate();
        }
    }

    public void Animate()
    {
        animator.SetFloat("Velocity", agent.velocity.magnitude);
    }

    void Move()
    {
        targetSpeed = sprint ? SprintSpeed : MoveSpeed;
        agent.speed = targetSpeed;
    }

    #region States
    public void TargetFound(System.Type type, GameObject player)
    {
        if (targetPlayer == player) return;
        if (targetDetection != null)
            if (targetDetection == typeof(EnemyVision) && type == typeof(EnemyHearing)) return;
        targetDetection = type;
        targetPlayer = player;
        Debug.Log(player.GetComponent<Player>().steamId.Value);
        Debug.Log(targetDetection.ToString());
        StopAllCoroutines();
        StartCoroutine(Pursuit());
    }

    public void TargetLost(System.Type type)
    {
        if (targetDetection != type) return;
        StartCoroutine(PursuitAfterburner());
    }

    IEnumerator Patrol()
    {
        state = AIStates.Patrol;
        sprint = false;

        var navMeshWaypoints = GameObject.FindGameObjectsWithTag("NavMeshWaypoint");
        agent.SetDestination(navMeshWaypoints[Random.Range(0, navMeshWaypoints.Length)].transform.position);

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        yield return new WaitForSeconds(2);
        StartCoroutine(Search(patrolSearchPoints, patrolSearchTime));
        yield break;
    }

    IEnumerator Pursuit()
    {
        state = AIStates.Pursuit;
        sprint = true;

        agent.SetDestination(targetPlayer.transform.position);
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            if (targetPlayer) agent.SetDestination(targetPlayer.transform.position);
            yield return null;
        }

        if (targetPlayer) StartCoroutine(Kill());
        else
        {
            yield return new WaitForSeconds(2);
            StartCoroutine(Search(pursuitSearchPoints, pursuitSearchTime));
        }
        yield break;
    }

    IEnumerator PursuitAfterburner()
    {
        yield return new WaitForSeconds(pursuitAftertime);
        targetDetection = null;
        targetPlayer = null;
        yield break;
    }

    IEnumerator Search(int searchPoints, float waitTime)
    {
        bool GetRandomPoint(float maxRange, out Vector3 result)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * maxRange;
            randomPoint.Set(randomPoint.x, 0.5f, randomPoint.z);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 100.0f, NavMesh.AllAreas))
            {
                //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
                //or add a for loop like in the documentation
                result = hit.position;
                return true;
            }
            result = Vector3.zero;
            return false;
        }

        while (searchPoints > 0)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                Vector3 point;
                if (GetRandomPoint(20f, out point)) //Потом сделать проверку на расстояние path и если точка за стеной и очень долго идти или наоборот очень близко, то перегенерировать
                {
                    agent.SetDestination(point);
                }
                while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
                {
                    if (targetPlayer)
                    {
                        StartCoroutine(Pursuit());
                        yield break;
                    }
                    yield return null;
                }
                yield return new WaitForSeconds(waitTime);
            }
            searchPoints--;
        }
        StartCoroutine(Patrol());
        yield break;
    }

    IEnumerator Kill()
    {
        if (targetPlayer.GetComponent<NetworkObject>().IsOwner) GameManager.Instance.MakePlayerSpectator();
        if (NetworkManager.Singleton.IsHost) targetPlayer.GetComponent<NetworkObject>().Despawn();

        targetDetection = null;
        targetPlayer = null;

        yield return new WaitForSeconds(0.5f);
        Debug.Log("AfterKill");
        StartCoroutine(Patrol());
        yield break;
    }
    #endregion
}