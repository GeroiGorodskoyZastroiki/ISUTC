using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField] float LOSRange; 

    Enemy enemy;
    System.Type type;

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        type = this.GetType();
    }

    void OnTriggerStay(Collider collider)
    {
        var player = collider.GetComponent<PlayerTag>();
        var flash = collider.GetComponent<FlashlightCast>();

        if (player)
        {
            Debug.DrawLine(transform.position, player.transform.position + new Vector3(0f, 0.9f, 0f), Color.red);
            Physics.SphereCast(transform.position + new Vector3(0f, 1.15f, 0f), 0.2f, (player.transform.position + new Vector3(0f, 0.9f, 0f)) - transform.position, out var hit, LOSRange);
            if (hit.transform)
            {
                if (hit.transform.gameObject == player.gameObject)
                {
                    enemy.TargetFound(type, player.gameObject);
                    return;
                }
                else enemy.TargetLost(type);
            }
        }
        if (flash)
        {
            if (flash.gameObject.transform.parent.GetComponentInChildren<Light>().enabled)
            {
                player = flash.GetComponentInParent<PlayerTag>();
                enemy.TargetFound(type, player.gameObject);
                return;
            }
            else enemy.TargetLost(type);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        var player = collider.GetComponent<PlayerTag>();
        if (player) enemy.TargetLost(type);
        var flash = collider.GetComponent<FlashlightCast>();
        if (flash) enemy.TargetLost(type);
    }
}
