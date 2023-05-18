using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;


namespace Grid
{
    public class ProceduralTerrain : MonoBehaviour
    {
        public GameObject[] treePrefabs;
        [SerializeField] private float treeNoiseScale;
        [SerializeField] private float treeDensity;

        [SerializeField] private GameObject[] stonePrefabs;
        [SerializeField] private float stoneSpawnProb;

        [SerializeField] private GameObject[] ironPrefabs;
        [SerializeField] private float ironSpawnProb;

        [SerializeField] private GameObject[] goldPrefabs;
        [SerializeField] private float goldSpawnProb;

        [Space(20)]
        [SerializeField] private Color waterColor;
        [SerializeField] private Color grassColor;
        [SerializeField] private Color fertilizedGrass1Color;
        [SerializeField] private Color fertilizedGrass2Color;
        private Material edgeMaterial;

        [SerializeField] private float waterLevel;
        [SerializeField] private float scale;
        [SerializeField] private int gridSize;
        [SerializeField] private float nodeSize;


        [SerializeField] private int chunkSize;
        private List<GameObject> chunks;
        [SerializeField] private Material groundMaterial;
        [SerializeField] private bool drawWater;
        [SerializeField] private bool spawnTrees;
        [SerializeField] private bool spawnMinerals;

        private void Start()
        {
            float[,] noiseMap = new float[gridSize, gridSize];
            (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    float noiseValue = Mathf.PerlinNoise(x * scale + xOffset, y * scale + yOffset);
                    noiseMap[x, y] = noiseValue;
                }
            }

