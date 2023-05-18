using System.Collections.Generic;
using UnityEngine;
using Cursor;

namespace Buildings
{
    [CreateAssetMenu(fileName = "BuildingSO", menuName = "Building/NormalBuilding")]

    public class BuildingSO : ScriptableObject
    {
        public string BuildingName { get; private set; }

        [Space(20)]
        [Header("Resources Required")]
        [SerializeField] private int woodRequired;
        [SerializeField] private int stoneRequired;
        [SerializeField] private int ironRequired;
        [SerializeField] private int goldRequired;

        [Space(20)]
        [SerializeField] protected GameObject buildingObj;
        protected GameObject building;
        public float BuildingRotation { get; private set; }

        public void CreateBuilding(Vector3 position, float buildingRotation = 0)
        {
            building = GameObject.Instantiate(buildingObj, position, Quaternion.Euler(0, buildingRotation, 0));
            building.transform.localScale = Vector3.one * Grid.Grid.Instance.NodeSize;

            CursorManager.Instance.DisableCursorMarker();
        }

        public void RotateBuilding()
        {
            building.transform.Rotate(new Vector3(0, 90, 0));
            BuildingRotation = building.transform.transform.eulerAngles.y;
        }

        public virtual void MoveBuildingToClosestAvailablePosition(Vector3Int position)
        {
            Vector2Int cellPosition = new Vector2Int(position.x, position.z);

            List<Vector2Int> newPositions = Grid.Grid.Instance.SearchForClosestFreeCellPosition(cellPosition, cellPosition.x, cellPosition.y, new List<Vector2Int>(), new List<Vector2Int>());

            Vector2Int closestPosition = new Vector2Int(100000, 100000);

            foreach (Vector2Int pos in newPositions)
            {
                if(Vector2.Distance(cellPosition, pos) < Vector2.Distance(cellPosition, closestPosition))
                {
                    closestPosition = pos;
                }
            }

            building.transform.position = new Vector3(closestPosition.x, buildingObj.transform.position.y, closestPosition.y);
        }

        public void MoveBuildingToPosition(Vector3 position)
        {
            building.transform.position = new Vector3(position.x, buildingObj.transform.position.y, position.z);
        }

        public virtual bool IsPositionFree(Vector3Int position)
        {
            bool isPositionFree = Grid.Grid.Instance.GetCell(position.x, position.z).CanBuildAtPosition();

            return isPositionFree;
        }

        public void SetBuildingMaterials(bool isPositionFree)
        {
            if (isPositionFree)
            {
                building.transform.GetChild(0).gameObject.SetActive(true);
                building.transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                building.transform.GetChild(0).gameObject.SetActive(false);
                building.transform.GetChild(1).gameObject.SetActive(true);
            }
        }

        public Vector3 GetBuildingPosition()
        {
            return building.transform.position;
        }

        public virtual void BuildingPlaced(int x, int y)
        {
            Grid.Grid.Instance.GetCell(x, y).BuildingCreated(building);
            CursorManager.Instance.EnableCursorMarker();
        }

        public virtual void DestroyBuilding()
        {
            Destroy(building);
            building = null;
            CursorManager.Instance.EnableCursorMarker();
        }
    }
}