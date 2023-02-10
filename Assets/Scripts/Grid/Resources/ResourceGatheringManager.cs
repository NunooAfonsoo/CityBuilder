using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ResourceTypes
{
    public class ResourceGatheringManager : MonoBehaviour
    {
        public static ResourceGatheringManager Instance { get; private set; }

        public Queue<Resource> ResourcesToBeGathered { get; private set; }

        private void Awake()
        {
            Instance = this;

            ResourcesToBeGathered = new Queue<Resource>();
        }

        public void RegisterResource(Resource resource)
        {
            ResourcesToBeGathered.Enqueue(resource);
        }

        public void ClearResourcesQueue()
        {
            ClearResourcesToBeGathered();
        }

        private void ClearResourcesToBeGathered()
        {
            ResourcesToBeGathered.Clear();
        }

        public Resource GetNextResourceToHarvest()
        {
            if (ResourcesToBeGathered.Count > 0)
            {
                Resource resource = ResourcesToBeGathered.Dequeue();
                resource.ChangeGatheredState(Resource.ResourceGatherStates.BeingGathered);
                return resource;
            }
            return null;
        }
    }
}