            float[,] falloffMap = new float[gridSize, gridSize];
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    float xv = x / (float)gridSize * 2 - 1;
                    float yv = y / (float)gridSize * 2 - 1;
                    float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));
                    falloffMap[x, y] = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
                }
            }

            Grid.Instance.CreateGrid(gridSize, gridSize, nodeSize);
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    float noiseValue = noiseMap[x, y];
                    noiseValue -= falloffMap[x, y];
                    bool isWater = noiseValue < waterLevel;
                    Node cell = new Node(x, y, isWater);
                    Grid.Instance.SetNode(x, y, cell);
                    Grid.Instance.SetNodeWalkability(x, y, !isWater);
                    if(isWater)
                    {
                        Grid.Instance.UpdateNeighbours(x, y);
                    }
                }
            }

            chunks = new List<GameObject>();
            DrawTerrainMeshByChunks();
            //DrawEdgeMesh(grid); //FIX ME
            DrawTexture();
            if (spawnTrees) GenerateTrees();
            if (spawnMinerals) GenerateMinerals();

            Grid.Instance.BlurPenaltyMap(3);
        }

        private void DrawTerrainMeshByChunks()
        {
            int nChunks = gridSize % chunkSize == 0 ? (int)Mathf.Pow(gridSize / chunkSize, 2) : (int)Mathf.Pow(Mathf.CeilToInt((float)gridSize / (float)chunkSize), 2);
            int chunkIndexY = 0, chunkIndexX = 0;

            while (chunkIndexY + 1 < Mathf.Sqrt(nChunks))
            {
                while (chunkIndexX + 1 < Mathf.Sqrt(nChunks))
                {
                    List<Vector3> vertices = new List<Vector3>();
                    List<int> triangles = new List<int>();
                    List<Vector2> uvs = new List<Vector2>();

                    for (int y = chunkIndexY * chunkSize; y < (chunkIndexY + 1) * chunkSize; y++)
                    {
                        for (int x = chunkIndexX * chunkSize; x < (chunkIndexX + 1) * chunkSize; x++)
                        {
                            if (x < gridSize && y < gridSize)
                            {
                                Node node = Grid.Instance.GetCell(x, y);
                                if (!node.IsWater || drawWater)
                                {
                                    float xFloat = x * nodeSize;
                                    float yFloat = y * nodeSize;

                                    Vector3 a = new Vector3(xFloat - nodeSize / 2, 0, yFloat + nodeSize / 2);
                                    Vector3 b = new Vector3(xFloat + nodeSize / 2, 0, yFloat + nodeSize / 2);
                                    Vector3 c = new Vector3(xFloat - nodeSize / 2, 0, yFloat - nodeSize / 2);
                                    Vector3 d = new Vector3(xFloat + nodeSize / 2, 0, yFloat - nodeSize / 2);
                                    Vector2 uvA = new Vector2(x / (float)gridSize, y / (float)gridSize);
                                    Vector2 uvB = new Vector2((x + nodeSize) / (float)gridSize, y / (float)gridSize);
                                    Vector2 uvC = new Vector2(x / (float)gridSize, (y + nodeSize) / (float)gridSize);
                                    Vector2 uvD = new Vector2((x + nodeSize) / (float)gridSize, (y + nodeSize) / (float)gridSize);
                                    Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                                    Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                                    for (int k = 0; k < 6; k++)
                                    {
                                        vertices.Add(v[k]);
                                        triangles.Add(triangles.Count);
                                        uvs.Add(uv[k]);
                                    }
                                }
                            }
                        }
                    }
                    GameObject chunk = new GameObject("Chunk " + chunkIndexX.ToString() + chunkIndexY.ToString());
                    chunk.transform.SetParent(transform);

                    Mesh mesh = new Mesh();

                    mesh.vertices = vertices.ToArray();
                    mesh.triangles = triangles.ToArray();
                    mesh.uv = uvs.ToArray();
                    mesh.RecalculateNormals();

                    MeshFilter meshFilter = chunk.AddComponent<MeshFilter>();
                    meshFilter.mesh = mesh;

                    MeshRenderer renderer = chunk.AddComponent<MeshRenderer>();
                    renderer.material = groundMaterial;
                    chunks.Add(chunk);
                    chunkIndexX++;
                }
                chunkIndexY++;
                chunkIndexX = 0;
            }

            ChooseGrassFertilizer();
        }

        private void ChooseGrassFertilizer()
        {
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    Node cell = Grid.Instance.GetCell(x, y);
                    if (!cell.IsWater)
                    {
                        float fertilityProb = Random.Range(0.0f, 1.0f);
                        float probMediumFert = 0.0f;
                        float probHighFert = 0.0f;

                        switch (cell.GrassFertilityLevel)
                        {
                            case Node.GrassFertility.Little:
                                probMediumFert = 0.025f;
                                probHighFert = 0.04f;
                                break;
                            case Node.GrassFertility.Medium:
                                probMediumFert = 0.05f;
                                probHighFert = 0.075f;
                                break;
                            case Node.GrassFertility.High:
                                probMediumFert = 0.075f;
                                probHighFert = 0.20f;
                                break;
                        }

                        if (cell.GrassFertilityLevel == Node.GrassFertility.Little)
                        {
                            if (fertilityProb < probMediumFert)
                            {
                                cell.SetFertilityLevel(Node.GrassFertility.Medium);
                            }
                            else if (fertilityProb < probHighFert)
                            {
                                cell.SetFertilityLevel(Node.GrassFertility.High);
                            }
                        }
                        SpreadFertility(x, y, cell.GrassFertilityLevel);
                    }
                }
            }
        }

        private void SpreadFertility(int x, int y, Node.GrassFertility grassFertilityLevel)
        {
            Node[] neighbourCells = { Grid.Instance.GetCell(x, y + 1), Grid.Instance.GetCell(x + 1, y), Grid.Instance.GetCell(x, y - 1), Grid.Instance.GetCell(x - 1, y) };

            foreach (Node node in neighbourCells)
            {
                if (node.GrassFertilityLevel == Node.GrassFertility.Little)
                {
                    float fertilityProb = Random.Range(0.0f, 1.0f);
                    switch (grassFertilityLevel)
                    {
                        case Node.GrassFertility.Medium:
                            if (fertilityProb < 0.17f)
                            {
                                node.SetFertilityLevel(Node.GrassFertility.Medium);
                            }
                            else if (fertilityProb < 0.28f)
                            {
                                node.SetFertilityLevel(Node.GrassFertility.High);
                            }
                            break;

                        case Node.GrassFertility.High:
                            if (fertilityProb < 0.24f)
                            {
                                node.SetFertilityLevel(Node.GrassFertility.Medium);
                            }
                            else if (fertilityProb < 0.55f)
                            {
                                node.SetFertilityLevel(Node.GrassFertility.High);
                            }
                            break;
                    }
                }
            }
        }

        private void DrawEdgeMesh()
        {
            Mesh mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    Node cell = Grid.Instance.GetCell(x, y);
                    if (!cell.IsWater)
                    {
                        if (x > 0)
                        {
                            Node left = Grid.Instance.GetCell(x - 1, y);
                            if (left.IsWater)
                            {
                                Vector3 a = new Vector3(x - .5f, 0, y + .5f);
                                Vector3 b = new Vector3(x - .5f, 0, y - .5f);
                                Vector3 c = new Vector3(x - .5f, -1, y + .5f);
                                Vector3 d = new Vector3(x - .5f, -1, y - .5f);
                                Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                                for (int k = 0; k < 6; k++)
                                {
                                    vertices.Add(v[k]);
                                    triangles.Add(triangles.Count);
                                }
                            }
                        }
                        if (x < gridSize - 1)
                        {
                            Node right = Grid.Instance.GetCell(x + 1, y);
                            if (right.IsWater)
                            {
                                Vector3 a = new Vector3(x + .5f, 0, y - .5f);
                                Vector3 b = new Vector3(x + .5f, 0, y + .5f);
                                Vector3 c = new Vector3(x + .5f, -1, y - .5f);
                                Vector3 d = new Vector3(x + .5f, -1, y + .5f);
                                Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                                for (int k = 0; k < 6; k++)
                                {
                                    vertices.Add(v[k]);
                                    triangles.Add(triangles.Count);
                                }
                            }
                        }
                        if (y > 0)
                        {
                            Node down = Grid.Instance.GetCell(x, y - 1);
                            if (down.IsWater)
                            {
                                Vector3 a = new Vector3(x - .5f, 0, y - .5f);
                                Vector3 b = new Vector3(x + .5f, 0, y - .5f);
                                Vector3 c = new Vector3(x - .5f, -1, y - .5f);
                                Vector3 d = new Vector3(x + .5f, -1, y - .5f);
                                Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                                for (int k = 0; k < 6; k++)
                                {
                                    vertices.Add(v[k]);
                                    triangles.Add(triangles.Count);
                                }
                            }
                        }
                        if (y < gridSize - 1)
                        {
                            Node up = Grid.Instance.GetCell(x, y + 1);
                            if (up.IsWater)
                            {
                                Vector3 a = new Vector3(x + .5f, 0, y + .5f);
                                Vector3 b = new Vector3(x - .5f, 0, y + .5f);
                                Vector3 c = new Vector3(x + .5f, -1, y + .5f);
                                Vector3 d = new Vector3(x - .5f, -1, y + .5f);
                                Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                                for (int k = 0; k < 6; k++)
                                {
                                    vertices.Add(v[k]);
                                    triangles.Add(triangles.Count);
                                }
                            }
                        }
                    }
                }
            }
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();

            GameObject edgeObj = new GameObject("Edge");
            edgeObj.transform.SetParent(transform);

            MeshFilter meshFilter = edgeObj.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            MeshRenderer meshRenderer = edgeObj.AddComponent<MeshRenderer>();
            meshRenderer.material = edgeMaterial;
        }

        private void DrawTexture()
        {
            int nChunks = (int)Mathf.Sqrt(chunks.Count);
            foreach (GameObject chunk in chunks)
            {
                Texture2D texture = new Texture2D(gridSize, gridSize);
                Color[] colorMap = new Color[gridSize * gridSize];
                for (int y = 0; y < gridSize; y++)
                {
                    for (int x = 0; x < gridSize; x++)
                    {
                        Node cell = Grid.Instance.GetCell(x, y);
                        if (cell.IsWater)
                            colorMap[y * gridSize + x] = waterColor;
                        else
                        {
                            switch (cell.GrassFertilityLevel)
                            {
                                case Node.GrassFertility.Little:
                                    colorMap[y * gridSize + x] = grassColor;
                                    break;
                                case Node.GrassFertility.Medium:
                                    colorMap[y * gridSize + x] = fertilizedGrass1Color;
                                    break;
                                case Node.GrassFertility.High:
                                    colorMap[y * gridSize + x] = fertilizedGrass2Color;
                                    break;
                            }
                        }
                    }
                }

                texture.filterMode = FilterMode.Point;
                texture.SetPixels(colorMap);
                texture.Apply();

                MeshRenderer meshRenderer = chunk.GetComponent<MeshRenderer>();
                meshRenderer.material.mainTexture = texture;
                meshRenderer.material.SetFloat("_Smoothness", 0);
            }
        }

        public void TurnWaterBloody(Node node) ////////////////////////////////////
        {
            int nChunks = (int)Mathf.Sqrt(chunks.Count);
            foreach (GameObject chunk in chunks)
            {
                Texture2D texture = new Texture2D(gridSize, gridSize);
                Color[] colorMap = new Color[gridSize * gridSize];
                for (int y = 0; y < gridSize; y++)
                {
                    for (int x = 0; x < gridSize; x++)
                    {
                        if(Mathf.Abs(node.CellPosition.x - x) <= 1 && Mathf.Abs(node.CellPosition.y - y) <= 1)
                        {
                            Node cell = Grid.Instance.GetCell(x, y);
                            if (cell.IsWater)
                                colorMap[y * gridSize + x] = Color.red;
                            else
                                colorMap[y * gridSize + x] = grassColor;
                        }
                        else
                        {
                            Node cell = Grid.Instance.GetCell(x, y);
                            if (cell.IsWater)
                                colorMap[y * gridSize + x] = waterColor;
                            else
                            {
                                switch (cell.GrassFertilityLevel)
                                {
                                    case Node.GrassFertility.Little:
                                        colorMap[y * gridSize + x] = grassColor;
                                        break;
                                    case Node.GrassFertility.Medium:
                                        colorMap[y * gridSize + x] = fertilizedGrass1Color;
                                        break;
                                    case Node.GrassFertility.High:
                                        colorMap[y * gridSize + x] = fertilizedGrass2Color;
                                        break;
                                }
                            }
                        }
                    }
                }

                texture.filterMode = FilterMode.Point;
                texture.SetPixels(colorMap);
                texture.Apply();

                MeshRenderer meshRenderer = chunk.GetComponent<MeshRenderer>();
                meshRenderer.material.mainTexture = texture;
            }
        }


        private void GenerateTrees()
        {
            float[,] noiseMap = new float[gridSize, gridSize];
            (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    float noiseValue = Mathf.PerlinNoise(x * treeNoiseScale + xOffset, y * treeNoiseScale + yOffset);
                    noiseMap[x, y] = noiseValue;
                }
            }

            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    Node cell = Grid.Instance.GetCell(x, y);
                    if (!cell.IsWater)
                    {
                        float spawnProb = Random.Range(0f, treeDensity);
                        if (noiseMap[(int)x, (int)y] < spawnProb)
                        {
                            GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
                            GameObject tree = Instantiate(prefab, transform);
                            tree.transform.position = new Vector3(x * nodeSize, 0, y * nodeSize);
                            tree.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                            tree.transform.localScale = Vector3.one * Random.Range(0.8f, 1.2f) * nodeSize;
                            cell.TreeSpawned(tree);
                        }
                    }
                }
            }
        }

        private void GenerateMinerals()
        {
            // Stone Spawn
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    Node cell = Grid.Instance.GetCell(x, y);
                    if (!cell.IsWater && cell.IsFree())
                    {
                        float spawnProb = Random.Range(0.0000f, 1.0000f);
                        if (spawnProb < stoneSpawnProb)
                        {
                            GameObject prefab = stonePrefabs[Random.Range(0, stonePrefabs.Length)];
                            GameObject stone = Instantiate(prefab, transform);
                            stone.transform.position = new Vector3(x * nodeSize, 0, y * nodeSize);
                            stone.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                            stone.transform.localScale = Vector3.one * Random.Range(.8f, 1.2f) * nodeSize;
                            cell.StoneSpawned(stone);
                            Grid.Instance.SetNodeWalkability(x, y, false);
                        }
                    }
                }
            }

            // Iron Spawn
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    Node cell = Grid.Instance.GetCell(x, y);
                    if (!cell.IsWater && cell.IsFree())
                    {
                        float spawnProb = Random.Range(0.0000f, 1.0000f);
                        if (spawnProb < ironSpawnProb)
                        {
                            GameObject prefab = ironPrefabs[Random.Range(0, ironPrefabs.Length)];
                            GameObject iron = Instantiate(prefab, transform);
                            iron.transform.position = new Vector3(x * nodeSize, 0, y * nodeSize);
                            iron.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                            iron.transform.localScale = Vector3.one * Random.Range(.8f, 1.2f) * nodeSize;
                            cell.IronSpawned(iron);
                            Grid.Instance.SetNodeWalkability(x, y, false);
                        }
                    }
                }
            }

            // Gold Spawn
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    Node cell = Grid.Instance.GetCell(x, y);
                    if (!cell.IsWater && cell.IsFree())
                    {
                        float spawnProb = Random.Range(0.0000f, 1.0000f);
                        if (spawnProb < goldSpawnProb)
                        {
                            GameObject prefab = goldPrefabs[Random.Range(0, goldPrefabs.Length)];
                            GameObject gold = Instantiate(prefab, transform);
                            gold.transform.position = new Vector3(x * nodeSize, 0, y * nodeSize);
                            gold.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                            gold.transform.localScale = Vector3.one * Random.Range(.8f, 1.2f) * nodeSize;
                            cell.GoldSpawned(gold);
                            Grid.Instance.SetNodeWalkability(x, y, false);
                        }
                    }
                }
            }
        }

        /*
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Node cell = Grid.Instance.GetCell(x, y);
                    if (cell.IsWater)
                        Gizmos.color = Color.blue;
                    else
                        Gizmos.color = Color.green;
                    Vector3 pos = new Vector3(x, 0, y);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
        */

    }
}