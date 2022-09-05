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

        public HashSet<Tree> TreesToBeGathered { get; private set; }
        public HashSet<Stone> StonesToBeGathered { get; private set; }
        public int ResourceGettingHarvested { get; private set; }//0 for trees, 1 for stone

        private ResourceGatheringManager()
        {
            TreesToBeGathered = new HashSet<Tree>();
            StonesToBeGathered = new HashSet<Stone>();
        }

        public void RegisterTree(Tree tree)
        {
            TreesToBeGathered.Add(tree);
        }

        public void RegisterStone(Stone stone)
        {
            StonesToBeGathered.Add(stone);
        }

        public void RemoveTree(Tree tree)
        {
            TreesToBeGathered.Remove(tree);
        }

        public void RemoveStone(Stone stone)
        {
            StonesToBeGathered.Remove(stone);
        }

        public void ClearTreesToBeGathered()
        {
            TreesToBeGathered.Clear();
        }

        public void ClearStonesToBeGathered()
        {
            StonesToBeGathered.Clear();
        }
        public void ClearAllHashSets()
        {
            ClearTreesToBeGathered();
            ClearStonesToBeGathered();
        }

        public void SetHarvestedResource(Type type)
        {
            if(type == typeof(Tree))
            {
                ResourceGettingHarvested = 0;
            }
            else if(type == typeof(Stone))
            {
                ResourceGettingHarvested = 1;
            }
        }

        public bool IsResourceBeingGathered(Resource resource)
        {
            switch(ResourceGettingHarvested)
            {
                case 0:
                    if(resource.GetType() == typeof(Tree))
                    {
                        return true;
                    }
                    return false;
                case 1:
                    if (resource.GetType() == typeof(Stone))
                    {
                        return true;
                    }
                    return false;
            }
            return false;
        }

        public Resource ChooseRadomResource(Type type)
        {
            if(type == typeof(Tree))
            {
                if(TreesToBeGathered.Count > 0)
                {
                    foreach(Tree treeToBeGathered in TreesToBeGathered)
                    {
                        if(treeToBeGathered.ResourceGatherState == Resource.ResourceGatherStates.NotGathered)
                        {
                            treeToBeGathered.ChangeGatheredState(Resource.ResourceGatherStates.BeingGathered);
                            RemoveTree(treeToBeGathered);
                            SetHarvestedResource(typeof(Tree));
                            return treeToBeGathered;
                        }
                    }
                }
            }
            else if (type == typeof(Stone))
            {
                if (StonesToBeGathered.Count > 0)
                {
                    foreach (Stone stoneToBeGathered in StonesToBeGathered)
                    {
                        if (stoneToBeGathered.ResourceGatherState == Resource.ResourceGatherStates.NotGathered)
                        {
                            stoneToBeGathered.ChangeGatheredState(Resource.ResourceGatherStates.BeingGathered);
                            RemoveStone(stoneToBeGathered);
                            SetHarvestedResource(typeof(Stone));
                            return stoneToBeGathered;
                        }
                    }
                }
            }
            return null;
        }
    }
}