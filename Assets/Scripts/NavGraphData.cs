using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NavGraphData", menuName = "NavGraphData", order = 0)]
public class NavGraphData : ScriptableObject
{ 
    public List<NavGraphPoint> data;

    public void AddToData(NavGraphPoint item)
    {
        if (data != null)
        {
            int index = 0;
            foreach (var point in data)
            {
                if (point.id == item.id)
                {
                    data.RemoveAt(index);
                    break;
                }
                index++;
            }
            data.Add(item);
        }
        else
        {
            data = new List<NavGraphPoint>();
            data.Add(item);
        }
    }

    public void ClearData()
    {
        data.Clear();
    }

    public List<NavGraphPoint> GetData()
    {
        return data;
    }
}
