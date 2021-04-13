using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPC : MonoBehaviour
{
    public NavGraphData navGraphData;
    public float speed;
    int currentSceneIndex;
    NavGraphPoint startPoint, targetPoint;
    List<NavGraphPoint> targetPath;


    bool startPosFlag = true;

    void Start()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        var minDistance = Mathf.Infinity;
        foreach (var item in navGraphData.GetData())
        {    
            if (item.sceneIndex == currentSceneIndex)
            {
                var tD = Vector3.Distance(transform.position, item.position);
                if (tD < minDistance)
                {
                    minDistance = tD;
                    startPoint = item;
                }
            }
        }
    }

    void Update()
    {
        MoveToStartPos();
        if(Input.GetKeyDown(KeyCode.Space))
        {
            targetPoint = getRandomTargetPoint();
            targetPath = FindPath.FindPathBFS(startPoint, targetPoint, navGraphData.GetData());
            MoveToTargetPos();
        }
    }

    void MoveToStartPos()
    {
        if (startPosFlag)
        {
            if (Vector3.Distance(transform.position, startPoint.position) > 0.5f)
            {
                gameObject.transform.Translate((startPoint.position - transform.position) * Time.deltaTime, Space.World);
            }
            else
            {
                startPosFlag = false;
            }
        }
    }

    NavGraphPoint getRandomTargetPoint()
    {
        int index = Random.Range(0, navGraphData.GetData().Count);
        NavGraphPoint result = navGraphData.GetData()[index];
        Debug.Log("Target point: " + result.id.ToString());
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
