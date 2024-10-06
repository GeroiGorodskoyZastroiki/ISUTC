using UnityEngine;
using UnityEngine.Events;

public class AIVision : MonoBehaviour
{
    #region Data
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _losRange;
    [HideInInspector] public UnityEvent<GameObject> OnVisualDetected;
    [HideInInspector] public UnityEvent<Vector3> OnVisualIndirectDetected;
    [HideInInspector] public UnityEvent<GameObject> OnVisualLost;
    #endregion

    #region References
    private AI AI;
    #endregion

    private void Start()
    {
        AI = GetComponentInParent<AI>();
    }

    private void Update() 
    {
        if (AI.Detection.TargetGameObject != null)
        {
            Ray ray = new Ray(transform.root.position + new Vector3(0f, 1.15f, 0f), (AI.Detection.TargetGameObject.transform.position - transform.root.position).normalized);
            Physics.Raycast(ray, out var hit, _losRange, _layerMask);
            if (hit.transform)
                if (hit.transform.root.gameObject != AI.Detection.TargetGameObject)
                {
                    Debug.Log("Lost " + AI.Detection.TargetGameObject);
                    OnVisualLost.Invoke(AI.Detection.TargetGameObject);
                }
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        var player = collider.GetComponent<Player>();
        if (player)
        {
            //Debug.DrawRay(transform.root.position + new Vector3(0f, 1.15f, 0f), ((player.gameObject.transform.position) - transform.root.position).normalized, Color.red);
            Ray ray = new Ray(transform.root.position + new Vector3(0f, 1.15f, 0f), (player.transform.position - transform.root.position).normalized);
            Physics.SphereCast(ray, 0.2f, out var hit, _losRange, _layerMask);
            if (hit.transform)
                if (hit.transform.root.gameObject == player.gameObject)
                {
                    Debug.Log("Detected " + player);
                    OnVisualDetected.Invoke(player.gameObject);
                    return;
                }                  
        }

        if (collider.tag != "FlashlightCast") return;
        var flash = collider;
        if (flash)
        {
            Ray ray = new Ray(transform.root.position + new Vector3(0f, 1.15f, 0f), (flash.transform.position - transform.root.position).normalized);
            Debug.DrawRay(transform.root.position + new Vector3(0f, 1.15f, 0f), (flash.transform.position - transform.root.position).normalized * 10f, Color.red);
            Physics.Raycast(ray, out var hit, _losRange, _layerMask);  
            if (hit.transform)
            {
                if (hit.collider.gameObject.tag != "FlashlightCast") return;
                    if (flash.gameObject.transform.root.GetComponentInChildren<Light>().enabled)
                    {
                        float distanceOffset = 0.5f * hit.distance;
                        var positionOffset = new Vector3(Random.Range(-distanceOffset, distanceOffset), 0, Random.Range(-distanceOffset, distanceOffset));
                        OnVisualIndirectDetected.Invoke(flash.transform.root.position + positionOffset);
                    }
            }
        }
    }

    // private void OnTriggerExit(Collider collider)
    // {
    //     if (collider.GetComponent<Player>())
    //         OnVisualLost.Invoke(collider.gameObject);
    // }
}
