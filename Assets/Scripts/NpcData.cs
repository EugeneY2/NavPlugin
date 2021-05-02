using UnityEngine;

[CreateAssetMenu(fileName = "NpcData", menuName = "NpcData", order = 1)]
public class NpcData : ScriptableObject
{ 
    [SerializeField]
    public int currentSceneIndex;
    [SerializeField]
    public int currentPointId;
    [SerializeField]
    public Vector3 currentPosition;
    [SerializeField]
    public float speed;
    [SerializeField]
    public float height;
}
