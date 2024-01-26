using System.Collections.Generic;
using UnityEngine;

public class ProjectColliders : MonoBehaviour
{
    [SerializeField] private float _radius;
    [SerializeField] private int _radiusPoints;
    [SerializeField] private float _range;
    public List<List<List<GameObject>>> Colliders = new(50);

    private void Start()
    {
        int k = _radiusPoints;
        for (int i = 0; i < _radiusPoints; i++)
        {
            List<List<GameObject>> secondList = new(k);
            for (int j = k; j > 0; j--)
            {
                List<GameObject> innerList = new();
                for (int z = 0; z < 4; z++)
                {
                    var go = new GameObject();
                    go.transform.parent = gameObject.transform;
                    go.AddComponent(typeof(FlashlightCast));
                    go.AddComponent(typeof(Rigidbody));
                    go.AddComponent(typeof(SphereCollider));
                    go.GetComponent<SphereCollider>().isTrigger = true;
                    go.GetComponent<SphereCollider>().radius = 0.25f;
                    go.layer = 2;
                    go.name = i + " " + j + " " + z;
                    innerList.Add(go);
                }
                secondList.Add(innerList);
            }
            Colliders.Add(secondList);
            k--;
        }
    }

    private void Update()
    {
        int k = _radiusPoints;
        for (int i = 0; i < _radiusPoints; i++)
        {
            for (int j = k; j > 0; j--)
            {
                Physics.Raycast(new Ray(gameObject.transform.position, Quaternion.AngleAxis(i, gameObject.transform.right) * Quaternion.AngleAxis(j, gameObject.transform.up) * gameObject.transform.forward), out RaycastHit hit, _range, 9);
                Colliders[i][j - 1][0].transform.position = hit.point;
                Physics.Raycast(new Ray(gameObject.transform.position, Quaternion.AngleAxis(i, gameObject.transform.right) * Quaternion.AngleAxis(-j, gameObject.transform.up) * gameObject.transform.forward), out hit, _range, 9);
                Colliders[i][j - 1][1].transform.position = hit.point;
                Physics.Raycast(new Ray(gameObject.transform.position, Quaternion.AngleAxis(-i, gameObject.transform.right) * Quaternion.AngleAxis(j, gameObject.transform.up) * gameObject.transform.forward), out hit, _range, 9);
                Colliders[i][j - 1][2].transform.position = hit.point;
                Physics.Raycast(new Ray(gameObject.transform.position, Quaternion.AngleAxis(-i, gameObject.transform.right) * Quaternion.AngleAxis(-j, gameObject.transform.up) * gameObject.transform.forward), out hit, _range, 9);
                Colliders[i][j - 1][3].transform.position = hit.point;
            }
            k--;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    var offset = radius / radiusPoints;
    //    int k = radiusPoints;
    //    for (int i = 0; i < radiusPoints; i++)
    //    {
    //        for (int j = k; j > 0; j--)
    //        {
    //            Gizmos.DrawRay(gameObject.transform.position, Quaternion.AngleAxis(i, gameObject.transform.right) * Quaternion.AngleAxis(j, gameObject.transform.up) * gameObject.transform.forward);
    //            Gizmos.DrawRay(gameObject.transform.position, Quaternion.AngleAxis(i, gameObject.transform.right) * Quaternion.AngleAxis(-j, gameObject.transform.up) * gameObject.transform.forward);
    //            Gizmos.DrawRay(gameObject.transform.position, Quaternion.AngleAxis(-i,gameObject.transform.right ) * Quaternion.AngleAxis(j, gameObject.transform.up) * gameObject.transform.forward);
    //            Gizmos.DrawRay(gameObject.transform.position, Quaternion.AngleAxis(-i,gameObject.transform.right ) * Quaternion.AngleAxis(-j, gameObject.transform.up) * gameObject.transform.forward);
    //        }
    //        k--;
    //    }
    //}
}
