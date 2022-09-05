using UnityEngine;
using System;
using ResourceTypes;

namespace Tools
{
    public class GatherResourcesTool : ITool
    {
        ResourceGatheringManager resourceGatheringManager;
        public GatherResourcesTool()
        {
            resourceGatheringManager = ResourceGatheringManager.Instance;
        }

        public void UseTool(GameObject resource, Type type)
        {
            if (resource != null)
            {
                if (resource.TryGetComponent(out ResourceTypes.Tree resourceTree) && type == typeof(ResourceTypes.Tree))
                {
                    if(!resourceGatheringManager.ResourcesToBeGathered.Contains(resourceTree))
                    {
                        resourceTree.ShowResourceMarker();
                    }


                    //resourceGatheringManager.RegisterTree(resourceTree);
                    resourceGatheringManager.RegisterResource(resourceTree);
                }
                else if (resource.TryGetComponent(out Stone resourceStone) && type == typeof(Stone))
                {
                    if (!resourceGatheringManager.ResourcesToBeGathered.Contains(resourceStone))
                    {
                        resourceStone.ShowResourceMarker();
                    }

                    //resourceGatheringManager.RegisterStone(resourceStone);
                    resourceGatheringManager.RegisterResource(resourceStone);
                }
            }
        }
    }
}
