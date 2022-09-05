using UnityEngine;
using System;

namespace Tools
{
    public interface ITool
    {
        void UseTool(GameObject gameObj, Type type);
    }
}