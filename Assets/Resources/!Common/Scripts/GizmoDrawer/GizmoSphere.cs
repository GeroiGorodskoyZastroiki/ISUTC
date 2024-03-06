using UnityEngine;

public class GizmoSphere : GizmoDrawer
{
    [Header("Size")]
    public float Radius = 1f;

    private void OnDrawGizmos()
    {
        if (AlwaysVisible)
        {
            Gizmos.color = Color;
            Gizmos.DrawSphere(transform.position + Offset, Radius);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!AlwaysVisible)
        {
            Gizmos.color = Color;
            Gizmos.DrawSphere(transform.position + Offset, Radius);
        }
    }
}
