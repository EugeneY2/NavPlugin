using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Npc : MonoBehaviour
{
    public float speed;
    public float npcHeight;
    public GameObject startWayPoint;
    public NpcData data;

    NpcData activeData;

    void Start()
    {
        activeData = GameObject.FindGameObjectWithTag("NavController").GetComponent<NavController>().npcData;

        if (activeData.SceneIndex == SceneManager.GetActiveScene().buildIndex)
        {
            transform.position = activeData.Position;
        }

    }

    // FOR TESTING PURPOSES
    void Update()
    {
        transform.position = activeData.Position;
    }

    public void AddToNpcData()
    {
        data.SceneIndex = SceneManager.GetActiveScene().buildIndex;
        data.CurrentPointID = startWayPoint.GetComponent<WayPoint>().id;
        data.Position = startWayPoint.GetComponent<WayPoint>().transform.position + new Vector3(0, npcHeight / 2, 0);
        data.Speed = speed;
        data.Height = npcHeight;
    }

    public void moveTo(NavGraphPoint point)
    {
        StartCoroutine(move(point));
        print("я пошелъ");
    }

    public IEnumerator move(NavGraphPoint point)
    {
        yield return null;
    }
}
