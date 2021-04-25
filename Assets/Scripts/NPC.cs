using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPC : MonoBehaviour
{
    public float speed;
    int currentSceneIndex;
    NavGraphPoint startPoint, targetPoint;
    List<NavGraphPoint> targetPath;

    bool startPosFlag = true;

    void Start()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    void Update()
    {
        
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
