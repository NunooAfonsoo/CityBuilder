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

        public void UseTool(GameObject resource, Type type, GameObject[] resourceMarker = null)
        {
            if (resource != null)
            {
                if (resource.TryGetComponent(out ResourceTypes.Tree resourceTree) && type == typeof(ResourceTypes.Tree))
                {
                    if(!resourceGatheringManager.TreesToBeGathered.Contains(resourceTree))
                    {
                        GameObject marker = GameObject.Instantiate(resourceMarker[0], resource.transform.position + new Vector3(0, 3f, 0), Quaternion.identity, resource.transform);
                    }
                    resourceGatheringManager.RegisterTree(resourceTree);
                }
                else if (resource.TryGetComponent(out Stone resourceStone) && type == typeof(Stone))
                {
                    if (!resourceGatheringManager.StonesToBeGathered.Contains(resourceStone))
                    {
                        GameObject marker = GameObject.Instantiate(resourceMarker[1], resource.transform.position + new Vector3(0, 1.2f, 0), Quaternion.identity, resource.transform);
                    }

                    resourceGatheringManager.RegisterStone(resourceStone);
                }
            }
        }
    }
}
