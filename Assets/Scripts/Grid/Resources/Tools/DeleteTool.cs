using UnityEngine;
using System;
using Grid;

namespace Tools
{
    public class DeleteTool : Tool
    {
        public DeleteTool() { }

        public override void UseDeleteTool(Node node)
        {
            node?.ClearBuilding();
        }
    }
}
