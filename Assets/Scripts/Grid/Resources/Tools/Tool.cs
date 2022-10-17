using UnityEngine;
using System;
using Grid;

namespace Tools
{
    public abstract class Tool
    {
        public virtual void UseDeleteTool(Node node) { }

        public virtual void UseResourceTool(GameObject gameObj, Type type, Action onNewResourceSelected) { }
    }
}