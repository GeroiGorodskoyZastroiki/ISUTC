using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class AINavigation : MonoBehaviour
{
    #region Data
    [ReadOnly][ShowInInspector] public SearchMode SearchMode;
    [FoldoutGroup("SearchModes")][SerializeField] public SearchMode PursuitSearchMode;
    [FoldoutGroup("SearchModes")][SerializeField] public SearchMode CheckSearchMode;
    [FoldoutGroup("SearchModes")][SerializeField] public SearchMode PatrolSearchMode;
    [FoldoutGroup("SearchModes")][SerializeField] public SearchMode KillSearchMode;
    [SerializeField] float _maxAdditionPathDistance = 5f;
    #endregion

    #region References
    [HideInInspector] public AI AI;
    #endregion

    private void Start()
    {
        StartCoroutine(Patrol());
    }

    public IEnumerator Patrol()
    {
        Debug.Log("Patrol");
        AI.State = AIStates.Patrol;
        AI.Movement.Sprint = false;
        SearchMode = PatrolSearchMode;

        var navMeshWaypoints = GameObject.FindGameObjectsWithTag("NavMeshWaypoint");
        AI.Agent.SetDestination(navMeshWaypoints[Random.Range(0, navMeshWaypoints.Length)].transform.position);

        while (AI.Agent.pathPending || AI.Agent.remainingDistance > AI.Agent.stoppingDistance)
            yield return null;

        yield return new WaitForSeconds(2);
        StartCoroutine(Search());
    }

    public IEnumerator Pursuit()
    {
        Debug.Log("Pursuit");
        AI.State = AIStates.Pursuit;
        AI.Movement.Sprint = true;
        SearchMode = PursuitSearchMode;

        AI.Agent.SetDestination(AI.Detection.TargetGameObject.transform.position);
        while (AI.Agent.pathPending || AI.Agent.remainingDistance > AI.Melee.AttackDistance)
        {
            AI.Agent.SetDestination(AI.Detection.TargetGameObject.transform.position);
            yield return null;
        }
    }

    public IEnumerator Check()
    {
        Debug.Log("Check");
        AI.State = AIStates.Check;
        AI.Movement.Sprint = true;
        SearchMode = CheckSearchMode;

        AI.Agent.SetDestination(AI.Detection.TargetPosition);
        while (AI.Agent.pathPending || AI.Agent.remainingDistance > AI.Agent.stoppingDistance)
            yield return null;

        yield return new WaitForSeconds(2);
        StartCoroutine(Search());
    }

    public IEnumerator Search()
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
        
        Debug.Log("Search");
        AI.State = AIStates.Search;

        for (int i = 0; i < SearchMode.Points; i++)
        {
            if (AI.Agent.remainingDistance <= AI.Agent.stoppingDistance)
                yield return null;

            while (true)
            {
                if (GetRandomPoint(SearchMode.MaxRange, out Vector3 point))
                {
                    NavMeshPath path = new NavMeshPath();
                    AI.Agent.CalculatePath(point, path);
                    if (path.CalculateDistance() > SearchMode.MaxRange + _maxAdditionPathDistance) continue; 
                }
                AI.Agent.SetDestination(point);
                break;
            }

            while (AI.Agent.pathPending || AI.Agent.remainingDistance > AI.Agent.stoppingDistance)
                yield return null;

            yield return new WaitForSeconds(SearchMode.WaitTime);
        }
        StartCoroutine(Patrol());
    }
}
