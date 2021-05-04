using System.Collections.Generic;

static class FindPath
{
    public static List<NavGraphPoint> FindPathBFS(int start, int end, Dictionary<int, NavGraphPoint> navGraph)
    {
        List<NavGraphPoint> result = new List<NavGraphPoint>();
        Queue<NavGraphPoint> pointsToCheck = new Queue<NavGraphPoint>();
        Dictionary<int, int> predecessor = new Dictionary<int, int>();
        List<int> connected = new List<int>();
        bool findComplete = false;
        NavGraphPoint currentPoint, startPoint, endPoint;
        navGraph.TryGetValue(start, out startPoint);
        navGraph.TryGetValue(end, out endPoint);
        pointsToCheck.Enqueue(startPoint);

        while (pointsToCheck.Count != 0)
        {
            currentPoint = pointsToCheck.Dequeue();
            connected = currentPoint.connectedIDs;
            foreach (var item in connected)
            {
                if (!predecessor.ContainsKey(item) && !predecessor.ContainsValue(item))
                {
                    if (item == end)
                    {
                        predecessor.Add(item, currentPoint.id);
                        findComplete = true;
                        pointsToCheck.Clear();
                        break;
                    }
                    else
                    {
                        predecessor.Add(item, currentPoint.id);
                        NavGraphPoint t;
                        navGraph.TryGetValue(item, out t);
                        pointsToCheck.Enqueue(t);
                    }
                }
            }
        }
        if (findComplete)
        {
            NavGraphPoint temp = endPoint;
            int predID = 0;
            while (temp.id != start)
            {
                result.Insert(0, temp);
                if (predecessor.TryGetValue(temp.id, out predID))
                {
                    navGraph.TryGetValue(predID, out temp);
                }
            }
        }
        return result;
    }
}
