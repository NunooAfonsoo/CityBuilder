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
        public int ResourceQuantity = 0;
        [SerializeField] private GameObject resourceMarker;
        private void Start()
        {
            ChangeGatheredState(ResourceGatherStates.NotGathered);
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
            // GIVE RESOURCES 
            // PLAY ANIMATION?
            if (this.gameObject)
            {
                Destroy(this.gameObject);
            }

            Grid.Grid grid = Grid.Grid.Instance;
            Vector3Int position = grid.GetGridPositionFromWorldPosition(transform.position);

            if(GetType() == typeof(ResourceTypes.Tree))
            {
           
            }
            else if (GetType() == typeof(Stone))
            {
                grid.SetNodeWalkability(position.x, position.z, true);
            }
        }

        public void ShowResourceMarker()
        {
            resourceMarker.SetActive(true);
        }
    }
}