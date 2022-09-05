using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Population
{
    public class MoveToPosition : Task
    {
        AIPath aiPath;
        AIDestinationSetter aiDestinationSetter;

        public MoveToPosition(AIDestinationSetter aiDestinationSetter, Vector3 position, AIPath aiPath)
        {
            this.aiDestinationSetter = aiDestinationSetter;
            UpdateTargetPosition(position);
            this.aiPath = aiPath;
        }

        public override Result Run()
        {
            if (!aiPath.reachedEndOfPath && !aiPath.reachedDestination)
            {
                return Result.Running;
            }
            else
            {
                return Result.Success;
            }
        }

        public void UpdateTargetPosition(Vector3 position)
        {
            aiDestinationSetter.UpdateTargetPosition(position);
        }
    }
}
