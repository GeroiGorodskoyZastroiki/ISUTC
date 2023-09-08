using UnityEngine;
using UnityEditor;

public class ApplyRootTransform : MonoBehaviour
{
    public void ApplyWorldTransformToChildren(Transform go)
    {
        if (go.childCount == 0) return;
        foreach (Transform child in go.GetComponentInChildren<Transform>())
        {
            if (child.parent != go) return;
            child.localPosition += go.position; //переписать под сохранение и применение world position
        }
        go.position = Vector3.zero;
        //go.rotation = Quaternion.identity;
    }

    public void ApplyWorldTransformToChildrenRecursive()
    {
        ApplyWorldTransformToChildren(gameObject.transform);
        foreach (Transform child in gameObject.GetComponentInChildren<Transform>())
        {
            ApplyWorldTransformToChildren(child);
        }
    }
}
