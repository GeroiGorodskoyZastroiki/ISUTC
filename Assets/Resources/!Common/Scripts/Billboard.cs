using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public static void RotateObjectTowards(Transform go, Transform target)
    {
        go.LookAt(go.position + (target.position - go.position) * -2f);
        go.rotation = Quaternion.Euler(new Vector3(0f, go.rotation.eulerAngles.y, 0f));
    }
}
