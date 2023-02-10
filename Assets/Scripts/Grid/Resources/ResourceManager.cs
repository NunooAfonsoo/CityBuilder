using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResourceTypes;
using UnityEngine.UI;
using TMPro;
using Constants;

namespace Resources
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        private int wood;
        private int stone;
        private int iron;
        private int gold;

        public class ResourceTypeArg : EventArgs
        {
            public Type type;
            public bool increase;
            public int newAmount;

            public ResourceTypeArg(Type type, bool increase, int newAmount)
            {
                this.type = type;
                this.increase = increase;
                this.newAmount = newAmount;
            }
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Resource.OnResourceHarvested += Resource_OnResourceHarvested;
        }

        private void Resource_OnResourceHarvested(object sender, Resource.OnResourceHarvestedArgs e)
        {
            AddResource(sender.GetType(), e.harvestAmount);
        }

        public void AddResource(Type type, int quantity)
        {
            if (type == typeof(ResourceTypes.Tree))
            {
                wood += quantity;
            }
            else if (type == typeof(Stone))
            {
                stone += quantity;
            }
            else if (type == typeof(Iron))
            {
                iron += quantity;
            }
            else if (type == typeof(Gold))
            {
                gold += quantity;
            }
        }

        public void UseResource(Type[] type, int[] quantity)
        {
            for(int i = 0; i < type.Length; i++)
            {
                if (type[i] == typeof(ResourceTypes.Tree))
                {
                    wood -= quantity[i];
                }
                else if (type[i] == typeof(Stone))
                {
                    stone -= quantity[i];
                }
                else if (type[i] == typeof(Iron))
                {
                    iron -= quantity[i];
                }
                else if (type[i] == typeof(Gold))
                {
                    gold += quantity[i];
                }
            }
        }
    }
}
