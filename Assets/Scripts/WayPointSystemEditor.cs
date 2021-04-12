using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WayPointSystem))]
public class WayPointSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WayPointSystem wp = (WayPointSystem)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Add Way Point"))
        {
            wp.AddNewWayPoint();
        }
        if (GUILayout.Button("Clear Way Points"))
        {
            wp.ClearWayPoints();
        }     
        if(GUILayout.Button("Add To NavGraph"))
        {
            wp.AddToNavGraph();
        }
    }
}

public static class WayPointSystemMenu
{
    [MenuItem("GameObject/Add WayPointSystem", false, 0)]

    static void AddPrefab() 
    {
        GameObject newWayPointSystem = new GameObject();
        newWayPointSystem.AddComponent<WayPointSystem>();
        newWayPointSystem.GetComponent<WayPointSystem>().Init();
        newWayPointSystem.name = "WayPointSystem";     
    }
}