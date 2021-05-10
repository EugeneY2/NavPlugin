using UnityEditor;

[CustomEditor(typeof(NpcData))]
public class NpcDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NpcData npcData = (NpcData)target;
        EditorUtility.SetDirty(npcData);
        DrawDefaultInspector();
    }
}
