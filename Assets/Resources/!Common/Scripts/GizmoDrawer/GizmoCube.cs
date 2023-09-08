using UnityEngine;

public class GizmoCube : GizmoDrawer
{
    [Header("Size")]
    public Vector3 size = new Vector3(1f, 1f, 1f);

    void OnDrawGizmos()
    {
        if (alwaysVisible)
        {
            Gizmos.color = color;
            Gizmos.DrawCube(transform.position + offset, new Vector3(size.x, size.y, size.z));
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!alwaysVisible)
        {
            Gizmos.color = color;
            Gizmos.DrawCube(transform.position + offset, new Vector3(size.x, size.y, size.z));
        }
    }
}
