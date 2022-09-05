using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cursor;

namespace Buildings
{
    [CreateAssetMenu(fileName = "BuildingSO", menuName = "Building/Road")]
    public class RoadSO : BuildingSO
    {
        [SerializeField] private List<GameObject> buildingObjs;


        public override void BuildingPlaced(int x, int y)
        {
            Grid.Grid.Instance.GetCell(x, y).RoadCreated(building);
            CursorManager.Instance.EnableCursorMarker();
        }

        public override void DestroyBuilding()
        {
            foreach(GameObject buildingObj in buildingObjs)
            {
                Destroy(buildingObj);
            }
            buildingObjs.Clear();
        }

        public List<GameObject> GetBuildingObjs()
        {
            return buildingObjs;
        }

        public void AddToBuildingObjs(GameObject building)
        {
            buildingObjs.Add(building);
        }
    }
}
