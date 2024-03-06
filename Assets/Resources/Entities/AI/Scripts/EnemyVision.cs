using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField] private float _losRange;

    private Enemy _enemy;
    private System.Type _type;
    LayerMask _layerMask;

    private void Start()
    {
        _enemy = GetComponentInParent<Enemy>();
        _type = GetType();
        _layerMask = LayerMask.GetMask(new string[] { "StaticGeometry", "Player" });
    }

    private void OnTriggerStay(Collider collider)
    {
        var player = collider.GetComponent<Player>();
        var flash = collider.GetComponent<FlashlightCast>();

        if (player)
        {
            //Debug.Log("player");
            Debug.DrawRay(transform.root.position + new Vector3(0f, 1.15f, 0f), ((player.gameObject.transform.position) - transform.root.position).normalized, Color.red);
            Physics.SphereCast(transform.root.position + new Vector3(0f, 1.15f, 0f), 0.2f, ((player.gameObject.transform.position) - transform.root.position).normalized, out var hit, _losRange, _layerMask);
            if (hit.transform)
            {
                //Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.gameObject == player.gameObject)
                {
                    //Debug.Log("targetfound");
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
                player = flash.GetComponentInParent<Player>();
                _enemy.TargetFound(_type, player.gameObject);
            }
            else _enemy.TargetLost(_type);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        var player = collider.GetComponent<Player>();
        if (player) _enemy.TargetLost(_type);
        var flash = collider.GetComponent<FlashlightCast>();
        if (flash) _enemy.TargetLost(_type);
    }
}
