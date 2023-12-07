using UnityEngine;

public abstract class GizmoDrawer : MonoBehaviour
{
    public Color Color = Color.gray;

    [Header("Offset")]
    public Vector3 Offset;

    [Header("Other")]
    [Tooltip("Whether to always show or not in Scene (Gizmo will always appear if the object is selected)")]
    public bool AlwaysVisible = true;
    // public bool hideThenSelected = false;
    //public bool wireframeMode = false;

    public virtual void Draw()
    {
        Gizmos.color = Color;
    }
}
