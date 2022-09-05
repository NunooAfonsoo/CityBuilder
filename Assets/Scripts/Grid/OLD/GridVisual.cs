using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    public class GridVisual : MonoBehaviour
    {
        private GridXZ<GridObject> grid;
        private Mesh mesh;
        private bool updateMesh;

        private void Awake()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
        }

        private void LateUpdate()
        {
            if(updateMesh)
            {
                updateMesh = false;
                UpdateVisualMesh();
            }
        }

        public void SetGrid(GridXZ<GridObject> grid)
        {
            this.grid = grid;
            UpdateVisualMesh();

            grid.OnGridObjectChanged += Grid_OnGridObjectChanged;
        }

        private void Grid_OnGridObjectChanged(object sender, GridXZ<GridObject>.OnGridObjectChangedEventArgs e)
        {
            updateMesh = true;
        }

        private void UpdateVisualMesh()
        {
            MeshUtils.CreateEmptyMeshArrays(grid.Width * grid.Length, out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Length; y++)
                {
                    int index = x * grid.Length + y;
                    Vector3 quadSize = new Vector3(1, 0, 1) * grid.GetCellSize();

                    GridObject gridObject = grid.GetGridObject(x, y);

                    float gridValueNormalized = 100f;
                    Vector2 gridValueUV = new Vector2(gridValueNormalized, 0f);

                    MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, grid.GetWorldPosition(x, y) + quadSize * .5f, 0f, quadSize, Vector2.zero, Vector2.zero);
                }
            }

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
        }
    }
}
