using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Npc))]
public class NpcEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Npc npc = (Npc)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Add To Npc Data"))
        {
            npc.AddToNpcData();
        }
    }
}