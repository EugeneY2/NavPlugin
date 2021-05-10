using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using System.Reflection;

public class NavController : MonoBehaviour
{
    public NavGraphData navGraphData;
    public NpcData npcData;
    public GameObject npcPrefab;
    NavGraphPoint startPoint, targetPoint;
    List<NavGraphPoint> targetPath;
    Dictionary<int, NavGraphPoint> navData;
    bool isMoving = false, isSceneChange = false, npcMovedToFromScene = false, spawningNpc = false;
    public bool chooseNextPoint { get; set; }
    Npc activeNpc;
    GameObject newNpc;
    GameObject[] npcs;
    NavGraphPoint nextPoint, prevPoint;
    public NpcData editableNpcData { get; set; }
    float passedDistance = 0, allDistance = 0;

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("NavController");
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        editableNpcData = Instantiate(npcData);
    }

    void Start()
    {
        navData = new Dictionary<int, NavGraphPoint>();
        foreach (var item in navGraphData.GetData())
        {
            navData.Add(item.id, item);
        }

        npcs = GameObject.FindGameObjectsWithTag("NPC");
        if (npcs.Length > 0)
        {
            activeNpc = npcs[0].GetComponent<Npc>();
        }
        chooseNextPoint = true;
        StartCoroutine(SceneChange());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isMoving)
            {
                navData.TryGetValue(editableNpcData.currentPointId, out startPoint);
                ChooseRandomTargetPoint();
                BuildPathToTargetPoint();
            }
        }
        if (isSceneChange)
        {
            npcs = GameObject.FindGameObjectsWithTag("NPC");
            if (npcs.Length > 0)
            {
                if (SceneManager.GetActiveScene().buildIndex != editableNpcData.currentSceneIndex)
                {
                    Destroy(npcs[0]);
                }
                else
                {
                    activeNpc = npcs[0].GetComponent<Npc>();
                }
            }
            isSceneChange = false;
        }
        if (npcMovedToFromScene)
        {
            if (spawningNpc)
            {
                if (SceneManager.GetActiveScene().buildIndex == editableNpcData.currentSceneIndex)
                {
                    SpawnNpc();
                    npcs = GameObject.FindGameObjectsWithTag("NPC");
                    spawningNpc = false;
                }
            }
        }
    }

    void ChooseRandomTargetPoint()
    {
        int index = Random.Range(0, navGraphData.GetData().Count);
        targetPoint = navGraphData.GetData()[index];
        if (targetPoint.id == editableNpcData.currentPointId)
        {
            ChooseRandomTargetPoint();
            return;
        }
        Debug.Log("Target point: " + targetPoint.id);
    }

    bool pathIsBuild()
    {
        bool result = false;
        if (targetPath.Count != 0)
        {
            result = true;
        }
        return result;
    }

    void BuildPathToTargetPoint()
    {
        targetPath = FindPath.FindPathBFS(startPoint.id, targetPoint.id, navData);
        if (!pathIsBuild())
        {
            Debug.Log("Can not find path to target point!");
            return;
        }
        else
        {
            string resultStr = "";
            foreach (var item in targetPath)
            {
                resultStr += " -> ";
                resultStr += item.id.ToString();
            }
            Debug.Log(resultStr);

            isMoving = true;
            StartCoroutine(MoveToTargetPoint());
        }
    }

    IEnumerator SceneChange()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (SceneManager.GetActiveScene().buildIndex == 0)
                {
                    yield return null;
                }
                else
                {
                    if (0 != editableNpcData.currentSceneIndex)
                    {
                        editableNpcData.currentPosition = activeNpc.GetPosition();
                    }
                    if (npcMovedToFromScene)
                    {
                        spawningNpc = true;  
                    }
                    yield return new WaitForSeconds(0.5f);
                    SceneManager.LoadScene(0);
                    isSceneChange = true;
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (SceneManager.GetActiveScene().buildIndex == 1)
                {
                    yield return null;
                }
                else
                {
                    if (1 != editableNpcData.currentSceneIndex && npcMovedToFromScene)
                    {
                        editableNpcData.currentPosition = activeNpc.GetPosition();
                    }
                    if (npcMovedToFromScene)
                    {
                        spawningNpc = true;
                    }
                    yield return new WaitForSeconds(0.5f);
                    SceneManager.LoadScene(1);
                    isSceneChange = true;
                }
            }
            yield return null;
        }
    }

    IEnumerator MoveToTargetPoint()
    {
        while (true)
        {
            if (!spawningNpc)
            {
                if (chooseNextPoint)
                {
                    if (targetPath.Count > 0)
                    {
                        nextPoint = targetPath[0];
                        targetPath.RemoveAt(0);
                        chooseNextPoint = false;
                        navData.TryGetValue(editableNpcData.currentPointId, out prevPoint);
                        allDistance = Vector3.Distance(editableNpcData.currentPosition, (nextPoint.position + new Vector3(0, editableNpcData.height / 2, 0)));
                    }
                    else
                    {
                        isMoving = false;
                        yield break;
                    }
                }
                if (nextPoint.sceneIndex != SceneManager.GetActiveScene().buildIndex && nextPoint.sceneIndex != editableNpcData.currentSceneIndex)
                {
                    print("Переход на др сцену");
                    editableNpcData.currentPointId = nextPoint.id;
                    editableNpcData.currentPosition = nextPoint.position + new Vector3(0, editableNpcData.height / 2, 0);
                    editableNpcData.currentSceneIndex = nextPoint.sceneIndex;
                    Destroy(npcs[0]);
                    npcMovedToFromScene = true;
                    yield return new WaitForSeconds(0.5f);
                    chooseNextPoint = true;
                }
                if (nextPoint.sceneIndex != SceneManager.GetActiveScene().buildIndex && nextPoint.sceneIndex == editableNpcData.currentSceneIndex)
                {
                    print("Скрытое перемещение");
                    yield return StartCoroutine(HiddenMovement(nextPoint));
                }
                if (nextPoint.sceneIndex == SceneManager.GetActiveScene().buildIndex && nextPoint.sceneIndex != editableNpcData.currentSceneIndex)
                {
                    print("Переход на активную сцену");
                    editableNpcData.currentPointId = nextPoint.id;
                    editableNpcData.currentPosition = nextPoint.position + new Vector3(0, editableNpcData.height / 2, 0);
                    editableNpcData.currentSceneIndex = nextPoint.sceneIndex;
                    npcMovedToFromScene = true;
                    spawningNpc = true;
                    yield return new WaitForSeconds(0.5f);
                    chooseNextPoint = true;
                }
                if (nextPoint.sceneIndex == SceneManager.GetActiveScene().buildIndex && nextPoint.sceneIndex == editableNpcData.currentSceneIndex)
                {
                    if (!activeNpc.moving)
                    {
                        print("Детальное движение");
                        activeNpc.MoveTo(nextPoint);
                    }
                }
            }
            yield return null;
        }
    }

    IEnumerator HiddenMovement(NavGraphPoint point)
    {
        while (true)
        {
            if (point.sceneIndex != SceneManager.GetActiveScene().buildIndex && point.sceneIndex == editableNpcData.currentSceneIndex)
            {
                passedDistance = Vector3.Distance(editableNpcData.currentPosition, prevPoint.position);
                Vector3 dir = ((point.position + new Vector3(0, editableNpcData.height / 2, 0)) - editableNpcData.currentPosition).normalized * editableNpcData.speed * (Mathf.Abs(allDistance - passedDistance) / allDistance);
                if (Vector3.Distance(editableNpcData.currentPosition, (point.position + new Vector3(0, editableNpcData.height / 2, 0))) > 0.5f)
                {
                    editableNpcData.currentPosition += dir;
                }
                else
                {
                    editableNpcData.currentPointId = point.id;
                    editableNpcData.currentPosition = point.position;
                    chooseNextPoint = true;
                    passedDistance = 0;
                    print("Прибыл в точку: " + point.id);
                    yield break;
                }
            }
            else
            {
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    void SpawnNpc()
    {
        newNpc = Instantiate(npcPrefab);
        //COPY NavMeshAgent FIELDS
        newNpc.GetComponent<NavMeshAgent>().baseOffset = editableNpcData.baseOffset;
        newNpc.GetComponent<NavMeshAgent>().speed = editableNpcData.speed;
        newNpc.GetComponent<NavMeshAgent>().angularSpeed = editableNpcData.angularSpeed;
        newNpc.GetComponent<NavMeshAgent>().acceleration = editableNpcData.acceleration;
        newNpc.GetComponent<NavMeshAgent>().stoppingDistance = editableNpcData.stoppingDistance;
        newNpc.GetComponent<NavMeshAgent>().radius = editableNpcData.radius;
        newNpc.GetComponent<NavMeshAgent>().height = editableNpcData.height;
        activeNpc = newNpc.GetComponent<Npc>();
    }
}
