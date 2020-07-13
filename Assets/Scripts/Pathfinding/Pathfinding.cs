/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperTiled2Unity;

public class Pathfinding {

    public static Pathfinding Instance { get; private set; }

    private GridManager gridManager;
    private List<GridTile> openList;
    private List<GridTile> closedList;

    public Pathfinding(SuperMap map) {
        Instance = this;
        gridManager = new GridManager(map);
    }

    public GridManager GetGridManager() {
        return gridManager;
    }

    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition) {
        gridManager.GetXY(startWorldPosition, out int startX, out int startY);
        gridManager.GetXY(endWorldPosition, out int endX, out int endY);

        List<GridTile> path = FindPath(startX, startY, endX, endY);
        if (path == null) {
            return null;
        } else {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach (GridTile gridTile in path) {
                vectorPath.Add(new Vector3(gridTile.x, gridTile.y) * gridManager.GetCellWidth() + Vector3.one * gridManager.GetCellWidth() * .5f);
            }
            return vectorPath;
        }
    }

    public List<GridTile> FindPath(int startX, int startY, int endX, int endY) {
        GridTile startNode = gridManager.GetGridTile(startX, startY);
        GridTile endNode = gridManager.GetGridTile(endX, endY);

        Debug.Log("startNode: " + startNode + ", endNode: " + endNode);
        if (startNode == null || endNode == null) {
            // Invalid Path
            Debug.Log("Invalid Path");
            return null;
        }

        openList = new List<GridTile> { startNode };
        closedList = new List<GridTile>();

        for (int x = 0; x < gridManager.GetWidth(); x++) {
            for (int y = 0; y < gridManager.GetHeight(); y++) {
                GridTile GridTile = gridManager.GetGridTile(x, -y);
                GridTile.gCost = 99999999;
                GridTile.CalculateFCost();
                GridTile.cameFromTile = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();
        
        //PathfindingDebugStepVisual.Instance.ClearSnapshots();
        //PathfindingDebugStepVisual.Instance.TakeSnapshot(grid, startNode, openList, closedList);

        while (openList.Count > 0) {
            GridTile currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode) {
                // Reached final node
                //PathfindingDebugStepVisual.Instance.TakeSnapshot(grid, currentNode, openList, closedList);
                //PathfindingDebugStepVisual.Instance.TakeSnapshotFinalPath(grid, CalculatePath(endNode));
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            Debug.Log("currentNode: " + currentNode);
            foreach (GridTile neighbourNode in GetNeighbourList(currentNode)) {
                if (closedList.Contains(neighbourNode)) continue;
                if (!neighbourNode.isWalkable) {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost) {
                    neighbourNode.cameFromTile = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode)) {
                        openList.Add(neighbourNode);
                    }
                }
                //PathfindingDebugStepVisual.Instance.TakeSnapshot(grid, currentNode, openList, closedList);
            }
        }

        // Out of nodes on the openList
        return null;
    }

    private List<GridTile> GetNeighbourList(GridTile currentNode) {
        List<GridTile> neighbourList = new List<GridTile>();

        // Left
        if (currentNode.x - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
        
        // Right
        if (currentNode.x + 1 < gridManager.GetWidth()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
        
        // Down
        if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
        
        // Up
        if (currentNode.y + 1 < gridManager.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighbourList;
    }

    public GridTile GetNode(int x, int y) {
        return gridManager.GetGridTile(x, -y);
    }

    private List<GridTile> CalculatePath(GridTile endNode) {
        List<GridTile> path = new List<GridTile>();
        path.Add(endNode);
        GridTile currentNode = endNode;
        while (currentNode.cameFromTile != null) {
            path.Add(currentNode.cameFromTile);
            currentNode = currentNode.cameFromTile;
        }
        path.Reverse();
        return path;
    }

    private int CalculateDistanceCost(GridTile a, GridTile b) {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return b.cost * remaining;
    }

    private GridTile GetLowestFCostNode(List<GridTile> GridTileList) {
        GridTile lowestFCostNode = GridTileList[0];
        for (int i = 1; i < GridTileList.Count; i++) {
            if (GridTileList[i].fCost < lowestFCostNode.fCost) {
                lowestFCostNode = GridTileList[i];
            }
        }
        return lowestFCostNode;
    }

}
