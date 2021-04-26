using UnityEngine;
using UnityEngine.SceneManagement;

public class Npc : MonoBehaviour
{
    public float speed;
    public float npcHeight;
    public GameObject startWayPoint;
    public NpcData data;

    void Start()
    {
        transform.position = data.Position;
    }

    public void AddToNpcData()
    {
        data.SceneIndex = SceneManager.GetActiveScene().buildIndex;
        data.CurrentPointID = startWayPoint.GetComponent<WayPoint>().id;
        data.Position = startWayPoint.GetComponent<WayPoint>().transform.position + new Vector3(0, npcHeight / 2, 0);
        data.Speed = speed;
        data.Height = npcHeight;
    }
}
