using System.Collections.Generic;

static class FindPath
{
    public static List<NavGraphPoint> FindPathBFS(NavGraphPoint start, NavGraphPoint end, List<NavGraphPoint> navGraph)
    {
        List<NavGraphPoint> result = new List<NavGraphPoint>();
        Queue<NavGraphPoint> pointsToCheck = new Queue<NavGraphPoint>();
        List<int> checkedPoints = new List<int>();
        Dictionary<int, int> predecessor = new Dictionary<int, int>();
        bool findComplete = false;
        NavGraphPoint currentPoint;
        List<int> connected = new List<int>();

        pointsToCheck.Enqueue(start);

        while (pointsToCheck.Count != 0)
        {
            currentPoint = pointsToCheck.Dequeue();
            connected = currentPoint.connectedIDs;
            foreach (var item in connected)
            {
                if (!checkedPoints.Contains(item) && !pointsToCheck.Contains(navGraph.Find(x => x.id == item)))
                {
                    if (item == end.id)
                    {
                        predecessor.Add(item, currentPoint.id);
                        findComplete = true;
                        pointsToCheck.Clear();
                        break;
                    }
                    else
                    {
                        predecessor.Add(item, currentPoint.id);
                        pointsToCheck.Enqueue(navGraph.Find(x => x.id == item));
                    }
                }
            }
            checkedPoints.Add(currentPoint.id);
        }
        if (findComplete)
        {
            NavGraphPoint temp = end;
            int predID = 0;
            while (temp.id != start.id)
            {
                result.Insert(0, temp);
                if (predecessor.TryGetValue(temp.id, out predID))
                {
                    temp = navGraph.Find(x => x.id == predID);
                }
            }
        }
        return result;
    }
}
