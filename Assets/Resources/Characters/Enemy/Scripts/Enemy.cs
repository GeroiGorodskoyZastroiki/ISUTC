using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public enum AIStates
{
    Patrol,
    Pursuit
}

public class Enemy : MonoBehaviour
{
    //public SteamId steamId = 0;

    [Header("AI")]
    public GameObject TargetPlayer;
    public System.Type TargetDetection;
    public AIStates State = AIStates.Patrol;
    [SerializeField] private float _pursuitAftertime;
    [SerializeField] private int _patrolSearchPoints;
    [SerializeField] private float _patrolWaitTime;
    [SerializeField] private int _pursuitSearchPoints;
    [SerializeField] private float _pursuitWaitTime;
    [SerializeField] private float _attackDistance = 1f;

    [Header("Movement Constants")]
    [SerializeField] private float _moveSpeed = 4f;
    [SerializeField] private float _sprintSpeed = 8f;

    [Header("Movement")]
    [SerializeField] private float _targetSpeed;
    [SerializeField] private Vector2 _moveDirection;
    [SerializeField] private Vector3 _moveVelocity;
    [SerializeField] private Vector2 _animVelocity;
    [SerializeField] private bool _sprint;

    [Header("Debug")]
    [SerializeField] private bool _standStill;
    [SerializeField] private bool _pursuitTransform;
    [SerializeField] private Transform _destination;

    private NavMeshAgent _agent;
    private Animator _animator;

    private void Awake()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            Destroy(GetComponentInChildren<EnemyVision>());
            Destroy(GetComponentInChildren<EnemyHearing>());
            Destroy(this);
        }
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(Patrol());
    }

    private void Update()
    {
        if (!_standStill)
        {
            if (_pursuitTransform) _agent.SetDestination(_destination.position);

            Move();
            Animate();
        }
    }

    public void Animate()
    {
        _animator.SetFloat("Velocity", _agent.velocity.magnitude);
    }

    private void Move()
    {
        _targetSpeed = _sprint ? _sprintSpeed : _moveSpeed;
        _agent.speed = _targetSpeed;
    }

    #region States
    public void TargetFound(System.Type type, GameObject player)
    {
        if (TargetPlayer == player) return;
        if (TargetDetection != null)
            if (TargetDetection == typeof(EnemyVision) && type == typeof(EnemyHearing)) return;
        TargetDetection = type;
        TargetPlayer = player;
        Debug.Log(player.GetComponent<PlayerNetwork>().SteamId.Value);
        Debug.Log(TargetDetection.ToString());
        StopAllCoroutines();
        StartCoroutine(Pursuit());
    }

    public void TargetLost(System.Type type)
    {
        if (TargetDetection != type) return;
        StartCoroutine(PursuitAfterburner());
    }

    private IEnumerator Patrol()
    {
        State = AIStates.Patrol;
        _sprint = false;

        var navMeshWaypoints = GameObject.FindGameObjectsWithTag("NavMeshWaypoint");
        _agent.SetDestination(navMeshWaypoints[Random.Range(0, navMeshWaypoints.Length)].transform.position);

        while (_agent.pathPending || _agent.remainingDistance > _agent.stoppingDistance)
            yield return null;

        yield return new WaitForSeconds(2);
        StartCoroutine(Search(_patrolSearchPoints, _patrolWaitTime));
    }

    private IEnumerator Pursuit()
    {
        State = AIStates.Pursuit;
        _sprint = true;

        _agent.SetDestination(TargetPlayer.transform.position);
        while (_agent.pathPending || _agent.remainingDistance > _attackDistance) //_agent.stoppingDistance
        {
            if (TargetPlayer) _agent.SetDestination(TargetPlayer.transform.position);
            yield return null;
        }

        if (TargetPlayer)
        {
            if (!TargetPlayer.GetComponent<NetworkObject>().IsOwnedByServer)
                yield return new WaitForSeconds(0.1f);
            StartCoroutine(Kill());
        }
        else
        {
            yield return new WaitForSeconds(2);
            StartCoroutine(Search(_pursuitSearchPoints, _pursuitWaitTime));
        }
    }

    private IEnumerator PursuitAfterburner()
    {
        yield return new WaitForSeconds(_pursuitAftertime);
        TargetDetection = null;
        TargetPlayer = null;
    }

    private IEnumerator Search(int searchPoints, float waitTime)
    {
        bool GetRandomPoint(float maxRange, out Vector3 result)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * maxRange;
            randomPoint.Set(randomPoint.x, 0.5f, randomPoint.z);
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 100.0f, NavMesh.AllAreas))
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
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (GetRandomPoint(20f, out Vector3 point)) //Потом сделать проверку на расстояние path и если точка за стеной и очень долго идти или наоборот очень близко, то перегенерировать
                {
                    _agent.SetDestination(point);
                }
                while (_agent.pathPending || _agent.remainingDistance > _agent.stoppingDistance)
                {
                    if (TargetPlayer)
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
    }

    private IEnumerator Kill()
    {
        TargetPlayer.GetComponent<NetworkObject>().Despawn();

        TargetDetection = null;
        TargetPlayer = null;

        yield return new WaitForSeconds(0.5f);
        Debug.Log("AfterKill");
        StartCoroutine(Patrol());
    }
    #endregion
}