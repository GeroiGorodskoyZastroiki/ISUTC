using UnityEngine;

public class GizmoSphere : GizmoDrawer 
{
	[Header ("Size")]
	public float radius = 1f;

    void OnDrawGizmos()
    {
        if (alwaysVisible)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position + offset, radius);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!alwaysVisible)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position + offset, radius);
        }
    }
}
