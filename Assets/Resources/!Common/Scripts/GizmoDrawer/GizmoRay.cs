using UnityEngine;

public class GizmoRay : GizmoDrawer
{
    [Header("Size")]
    public float Lenght = 1f;

    private void OnDrawGizmos()
    {
        if (AlwaysVisible) Draw();
    }

    private void OnDrawGizmosSelected()
    {
        if (!AlwaysVisible) Draw();
    }

    public override void Draw()
    {
        base.Draw();
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
