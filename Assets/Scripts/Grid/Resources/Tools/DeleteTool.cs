using UnityEngine;
using System;

namespace Tools
{
    public class DeleteTool : ITool
    {
        public DeleteTool() { }

        public void UseTool(GameObject building, Type type = null, GameObject[] resourceMarker = null)
        {
            if (building != null) UnityEngine.Object.Destroy(building);
        }
    }
}
