using UnityEngine;

public class AimPoint : MonoBehaviour
{
    private float _maxDistance = 100f;
    private float _lastHitDistance;

    private void Update() =>
        transform.position = GetRaycastHitPoint();

    private Vector3 GetRaycastHitPoint()
    {
        Ray ray = Camera.main!.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance))
        {
            _lastHitDistance = hit.distance;
            return hit.point;
        }
        else return ray.origin + ray.direction * _lastHitDistance;
    }
}
