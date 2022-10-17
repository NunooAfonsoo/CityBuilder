using UnityEngine;
using System;
using ResourceTypes;

namespace Tools
{
    public class GatherResourcesTool : Tool
    {
        private ResourceGatheringManager resourceGatheringManager;

        public GatherResourcesTool()
        {
            resourceGatheringManager = ResourceGatheringManager.Instance;
        }

        public override void UseResourceTool(GameObject resourceObj, Type type, Action onNewResourceSelected)
        {
            if (resourceObj != null)
            {
                resourceObj.TryGetComponent(out Resource resource);

                if (resource.GetType() == type && ResourceCanBeGathered(resource))
                {
                    resourceGatheringManager.RegisterResource(resource);
                    resource.ShowResourceMarker();
                    onNewResourceSelected?.Invoke();
                }
            }
        }

        private bool ResourceCanBeGathered(Resource resource)
        {
            return resource.ResourceGatherState == Resource.ResourceGatherStates.NotGathered && !resourceGatheringManager.ResourcesToBeGathered.Contains(resource);
        }
    }
}
