using Codice.Client.Common.GameUI;
using Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnitBrains;
using UnitBrains.Pathfinding;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AStarUnitPath : BaseUnitPath
{
    private Vector2Int _startPoint;
    private Vector2Int _endPoint;
    private IReadOnlyRuntimeModel _runTimeModel;
    private int[] dx = { -1, 0, 1, 0 };
    private int[] dy = { 0, -1, 0, 1 };

    public AStarUnitPath(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint) : base(runtimeModel, startPoint, endPoint)
    {
        _startPoint = startPoint;
        _endPoint = endPoint;
        _runTimeModel = runtimeModel;
    }

    protected override void Calculate()
    {
        APathNode startNode = new APathNode(_startPoint);
        APathNode targetNode = new APathNode(_endPoint);

        //¬се вершины в которые можно пойти
        List<APathNode> openList = new List<APathNode>() { startNode };
        // все вершины по которым прошли
        List<APathNode> closedList = new List<APathNode>();
        var result = new List<Vector2Int> { _startPoint };

        while (openList.Count > 0)
        {
            APathNode currentNode = openList[0];

            foreach (var node in openList)
            {
                if (node.Value < currentNode.Value)
                {
                    currentNode = node;
                }
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            //ѕроверка на конец пути
            if (currentNode.X == targetNode.X && currentNode.Y == targetNode.Y)
            {
                while (currentNode != null)
                {
                    result.Add(currentNode.ReturnVector2());
                    currentNode = currentNode.Parent;
                }
                result.Reverse();
                path = result.ToArray();
                return;
            }
            //–ассчет следующей ноды
            bool newNodeAdded = false;
            for (int i = 0; i < dx.Length; i++)
            {
                int newX = currentNode.X + dx[i];
                int newY = currentNode.Y + dy[i];
                if (_runTimeModel.IsTileWalkable(new Vector2Int(newX, newY)) || (newX == targetNode.X && newY == targetNode.Y))
                {
                    APathNode neighbor = new APathNode(newX, newY);
                    if (closedList.Contains(neighbor))
                        continue;
                    neighbor.Parent = currentNode;
                    neighbor.CalculateEstimate(targetNode.X, targetNode.Y);
                    neighbor.CalculateValue();

                    openList.Add(neighbor);
                    newNodeAdded = true;
                }
            }
            if (!newNodeAdded)
            {
                Debug.Log("No path found, surrounded by obstacles.");
                result.Add(_startPoint);
                path = result.ToArray();
                return;
            }

        }
    }


}
