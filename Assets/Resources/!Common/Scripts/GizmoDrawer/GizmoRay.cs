using UnityEngine;

public class GizmoRay : GizmoDrawer 
{
	[Header ("Size")]
	public float lenght = 1f;

    void OnDrawGizmos()
    {
        if (alwaysVisible) Draw();
    }

    void OnDrawGizmosSelected()
    {
        if (!alwaysVisible) Draw();
    }

    public override void Draw()
    {
        base.Draw();
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
