using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WayPoint : MonoBehaviour
{
    public List<GameObject> connectedPoints;
    [ShowOnly] public int id;
    [ShowOnly] public int sceneIndex;
    public bool isDoor;
    public int doorPairID;

    public void UpdateConnected()
    {
        if (connectedPoints != null)
        {
            foreach (var item in connectedPoints)
            {
                item.GetComponent<WayPoint>().AddConnectedPoint(gameObject);
            }
        }
    }

    public void AddConnectedPoint(GameObject item)
    {
        if (connectedPoints != null)
        {
            if (!connectedPoints.Contains(item))
            {
                connectedPoints.Add(item);
            }
        }
        else
        {
            connectedPoints = new List<GameObject>();
            connectedPoints.Add(item);
        }
    }
}

[CustomEditor(typeof(WayPoint))]
public class WayPointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WayPoint wp = (WayPoint)target;
        DrawDefaultInspector();
        wp.GetComponent<WayPoint>().UpdateConnected();
        if(!wp.isDoor)
        {
            wp.doorPairID = 0;
        }
    }
}
