using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Npc : MonoBehaviour
{
    public GameObject startWayPoint;
    public NpcData npcDataStorage;
    NavMeshAgent agent;
    NavController controller;
    public bool moving { get; set; }

    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("NavController").GetComponent<NavController>();
        agent = GetComponent<NavMeshAgent>();
        if (controller.editableNpcData.currentSceneIndex == SceneManager.GetActiveScene().buildIndex)
        {
            transform.position = controller.editableNpcData.currentPosition;
        }
        moving = false;
    }

    public void AddToNpcData()
    {
        agent = GetComponent<NavMeshAgent>();
        npcDataStorage.currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        npcDataStorage.currentPointId = startWayPoint.GetComponent<WayPoint>().id;
        npcDataStorage.currentPosition = startWayPoint.GetComponent<WayPoint>().transform.position + new Vector3(0, agent.height / 2, 0);
        npcDataStorage.baseOffset = agent.baseOffset;
        npcDataStorage.speed = agent.speed;
        npcDataStorage.angularSpeed = agent.angularSpeed;
        npcDataStorage.acceleration = agent.acceleration;
        npcDataStorage.stoppingDistance = agent.stoppingDistance;
        npcDataStorage.radius = agent.radius;
        npcDataStorage.height = agent.height;
    }

    public void MoveTo(NavGraphPoint point)
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(point.position + new Vector3(0, controller.editableNpcData.height / 2, 0));
        moving = true;
        StartCoroutine(CheckDistance(point.position + new Vector3(0, controller.editableNpcData.height / 2, 0), point.id));
    }

    IEnumerator CheckDistance(Vector3 pos, int id)
    {
        while (true)
        {
            if (Vector3.Distance(transform.position, pos) < 0.1f)
            {
                controller.editableNpcData.currentPointId = id;
                controller.editableNpcData.currentPosition = transform.position;
                moving = false;
                controller.chooseNextPoint = true;
                yield break;
            }
            yield return null;
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
