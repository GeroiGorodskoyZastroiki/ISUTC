using UnityEngine;

public class GizmoCube : GizmoDrawer
{
    [Header("Size")]
    public Vector3 Size = new(1f, 1f, 1f);

    private void OnDrawGizmos()
    {
        if (AlwaysVisible)
        {
            Gizmos.color = Color;
            Gizmos.DrawCube(transform.position + Offset, new Vector3(Size.x, Size.y, Size.z));
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!AlwaysVisible)
        {
            Gizmos.color = Color;
            Gizmos.DrawCube(transform.position + Offset, new Vector3(Size.x, Size.y, Size.z));
        }
    }
}
