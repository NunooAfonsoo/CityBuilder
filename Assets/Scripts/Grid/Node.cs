using UnityEngine;

namespace Grid
{
    public class Node
    {
        public Node ParentNode { get; private set; }
        public int GCost;
        public int HCost;
        public int MovementPenalty { get; private set; }

        public enum GrassFertility
        {
            Water,
            Little,
            Medium,
            High,
        }

        public GrassFertility GrassFertilityLevel { get; private set; }

        public Vector2Int CellPosition { get; private set; }

        public bool IsWater { get; private set; }
        public bool HasTree { get; private set; }
        public bool HasStone { get; private set; }
        public bool HasIron { get; private set; }
        public bool HasGold { get; private set; }

        public bool HasBuilding
        {
            get
            {
                return Building != null;
            }
            private set { }
        }
        public bool HasRoad
        {
            get
            {
                return Road != null;
            }
            private set { }
        }

        public GameObject Resource { get; private set; }
        public GameObject Building { get; private set; }
        public GameObject Road { get; private set; }

        public Node(int cellpositionX, int cellpositionY, bool isWater)
        {
            CellPosition = new Vector2Int(cellpositionX, cellpositionY);
            this.IsWater = isWater;

            if (IsWater) GrassFertilityLevel = GrassFertility.Water;
            else GrassFertilityLevel = GrassFertility.Little;

             Resource = null;
             Building = null;
             Road = null;
        }


        #region ASTAR
        public int CompareTo(Node nodeToCompare)
        {
            int compare = FCost().CompareTo(nodeToCompare.FCost());
            if (compare == 0)
            {
                compare = HCost.CompareTo(nodeToCompare.HCost);
            }
            return -compare;
        }

        public void SetMovementPenalty(int penalty) => MovementPenalty = penalty;

        public int FCost() => GCost + HCost;

        public void SetParentNode(Node node) => ParentNode = node;
        #endregion

        internal void SetFertilityLevel(GrassFertility fertilityLevel) => GrassFertilityLevel = fertilityLevel;

        public void SetWater(bool isWater) => IsWater = isWater;

        public void TreeSpawned(GameObject tree)
        {
            HasTree = true;
            this.Resource = tree;
        }

        public void StoneSpawned(GameObject stone)
        {
            HasStone = true;
            this.Resource = stone;
        }

        public void IronSpawned(GameObject iron)
        {
            HasIron = true;
            this.Resource = iron;
        }

        public void GoldSpawned(GameObject gold)
        {
            HasGold = true;
            this.Resource = gold;
        }

        public void BuildingCreated(GameObject building)
        {
            this.Building = building;
            Grid.Instance.SetNodeWalkability(CellPosition.x, CellPosition.y, false);
        }

        public void ClearTree()
        {
            Object.Destroy(Resource);
            Resource = null;
        }

        public void ClearStone()
        {
            Object.Destroy(Resource);
            HasStone = false;
            Resource = null;
        }

        public void ClearIron()
        {
            Object.Destroy(Resource);
            HasIron = true;
            Resource = null;
        }

        public void ClearGold()
        {
            Object.Destroy(Resource);
            HasGold = true;
            Resource = null;
        }

        public void ClearBuilding()
        {
            Object.Destroy(Building);
            Building = null;
        }
        public void RoadCreated(GameObject road)
        {
            this.Road = road;
        }

        public bool IsFree() => !HasTree && !HasStone && !HasIron && !HasGold && !HasBuilding;

        public bool IsWalkable() => !IsWater && !HasStone && !HasIron && !HasGold && !HasBuilding;

        public bool CanBuildAtPosition() => !IsWater && !HasTree && !HasStone && !HasIron && !HasGold && !HasBuilding;
        public bool CanBuildAtWaterPosition() => IsWater && !HasBuilding;

    }
}