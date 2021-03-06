using UnityEngine;

[CreateAssetMenu(fileName = "NpcData", menuName = "NpcData", order = 1)]
public class NpcData : ScriptableObject
{
    [SerializeField]
    public int currentSceneIndex, currentPointId;
    [SerializeField]
    public Vector3 currentPosition;
    [SerializeField]
    public float baseOffset, speed, angularSpeed, acceleration, stoppingDistance, radius, height;
}
