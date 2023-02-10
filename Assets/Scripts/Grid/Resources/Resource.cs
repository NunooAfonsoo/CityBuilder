using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grid;
using Resources;
using System;

namespace ResourceTypes
{
    public abstract class Resource : MonoBehaviour
    {
        public enum ResourceGatherStates
        {
            NotGathered,
            BeingGathered,
            Gathered
        }

        public ResourceGatherStates ResourceGatherState { get; private set; }

        [SerializeField] private GameObject resourceMarker;
        private int harvestAmount;

        public static event EventHandler<OnResourceHarvestedArgs> OnResourceHarvested;
        public class OnResourceHarvestedArgs : EventArgs
        {
            public int harvestAmount;
            public OnResourceHarvestedArgs(int harvestAmount)
            {
                this.harvestAmount = harvestAmount;
            }
        }
        private void Awake()
        {
            ChangeGatheredState(ResourceGatherStates.NotGathered);
            harvestAmount = 2;
        }

        public void ChangeGatheredState(ResourceGatherStates state)
        {
            ResourceGatherState = state;
            if(ResourceGatherState == ResourceGatherStates.Gathered)
            {
                ResourceHarvested();
            }
        }

        private void ResourceHarvested()
        {
            OnResourceHarvested?.Invoke(this, new OnResourceHarvestedArgs(harvestAmount));
            // GIVE RESOURCES 
            // PLAY ANIMATION?

            Grid.Grid grid = Grid.Grid.Instance;
            Vector3Int position = grid.GetGridPositionFromWorldPosition(transform.position);

            if(GetType() == typeof(Tree))
            {
           
            }
            else if (GetType() == typeof(Stone))
            {
                grid.SetNodeWalkability(position.x, position.z, true);
            }

            if (this.gameObject)
            {
                Destroy(this.gameObject);
            }
        }

        public void ShowResourceMarker()
        {
            resourceMarker.SetActive(true);
        }
    }
}