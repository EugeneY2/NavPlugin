using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavController : MonoBehaviour
{
    public NavGraphData navGraphData;
    public NpcData npcData;
    public GameObject NpcPrefab;
    NavGraphPoint startPoint, targetPoint;
    List<NavGraphPoint> targetPath;
    Dictionary<int, NavGraphPoint> navData;
    bool isMoving = false;

    GameObject activeNpc;
    GameObject[] npcs;

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("NavController");
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        navData = new Dictionary<int, NavGraphPoint>();
        foreach (var item in navGraphData.GetData())
        {
            navData.Add(item.id, item);
        }
        npcData = Instantiate(npcData);

        npcs = GameObject.FindGameObjectsWithTag("NPC");
        if (npcs.Length > 0)
        {
            print("asdd");
            activeNpc = npcs[0];
        }
    }

    void Update()
    {
        SceneChange();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isMoving)
            {
                navData.TryGetValue(npcData.CurrentPointID, out startPoint);
                ChooseRandomTargetPoint();
                BuildPathToTargetPoint();
                isMoving = true;
                StartCoroutine(MoveToTargetPoint());
            }
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            print("SceneIndex: " + npcData.SceneIndex.ToString());
            print("CurrentPointID: " + npcData.CurrentPointID.ToString());
            print("Position: " + npcData.Position.ToString());
            print("Speed: " + npcData.Speed.ToString());
            print("Height: " + npcData.Height.ToString());
        }
    }

    void ChooseRandomTargetPoint()
    {
        int index = Random.Range(0, navGraphData.GetData().Count);
        targetPoint = navGraphData.GetData()[index];
        if (targetPoint.id == npcData.CurrentPointID)
        {
            ChooseRandomTargetPoint();
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
        }
    }

    void SceneChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadScene(1);
        }
    }

    IEnumerator MoveToTargetPoint()
    {
        while (true)
        {
            if (pathIsBuild())
            {
                NavGraphPoint nextPoint;
                if (targetPath.Count > 0)
                {
                    nextPoint = targetPath[0];
                    targetPath.RemoveAt(0);
                    if (nextPoint.sceneIndex != SceneManager.GetActiveScene().buildIndex && nextPoint.sceneIndex != npcData.SceneIndex)
                    {
                        print("Переход на др сцену");
                    }
                    if (nextPoint.sceneIndex != SceneManager.GetActiveScene().buildIndex && nextPoint.sceneIndex == npcData.SceneIndex)
                    {
                        print("Скрытое перемещение");
                        yield return StartCoroutine(hiddenMovement(nextPoint));
                        npcData.CurrentPointID = nextPoint.id;
                    }
                    if (nextPoint.sceneIndex == SceneManager.GetActiveScene().buildIndex && nextPoint.sceneIndex != npcData.SceneIndex)
                    {
                        print("Переход на активную сцену");
                    }
                    if (nextPoint.sceneIndex == SceneManager.GetActiveScene().buildIndex && nextPoint.sceneIndex == npcData.SceneIndex)
                    {
                        activeNpc = GameObject.FindGameObjectWithTag("NPC");
                        print("Детальное движение");
                        activeNpc.GetComponent<Npc>().moveTo(nextPoint);
                    }
                }
                else
                {
                    isMoving = false;
                    yield break;
                }
            }
            else
            {
                isMoving = false;
                yield break;
            }
        }
    }

    IEnumerator hiddenMovement(NavGraphPoint point)
    {
        while (true)
        {
            Vector3 dir = ((point.position + new Vector3(0, npcData.Height / 2, 0)) - npcData.Position).normalized * npcData.Speed / 5;
            if (Mathf.Abs(Vector3.Distance(npcData.Position, (point.position + new Vector3(0, npcData.Height / 2, 0)))) > 1f)
            {
                npcData.Position += dir;
            }
            else
            {
                yield break;
            }
            yield return new WaitForSeconds(1);
        }
    }

    void spawnNpc(NavGraphPoint point)
    {
        // npcs = GameObject.FindGameObjectsWithTag("NPC");
        // if (npcs.Length > 0)
        // {
        //     return;
        // }
        // else
        // {
        //     activeNpc = Instantiate(NpcPrefab);
        //     activeNpc.GetComponent<Npc>().speed = npcData.Speed;
        //     activeNpc.GetComponent<Npc>().npcHeight = npcData.Height;
        //     activeNpc.transform.position = point.position + new Vector3(0, npcData.Height / 2, 0);
        // }
    }
}
