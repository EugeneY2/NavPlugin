using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPC : MonoBehaviour
{
    public NavGraphData navGraphData;
    public float speed;
    int currentSceneIndex;
    NavGraphPoint startPoint;

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
}
