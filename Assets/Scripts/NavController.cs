using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class NavController : MonoBehaviour
{
    public NavGraphData navGraphData;
    public NpcData npcData;
    public GameObject npcPrefab;
    NavGraphPoint startPoint, targetPoint;
    List<NavGraphPoint> targetPath;
    Dictionary<int, NavGraphPoint> navData;
    bool isMoving = false, isSceneChange = false, npcMovedToFromScene = false, spawningNpc = false, copyNpsData = true;
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
        if (copyNpsData)
        {
            editableNpcData = Instantiate(npcData);
            copyNpsData = false;
        }
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
                    foreach (var item in npcs)
                    {
                        Destroy(item);
                    }
                }
                else
                {
                    activeNpc = npcs[0].GetComponent<Npc>();
                    spawningNpc = false;
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
                    activeNpc = npcs[0].GetComponent<Npc>();
                    spawningNpc = false;
                }
                else
                {
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

    bool PathIsBuild()
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
        if (!PathIsBuild())
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
                if (SceneManager.GetActiveScene().buildIndex != 0)
                {
                    yield return new WaitForSeconds(1);
                    SceneChangeCheck(0);     
                    SceneManager.LoadScene(0);
                    isSceneChange = true;
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (SceneManager.GetActiveScene().buildIndex != 1)
                {
                    yield return new WaitForSeconds(1);
                    SceneChangeCheck(1);
                    SceneManager.LoadScene(1);
                    isSceneChange = true;
                }
            }
            yield return null;
        }
    }

    void SceneChangeCheck(int sceneIndex)
    {
        if (sceneIndex != editableNpcData.currentSceneIndex)
        {
            editableNpcData.currentPosition = activeNpc.GetPosition();
        }
        if (npcMovedToFromScene)
        {
            spawningNpc = true;
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
                    print("?????????????? ???? ???? ??????????");
                    editableNpcData.currentPointId = nextPoint.id;
                    editableNpcData.currentPosition = nextPoint.position + new Vector3(0, editableNpcData.height / 2, 0);
                    editableNpcData.currentSceneIndex = nextPoint.sceneIndex;
                    Destroy(npcs[0]);
                    npcMovedToFromScene = true;
                    chooseNextPoint = true;
                }
                if (nextPoint.sceneIndex != SceneManager.GetActiveScene().buildIndex && nextPoint.sceneIndex == editableNpcData.currentSceneIndex)
                {
                    print("?????????????? ??????????????????????");
                    yield return StartCoroutine(HiddenMovement(nextPoint));
                }
                if (nextPoint.sceneIndex == SceneManager.GetActiveScene().buildIndex && nextPoint.sceneIndex != editableNpcData.currentSceneIndex)
                {
                    print("?????????????? ???? ???????????????? ??????????");
                    editableNpcData.currentPointId = nextPoint.id;
                    editableNpcData.currentPosition = nextPoint.position + new Vector3(0, editableNpcData.height / 2, 0);
                    editableNpcData.currentSceneIndex = nextPoint.sceneIndex;
                    npcMovedToFromScene = true;
                    spawningNpc = true;
                    chooseNextPoint = true;
                }
                if (nextPoint.sceneIndex == SceneManager.GetActiveScene().buildIndex && nextPoint.sceneIndex == editableNpcData.currentSceneIndex)
                {
                    if (!spawningNpc)
                    {
                        if (!activeNpc.moving)
                        {
                            print("?????????????????? ????????????????");
                            activeNpc.MoveTo(nextPoint);
                        }
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
                    print("???????????? ?? ??????????: " + point.id);
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
    }
}
