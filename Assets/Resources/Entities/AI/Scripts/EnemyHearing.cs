using UnityEngine;
using UnityEngine.AI;

public class EnemyHearing : MonoBehaviour
{
    private Enemy _enemy;
    private NavMeshAgent _agent;

    public float HearingDistanceIndirect;

    private void Start()
    {
        _enemy = GetComponentInParent<Enemy>();
        _agent = _enemy.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        float nearestDistance = Mathf.Infinity;
        GameObject targetPlayer = null;
        foreach (GameObject soundEmitter in GameObject.FindGameObjectsWithTag("SoundEmitter"))
        {
            if (!soundEmitter.GetComponent<AudioSource>().isPlaying)
            {
                //if (enemy.targetPlayer == soundEmitter.GetComponentInParent<Player>().gameObject)
                if (_enemy.TargetPlayer != null) _enemy.TargetLost(GetType());
                return;
            }
            NavMeshPath path = new();
            bool pathCalculated = _agent.CalculatePath(soundEmitter.transform.position, path);
            if (pathCalculated && path.CalculateDistance() < nearestDistance)
            {
                nearestDistance = path.CalculateDistance();
                targetPlayer = soundEmitter;
            }
        }
        Debug.Log(nearestDistance);
        if (nearestDistance < HearingDistanceIndirect) _enemy.TargetFound(GetType(), targetPlayer);
        else _enemy.TargetLost(GetType());
    }
}
