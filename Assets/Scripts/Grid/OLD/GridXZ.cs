using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Grid
{
    public class GridXZ<TGridObject>
    {

        public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
        public class OnGridObjectChangedEventArgs : EventArgs
        {
            public int x;
            public int z;
        }

        public int Width { get; private set; }

        public int Length { get; private set; }
        private float cellSize;
        private Vector3 originPosition;
        private TGridObject[,] gridArray;

        public GridXZ(int width, int height, float cellSize, Vector3 originPosition, Func<GridXZ<TGridObject>, int, int, TGridObject> createGridObject)
        {
            this.Width = width;
            this.Length = height;
            this.cellSize = cellSize;
            this.originPosition = originPosition;

            gridArray = new TGridObject[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < gridArray.GetLength(1); z++)
                {
                    gridArray[x, z] = createGridObject(this, x, z);
                }
            }

            bool showDebug = true;
            if (showDebug)
            {
                TextMesh[,] debugTextArray = new TextMesh[width, height];

                for (int x = 0; x < gridArray.GetLength(0); x++)
                {
                    for (int z = 0; z < gridArray.GetLength(1); z++)
                    {
                        //debugTextArray[x, z] = Utils.Utils.CreateWorldText(gridArray[x, z]?.ToString(), null, GetWorldPosition(x, z), 15, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center);
                        Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 100f);
                        Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 100f);
                    }
                }
                Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

                OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) => {
                    //debugTextArray[eventArgs.x, eventArgs.z].text = gridArray[eventArgs.x, eventArgs.z]?.ToString();
                };
            }
        }

        public float GetCellSize()
        {
            return cellSize;
        }

        public Vector3 GetWorldPosition(float x, float z)
        {
            return new Vector3(x, 0, z) * cellSize + originPosition - new Vector3(cellSize * 0.5f, 0, cellSize * 0.5f);
        }

        public void GetXZ(Vector3 worldPosition, out int x, out int z)
        {
            Debug.Log((worldPosition - originPosition).x / cellSize);
            x = Mathf.RoundToInt((worldPosition - originPosition).x / cellSize);
            z = Mathf.RoundToInt((worldPosition - originPosition).z / cellSize);
        }

        public void SetGridObject(int x, int z, TGridObject value)
        {
            if (x >= 0 && z >= 0 && x < Width && z < Length)
            {
                gridArray[x, z] = value;
                TriggerGridObjectChanged(x, z);
            }
        }

        public void TriggerGridObjectChanged(int x, int z)
        {
            OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, z = z });
        }

        public void SetGridObject(Vector3 worldPosition, TGridObject value)
        {
            GetXZ(worldPosition, out int x, out int z);
            SetGridObject(x, z, value);
        }

        public TGridObject GetGridObject(int x, int z)
        {
            if (x >= 0 && z >= 0 && x < Width && z < Length)
            {
                return gridArray[x, z];
            }
            else
            {
                return default(TGridObject);
            }
        }

        public TGridObject GetGridObject(Vector3 worldPosition)
        {
            int x, z;
            GetXZ(worldPosition, out x, out z);
            return GetGridObject(x, z);
        }

        public Vector2Int ValidateGridPosition(Vector2Int gridPosition)
        {
            return new Vector2Int(
                Mathf.Clamp(gridPosition.x, 0, Width - 1),
                Mathf.Clamp(gridPosition.y, 0, Length - 1)
            );
        }

        public void DestroyGridObject(int x, int z)
        {
            if (x >= 0 && z >= 0 && x < Width && z < Length)
            {
            }
        }

    }
}