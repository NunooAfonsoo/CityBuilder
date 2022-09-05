using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    public class GridObject
    {

        private GridXZ<GridObject> grid;
        private int x;
        private int y;
        public PlacedObject_Done placedObject;
        NodeType nodeType;
        [SerializeField] private GameObject node;

        public enum NodeType
        {
            Walkable,
            Unwalkable,
            Buildable,
            Unbuildable
        }

        public enum NodeSprite
        {
            Grass,
            Stone,
            Wood
        }

        public GridObject(GridXZ<GridObject> grid, int x, int y, NodeType nodeType, GameObject obj)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            placedObject = null;
            this.nodeType = nodeType;
            GameObject newobj = GameObject.Instantiate(obj);
            newobj.transform.position = new Vector3(x, -0.5f, y);
        }

        public override string ToString()
        {
            switch (nodeType)
            {
                case NodeType.Walkable:
                    return "Walkable";
                case NodeType.Unwalkable:
                    return "Unwalkable";
                case NodeType.Buildable:
                    return "Buildable";
                case NodeType.Unbuildable:
                    return "Unbuildable";
                default:
                    return "";
            }
        }

        public void SetPlacedObject(PlacedObject_Done placedObject)
        {
            this.placedObject = placedObject;
            grid.TriggerGridObjectChanged(x, y);
        }

        public void ClearPlacedObject()
        {
            placedObject = null;
            grid.TriggerGridObjectChanged(x, y);
        }

        public PlacedObject_Done GetPlacedObject()
        {
            return placedObject;
        }

        public bool CanBuild()
        {
            return placedObject == null;
        }

    }
}