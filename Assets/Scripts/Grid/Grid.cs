using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

namespace Grid
{
    public class Grid
    {
        private static Grid instance;
        public static Grid Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new Grid();
                }
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        private Node[,] grid;
        private int gridSize;
        public int NodeSize { get; private set; }
        public GridGraph gridGraph { get; private set; }


        private int penaltyMin = int.MaxValue;
        private int penaltyMax = int.MinValue;


        private Grid() { }

        public void CreateGrid(int x, int y, int nodeSize)
        {
            grid = new Node[x, y];
            gridSize = x;
            this.NodeSize = nodeSize;
            gridGraph = (GridGraph)AstarPath.active.graphs[0];
        }

        #region ASTAR
        public int Area()
        {
            return gridSize * gridSize;
        }

        public List<Node> GetNeighboursWithDiagonals(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.CellPosition.x + x;
                    int checkY = node.CellPosition.y + y;
                    Vector2Int newPosition = new Vector2Int(checkX, checkY);

                    if (IsCellPositionInsideGrid(newPosition))
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }

            return neighbours;
        }

        public List<Node> GetNeighboursWithoutDiagonals(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0 || IsNeighbourNodeDiagonal(node, GetCell(node.CellPosition.x + x, node.CellPosition.y + y)))
                        continue;

                    int checkX = node.CellPosition.x + x;
                    int checkY = node.CellPosition.y + y;
                    Vector2Int newPosition = new Vector2Int(checkX, checkY);

                    if (IsCellPositionInsideGrid(newPosition))
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }

            return neighbours;
        }

        private bool IsNeighbourNodeDiagonal(Node currentNode, Node neighbourNode)
        {
            if (neighbourNode.CellPosition.x == currentNode.CellPosition.x - 1 && neighbourNode.CellPosition.y == currentNode.CellPosition.y - 1 ||
               neighbourNode.CellPosition.x == currentNode.CellPosition.x + 1 && neighbourNode.CellPosition.y == currentNode.CellPosition.y - 1 ||
               neighbourNode.CellPosition.x == currentNode.CellPosition.x + 1 && neighbourNode.CellPosition.y == currentNode.CellPosition.y + 1 ||
               neighbourNode.CellPosition.x == currentNode.CellPosition.x - 1 && neighbourNode.CellPosition.y == currentNode.CellPosition.y + 1)
                return true;

            return false;
        }

        public void UpdateNeighbours(int x, int y)
        {
            gridGraph.GetNode(x, y).GetConnections(connectedTo => {
                connectedTo.Penalty = 10;
            });
        }

        public void BlurPenaltyMap(int blurSize)
        {
            int kernelSize = blurSize * 2 + 1;
            int kernelExtents = (kernelSize - 1) / 2;

            int[,] penaltiesHorizontalPass = new int[gridSize, gridSize];
            int[,] penaltiesVerticalPass = new int[gridSize, gridSize];

            for (int y = 0; y < gridSize; y++)
            {
                for (int x = -kernelExtents; x <= kernelExtents; x++)
                {
                    int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                    penaltiesHorizontalPass[0, y] += grid[sampleX, y].MovementPenalty;
                }

                for (int x = 1; x < gridSize; x++)
                {
                    int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSize);
                    int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridSize - 1);

                    penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - grid[removeIndex, y].MovementPenalty + grid[addIndex, y].MovementPenalty;
                }
            }

            for (int x = 0; x < gridSize; x++)
            {
                for (int y = -kernelExtents; y <= kernelExtents; y++)
                {
                    int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                    penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
                }

                int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
                GetCell(x, 0).SetMovementPenalty(blurredPenalty);

                for (int y = 1; y < gridSize; y++)
                {
                    int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSize);
                    int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSize - 1);

                    penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];
                    blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                    GetCell(x, y).SetMovementPenalty(blurredPenalty);

                    if (blurredPenalty > penaltyMax)
                    {
                        penaltyMax = blurredPenalty;
                    }
                    if (blurredPenalty < penaltyMin)
                    {
                        penaltyMin = blurredPenalty;
                    }
                }
            }
        }

        public void SetNodeWalkability(int x, int y, bool walkable)
        {
            gridGraph.GetNode(x, y).Walkable = walkable;
        }
        #endregion

        public void SetNode(int x, int y, Node cell)
        {
            grid[x, y] = cell;
        }

        public Node GetCell(int x, int y)
        {
            if(IsCellPositionInsideGrid(new Vector2Int(x, y))) return grid[x, y];
            return null;
        }

        public Vector3Int GetGridPositionFromWorldPosition(Vector3 worldPosition)
        {
            return new Vector3Int(Mathf.RoundToInt(worldPosition.x / NodeSize), 0, Mathf.RoundToInt(worldPosition.z / NodeSize));
        }

        private bool IsCellPositionInsideGrid(Vector2Int position)
        {
            return position.x >= 0 && position.x < gridSize && position.y >= 0 && position.y < gridSize;
        }

        public List<Vector2Int> SearchForClosestFreeCellPosition(Vector2Int initialPos, int x, int y, List<Vector2Int> visitedCells, List<Vector2Int> closestCells)
        {
            Node cell = GetCell(x, y);
            visitedCells.Add(cell.CellPosition);

            if (cell.CanBuildAtPosition())
            {
                closestCells.Add(cell.CellPosition);
                return closestCells;
            }

            Node[] neighbourCells = { GetCell(x + 1, y), GetCell(x - 1, y), GetCell(x, y + 1), GetCell(x, y - 1) };

            foreach (Node neighbourCell in neighbourCells)
            {
                if (neighbourCell != null && !visitedCells.Contains(neighbourCell.CellPosition) && Vector2Int.Distance(initialPos, neighbourCell.CellPosition) < 20)
                {
                    SearchForClosestFreeCellPosition(initialPos, neighbourCell.CellPosition.x, neighbourCell.CellPosition.y, visitedCells, closestCells);
                }
            }
            return closestCells;
        }


        public List<Vector2Int> SearchForClosestWaterCellPosition(Vector2Int initialPos, int x, int y, List<Vector2Int> visitedCells, List<Vector2Int> closestCells)
        {
            Node cell = GetCell(x, y);
            visitedCells.Add(cell.CellPosition);

            if (cell.IsWater) //FIX ME NEEDS TO SEE IF HAS BUILDING
            {
                closestCells.Add(cell.CellPosition);
                return closestCells;
            }

            Node[] neighbourCells = { GetCell(x + 1, y), GetCell(x - 1, y), GetCell(x, y + 1), GetCell(x, y - 1) };

            foreach (Node neighbourCell in neighbourCells)
            {
                if (neighbourCell != null && !visitedCells.Contains(neighbourCell.CellPosition) && Vector2Int.Distance(initialPos, neighbourCell.CellPosition) < 20)
                {
                    SearchForClosestFreeCellPosition(initialPos, neighbourCell.CellPosition.x, neighbourCell.CellPosition.y, visitedCells, closestCells);
                }
            }
            return closestCells;
        }

        /*
        void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridSize, 1, gridSize));
            if (grid != null)
            {
                foreach (Node n in grid)
                {

                    Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, n.MovementPenalty));
                    Gizmos.color = (n.IsWalkable()) ? Gizmos.color : Color.red;
                    Gizmos.DrawCube(new Vector3(n.CellPosition.x, 0.001f, n.CellPosition.y), Vector3.one * (1));
                }
            }
        }
        */

    }
}
