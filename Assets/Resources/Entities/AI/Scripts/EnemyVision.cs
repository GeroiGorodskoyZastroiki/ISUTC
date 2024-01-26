using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField] private float _losRange;

    private Enemy _enemy;
    private System.Type _type;

    private void Start()
    {
        _enemy = GetComponentInParent<Enemy>();
        _type = GetType();
    }

    private void OnTriggerStay(Collider collider)
    {
        var player = collider.GetComponent<PlayerTag>();
        var flash = collider.GetComponent<FlashlightCast>();

        if (player)
        {
            Debug.DrawLine(transform.position, player.transform.position + new Vector3(0f, 0.9f, 0f), Color.red);
            Physics.SphereCast(transform.position + new Vector3(0f, 1.15f, 0f), 0.2f, (player.transform.position + new Vector3(0f, 0.9f, 0f)) - transform.position, out var hit, _losRange);
            if (hit.transform)
            {
                if (hit.transform.gameObject == player.gameObject)
                {
                    _enemy.TargetFound(_type, player.gameObject);
                    return;
                }
                else _enemy.TargetLost(_type);
            }
        }
        if (flash)
        {
            if (flash.gameObject.transform.parent.GetComponentInChildren<Light>().enabled)
            {
                player = flash.GetComponentInParent<PlayerTag>();
                _enemy.TargetFound(_type, player.gameObject);
            }
            else _enemy.TargetLost(_type);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        var player = collider.GetComponent<PlayerTag>();
        if (player) _enemy.TargetLost(_type);
        var flash = collider.GetComponent<FlashlightCast>();
        if (flash) _enemy.TargetLost(_type);
    }
}
