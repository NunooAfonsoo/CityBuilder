using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ResourceTypes
{
    public class ResourceGatheringManager
    {
        private static ResourceGatheringManager instance;
        public static ResourceGatheringManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ResourceGatheringManager();
                return instance;
            }
        }

        public Queue<Resource> ResourcesToBeGathered { get; private set; }

        private ResourceGatheringManager()
        {
            ResourcesToBeGathered = new Queue<Resource>();
        }

        public void RegisterResource(Resource resource)
        {
            ResourcesToBeGathered.Enqueue(resource);
        }

        public void RemoveResource(Resource resource)
        {
            ResourcesToBeGathered = new Queue<Resource>(ResourcesToBeGathered.Where(x => x != resource));
        }

        private void ClearResourcesToBeGathered()
        {
            ResourcesToBeGathered.Clear();
        }

        public void ClearResourcesQueue()
        {
            ClearResourcesToBeGathered();
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