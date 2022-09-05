using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Grid
{
    public class Test : MonoBehaviour
    {
        public static Test Instance { get; private set; }

        public event EventHandler OnSelectedChanged;
        public event EventHandler OnObjectPlaced;


        private GridXZ<GridObject> grid;
        [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList = null;
        private PlacedObjectTypeSO placedObjectTypeSO;
        private PlacedObjectTypeSO.Dir dir;

        [SerializeField] private int gridWidth;
        [SerializeField] private int gridLength;        
        [SerializeField] private float cellSize;

        [SerializeField] private GridVisual gridVisual;
        [SerializeField] private GameObject[] possibleGridObjs;

        private void Awake()
        {
            Instance = this;

            grid = new GridXZ<GridObject>(gridWidth, gridLength, cellSize, new Vector3(0, 0, 0), (GridXZ<GridObject> g, int x, int y) => new GridObject(grid, x, y, GridObject.NodeType.Walkable, possibleGridObjs[UnityEngine.Random.Range(0, 2)]));
            //gridVisual.SetGrid(grid);

            placedObjectTypeSO = null;// placedObjectTypeSOList[0];

        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                //Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
                //grid.GetXZ(mousePosition, out int x, out int z);
                //Debug.Log(mousePosition + " " + x + " " + z);
                //grid.DestroyGridObject(x, z);
                //grid.SetGridObject(mousePosition, new GridObject(grid, x, z, GridObject.NodeType.Unwalkable, possibleGridObjs[UnityEngine.Random.Range(0, 3)]));
            }

            /*
            if (Mouse.current.leftButton.wasPressedThisFrame && placedObjectTypeSO != null)
            {
                Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
                grid.GetXZ(mousePosition, out int x, out int z);

                Vector2Int placedObjectOrigin = new Vector2Int(x, z);
                placedObjectOrigin = grid.ValidateGridPosition(placedObjectOrigin);

                // Test Can Build
                List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(placedObjectOrigin, dir);
                bool canBuild = true;
                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                    {
                        canBuild = false;
                        break;
                    }
                }

                if (canBuild)
                {
                    Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
                    Vector3 placedObjectWorldPosition = grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();

                    PlacedObject_Done placedObject = PlacedObject_Done.Create(placedObjectWorldPosition, placedObjectOrigin, dir, placedObjectTypeSO);

                    foreach (Vector2Int gridPosition in gridPositionList)
                    {
                        grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                    }

                    OnObjectPlaced?.Invoke(this, EventArgs.Empty);

                    //DeselectObjectType();
                }
                else
                {
                }
            }

            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                dir = PlacedObjectTypeSO.GetNextDir(dir);
            }

            if (Keyboard.current.digit1Key.wasPressedThisFrame) { placedObjectTypeSO = placedObjectTypeSOList[0]; RefreshSelectedObjectType(); }
            if (Keyboard.current.digit2Key.wasPressedThisFrame) { placedObjectTypeSO = placedObjectTypeSOList[1]; RefreshSelectedObjectType(); }
            if (Keyboard.current.digit3Key.wasPressedThisFrame) { placedObjectTypeSO = placedObjectTypeSOList[2]; RefreshSelectedObjectType(); }
            if (Keyboard.current.digit4Key.wasPressedThisFrame) { placedObjectTypeSO = placedObjectTypeSOList[3]; RefreshSelectedObjectType(); }
            if (Keyboard.current.digit5Key.wasPressedThisFrame) { placedObjectTypeSO = placedObjectTypeSOList[4]; RefreshSelectedObjectType(); }
            if (Keyboard.current.digit6Key.wasPressedThisFrame) { placedObjectTypeSO = placedObjectTypeSOList[5]; RefreshSelectedObjectType(); }

            if (Keyboard.current.digit0Key.wasPressedThisFrame) { DeselectObjectType(); }


            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
                if (grid.GetGridObject(mousePosition) != null)
                {
                    // Valid Grid Position
                    PlacedObject_Done placedObject = grid.GetGridObject(mousePosition).GetPlacedObject();
                    if (placedObject != null)
                    {
                        // Demolish
                        placedObject.DestroySelf();

                        List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
                        foreach (Vector2Int gridPosition in gridPositionList)
                        {
                            grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                        }
                    }
                }
            }
             */

        }

        private void DeselectObjectType()
        {
            placedObjectTypeSO = null; RefreshSelectedObjectType();
        }

        private void RefreshSelectedObjectType()
        {
            OnSelectedChanged?.Invoke(this, EventArgs.Empty);
        }


        public Vector2Int GetGridPosition(Vector3 worldPosition)
        {
            grid.GetXZ(worldPosition, out int x, out int z);
            return new Vector2Int(x, z);
        }

        public Vector3 GetMouseWorldSnappedPosition()
        {
            /*
            Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
            grid.GetXZ(mousePosition, out int x, out int z);

            if (placedObjectTypeSO != null)
            {
                Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
                Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
                return placedObjectWorldPosition;
            }
            else
            {
                return mousePosition;
            }
             */
            return Vector3.zero;
        }

        public Quaternion GetPlacedObjectRotation()
        {
            if (placedObjectTypeSO != null)
            {
                return Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0);
            }
            else
            {
                return Quaternion.identity;
            }
        }

        public PlacedObjectTypeSO GetPlacedObjectTypeSO()
        {
            return placedObjectTypeSO;
        }
    }
}