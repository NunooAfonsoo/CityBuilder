using UnityEngine;
using System;
using Grid;

namespace Tools
{
    public class Tool
    {
        public virtual void UseDeleteTool(Node node) 
        {
            Debug.LogError("UseDeleteTool not implemented!");
        }

        public virtual void UseResourceTool(GameObject gameObj, Type type, Action onNewResourceSelected) 
        {
            Debug.LogError("UseResourceTool not implemented!");
        }
    }
}