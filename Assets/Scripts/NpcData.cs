using UnityEngine;

[CreateAssetMenu(fileName = "NpcData", menuName = "NpcData", order = 1)]
public class NpcData : ScriptableObject
{ 
    [SerializeField]
    int currentSceneIndex, currentPointId;
    [SerializeField]
    Vector3 currentPosition;
    [SerializeField]
    float speed, height;

    public int SceneIndex
    {
        get{return currentSceneIndex;}
        set{currentSceneIndex = value;}
    }
    public int CurrentPointID
    {
        get{return currentPointId;}
        set{currentPointId = value;}
    }
    public Vector3 Position
    {
        get{return currentPosition;}
        set{currentPosition = value;}
    }
    public float Speed
    {
        get{return speed;}
        set{speed = value;}
    } 
    public float Height
    {
        get{return height;}
        set{height = value;}
    } 
}
