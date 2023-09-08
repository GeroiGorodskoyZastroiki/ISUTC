using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ApplyRootTransform))]
public class ApplyRootTransformEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ApplyRootTransform script = (ApplyRootTransform)target;
        if (GUILayout.Button("Apply Root Transform To Children"))
        {
            script.ApplyWorldTransformToChildren(script.transform);
        }
        if (GUILayout.Button("Apply Root Transform To Children Recursive"))
        {
            script.ApplyWorldTransformToChildrenRecursive();
        }
        EditorGUILayout.HelpBox("Root world position will be zero", MessageType.None);
    }
}
