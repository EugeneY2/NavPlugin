using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavController : MonoBehaviour
{
    public NavGraphData navGraphData;
    public NpcData npcData;
    NavGraphPoint startPoint, targetPoint;
    List<NavGraphPoint> targetPath;
    Dictionary<int, NavGraphPoint> navData;

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
    }

    void Update()
    {
        SceneChange();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            navData.TryGetValue(npcData.CurrentPointID, out startPoint);
            ChooseRandomTargetPoint();
            BuildPathToTargetPoint();
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
        Debug.Log("Target point: " + targetPoint.id);
        npcData.CurrentPointID = targetPoint.id;
    }

    void BuildPathToTargetPoint()
    {
        targetPath = FindPath.FindPathBFS(startPoint.id, targetPoint.id, navData);
        if (targetPath.Count == 0)
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
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene(0);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadScene(1);
        }
    }
}
