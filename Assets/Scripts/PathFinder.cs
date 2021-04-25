using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public NavGraphData navGraphData;
    public int startPointId;
    NavGraphPoint startPoint, targetPoint;
    List<NavGraphPoint> targetPath;
    Dictionary<int, NavGraphPoint> data;

    void Start()
    {
        data = new Dictionary<int, NavGraphPoint>();
        foreach (var item in navGraphData.GetData())
        {
            data.Add(item.id, item);
        }
        data.TryGetValue(startPointId, out startPoint);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            targetPoint = ChooseRandomTargetPoint();
            targetPath = FindPath.FindPathBFS(startPoint, targetPoint, data);
            MoveToTargetPos();
        }
    }

    NavGraphPoint ChooseRandomTargetPoint()
    {
        int index = Random.Range(0, navGraphData.GetData().Count);
        NavGraphPoint result = navGraphData.GetData()[index];
        Debug.Log("Target point: " + result.id);
        return result;
    }

    void MoveToTargetPos()
    {
        if(targetPath.Count == 0)
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
}
