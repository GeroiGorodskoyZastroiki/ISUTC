using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class FlashlightCast : MonoBehaviour
{
    [SerializeField] private sbyte _iterations;
    [SerializeField] private float _colliderRadius = 0.5f;
    [SerializeField] private float _angleAmplification = 1f;
    [SerializeField] private float _maxDistance = 100f;
    [SerializeField] private LayerMask _layerMask;

    private List<Vector2> _points;
    private List<GameObject> _colliders;

    private void Start()
    {
        GetCoordinates();
        MakeSphereColliders(_points.Count);
    }

    private void FixedUpdate()
    {
        ProjectColliders(GetRaycastHitPoints(GetRays(_points)));
    }

    void MakeSphereColliders(int count)
    {
        _colliders = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            GameObject colliderGO = new GameObject("Collider");
            colliderGO.layer = 9;
            colliderGO.tag = "FlashlightCast";
            colliderGO.transform.SetPositionAndRotation(transform.position, transform.rotation);
            colliderGO.transform.parent = transform;
            SphereCollider sphereCollider = colliderGO.AddComponent<SphereCollider>();
            sphereCollider.radius = _colliderRadius;
            sphereCollider.isTrigger = true;
            _colliders.Add(colliderGO);
        }
    }

    void GetCoordinates()
    {
        _points = new List<Vector2>() { new Vector2(0, 0) };
        for (int i = 4; i < _iterations * 4; i += 2)
            _points.AddRange(GetCircleCoordinates(i, (i / 2) - 1));
    }

    List<Vector2> GetCircleCoordinates(int numberOfPoints, int radius)
    {
        List<Vector2> points = new List<Vector2>();
        float angleIncrement = 360f / numberOfPoints; // Угол между точками

        for (int i = 0; i < numberOfPoints; i++)
        {
            float angle = Mathf.Deg2Rad * i * angleIncrement; // Угол в радианах
            float x = Mathf.Cos(angle) * radius; // Вычисление координаты x
            float y = Mathf.Sin(angle) * radius; // Вычисление координаты y
            points.Add(new Vector2(x, y));
        }
        return points;
    }

    List<Ray> GetRays(List<Vector2> points)
    {
        List<Ray> rays = new List<Ray>();
        foreach (Vector2 point in points)
            rays.Add(new Ray(transform.position, Quaternion.Euler(point.x * _angleAmplification, point.y * _angleAmplification, point.x * _angleAmplification) * transform.forward)); //Quaternion.Euler(0f, point.x * _angleAmplification, point.y * _angleAmplification
        return rays;

    }

    List<Vector3> GetRaycastHitPoints(List<Ray> rays)
    {
        List<Vector3> hitPoints = new List<Vector3>();
        foreach (Ray ray in rays)
        {
            Physics.Raycast(ray, out RaycastHit hit, _maxDistance, _layerMask);
            if (hit.transform != null) hitPoints.Add(hit.point);
        }
        return hitPoints;
    }

    void ProjectColliders(List<Vector3> points)
    {
        Debug.DrawRay(transform.position, transform.forward, Color.yellow);
        for (int i = 0; i < points.Count; i++)
            _colliders.Take(points.Count).ToList()[i].transform.position = points[i];
        if (_colliders.Count > points.Count)
            foreach (GameObject collider in _colliders.Skip(points.Count))
                collider.transform.position = transform.position;
    }
}