using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System;

public class WayPointSystem : MonoBehaviour
{
    public NavGraphData navGraphData;
    public List<GameObject> wayPoints;
    int id;

    public void Init()
    {
        wayPoints = new List<GameObject>();
    }

    void OnDrawGizmos()
    {
        if (wayPoints.Count > 0)
        {
            foreach (var point in wayPoints)
            {
                Handles.DrawWireCube(point.transform.position, new Vector3(1, 1, 1));
                if (point.GetComponent<WayPoint>().connectedPoints != null)
                {
                    foreach (var item in point.GetComponent<WayPoint>().connectedPoints)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawLine(point.transform.position, item.transform.position);
                    }
                }
            }
        }
    }

    public void AddNewWayPoint()
    {
        var wps = GameObject.FindGameObjectsWithTag("WP");
        if(wps.Length != 0)
        {
            id = wps[0].GetComponent<WayPoint>().id;
            foreach (var item in wps)
            {
                if(item.GetComponent<WayPoint>().id > id)
                {
                    id = item.GetComponent<WayPoint>().id;
                }
            }
            id++;
        }
        else
        {
            id = Int32.Parse((SceneManager.GetActiveScene().buildIndex + 1).ToString() + 0);
        }
        GameObject go = Resources.Load<GameObject>("WayPoint");
        go = Instantiate<GameObject>(go);
        go.tag = "WP";
        go.transform.parent = this.transform;
        go.name = "WayPoint_" + id.ToString();
        go.GetComponent<WayPoint>().id = id;
        go.GetComponent<WayPoint>().sceneIndex = SceneManager.GetActiveScene().buildIndex;
        wayPoints.Add(go);
    }

    public void DeleteWayPoint(GameObject point)
    {
        if (wayPoints != null)
        {
            if (wayPoints.Contains(point))
            {
                wayPoints.Remove(point);
            }
        }
    }

    public void ClearWayPoints()
    {
        wayPoints.Clear();
        foreach (var item in GameObject.FindGameObjectsWithTag("WP"))
        {
            DestroyImmediate(item);
        }
    }

    public void AddToNavGraph()
    {
        foreach (var point in wayPoints)
        {
            NavGraphPoint wpData = new NavGraphPoint();
            wpData.id = point.GetComponent<WayPoint>().id;
            wpData.sceneIndex = point.GetComponent<WayPoint>().sceneIndex;
            if (point.GetComponent<WayPoint>().isDoor)
            {
                wpData.connectedIDs.Add(point.GetComponent<WayPoint>().doorPairID);
            }
            wpData.position = point.transform.position;
            foreach (var conP in point.GetComponent<WayPoint>().connectedPoints)
            {
                wpData.connectedIDs.Add(conP.GetComponent<WayPoint>().id);
            }
            wpData.isDoor = point.GetComponent<WayPoint>().isDoor;
            navGraphData.AddToData(wpData);
        }
    }
}

[Serializable]
public class NavGraphPoint
{
    [SerializeField]
    public int id;
    [SerializeField]
    public int sceneIndex;
    [SerializeField]
    public Vector3 position;
    [SerializeField]
    public List<int> connectedIDs;
    [SerializeField]
    public bool isDoor;

    public NavGraphPoint()
    {
        connectedIDs = new List<int>();
    }
}
