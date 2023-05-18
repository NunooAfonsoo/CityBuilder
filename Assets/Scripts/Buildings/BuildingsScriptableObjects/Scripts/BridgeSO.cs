using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
    [CreateAssetMenu(fileName = "BuildingSO", menuName = "Building/Bridge")]
    public class BridgeSO : BuildingSO
    {

        public override void MoveBuildingToClosestAvailablePosition(Vector3Int position)
        {
            Vector2Int cellPosition = new Vector2Int(position.x, position.z);

            List<Vector2Int> newPositions = Grid.Grid.Instance.SearchForClosestWaterCellPosition(cellPosition, cellPosition.x, cellPosition.y, new List<Vector2Int>(), new List<Vector2Int>());

            Vector2Int closestPosition = new Vector2Int(100000, 100000);

            foreach (Vector2Int pos in newPositions)
            {
                if (Vector2.Distance(cellPosition, pos) < Vector2.Distance(cellPosition, closestPosition))
                {
                    closestPosition = pos;
                }
            }
            building.transform.position = new Vector3(closestPosition.x, buildingObj.transform.position.y, closestPosition.y);
        }

        public override bool IsPositionFree(Vector3Int position)
        {
            bool isPositionFree = Grid.Grid.Instance.GetCell((int)position.x, (int)position.z).CanBuildAtWaterPosition();
            SetBuildingMaterials(isPositionFree);
            

            return isPositionFree;
        }

        public override void BuildingPlaced(int x, int y)
        {
            Grid.Grid.Instance.GetCell(x, y).SetWater(false);
        }
    }
}
