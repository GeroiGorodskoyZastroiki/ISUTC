using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectColliders : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] int radiusPoints;
    [SerializeField] float range;
    public List<List<List<GameObject>>> colliders = new List<List<List<GameObject>>>(50);

    void Start()
    {
        int k = radiusPoints;
        for (int i = 0; i < radiusPoints; i++)
        {
            List<List<GameObject>> secondList = new List<List<GameObject>>(k);
            for (int j = k; j > 0; j--)
            {
                List<GameObject> innerList = new List<GameObject>();
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
            colliders.Add(secondList);
            k--;
        }
    }

    void Update()
    {
        int k = radiusPoints;
        for (int i = 0; i < radiusPoints; i++)
        {
            for (int j = k; j > 0; j--)
            {
                RaycastHit hit;
                Physics.Raycast(new Ray(gameObject.transform.position, Quaternion.AngleAxis(i, gameObject.transform.right) * Quaternion.AngleAxis(j, gameObject.transform.up) * gameObject.transform.forward), out hit, range, 9);
                colliders[i][j-1][0].transform.position = hit.point;
                Physics.Raycast(new Ray(gameObject.transform.position, Quaternion.AngleAxis(i, gameObject.transform.right) * Quaternion.AngleAxis(-j, gameObject.transform.up) * gameObject.transform.forward), out hit, range, 9);
                colliders[i][j-1][1].transform.position = hit.point;
                Physics.Raycast(new Ray(gameObject.transform.position, Quaternion.AngleAxis(-i, gameObject.transform.right) * Quaternion.AngleAxis(j, gameObject.transform.up) * gameObject.transform.forward), out hit, range, 9);
                colliders[i][j-1][2].transform.position = hit.point;
                Physics.Raycast(new Ray(gameObject.transform.position, Quaternion.AngleAxis(-i, gameObject.transform.right) * Quaternion.AngleAxis(-j, gameObject.transform.up) * gameObject.transform.forward), out hit, range, 9);
                colliders[i][j-1][3].transform.position = hit.point;
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
