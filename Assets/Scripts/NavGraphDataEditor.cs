using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NavGraphData))]
public class NavGraphDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NavGraphData ng = (NavGraphData)target;
        EditorUtility.SetDirty(ng);
        DrawDefaultInspector();
    }
}