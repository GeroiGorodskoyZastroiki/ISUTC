using UnityEngine;
using UnityEngine.AI;

public class EnemyHearing : MonoBehaviour
{
    Enemy enemy;
    NavMeshAgent agent;

    public float hearingDistanceIndirect;
  
    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        agent = enemy.GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        float nearestDistance = Mathf.Infinity;
        GameObject targetPlayer = null;
        foreach (GameObject soundEmitter in GameObject.FindGameObjectsWithTag("SoundEmitter"))
        {
            if (!soundEmitter.GetComponent<AudioSource>().isPlaying)
            {
                //if (enemy.targetPlayer == soundEmitter.GetComponentInParent<Player>().gameObject)
                if (enemy.targetPlayer != null) enemy.TargetLost(this.GetType());
                return;
            }
            NavMeshPath path = new NavMeshPath();
            bool pathCalculated = agent.CalculatePath(soundEmitter.transform.position, path);
            if (pathCalculated && path.CalculateDistance() < nearestDistance)
            {
                nearestDistance = path.CalculateDistance();
                targetPlayer = soundEmitter;
            }
        }
        Debug.Log(nearestDistance);
        if (nearestDistance < hearingDistanceIndirect) enemy.TargetFound(this.GetType(), targetPlayer);
        else enemy.TargetLost(this.GetType());
    }
}
