using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Population
{
    public class MoveToPosition : Task
    {
        private AIPath aiPath;
        private AIDestinationSetter aiDestinationSetter;
        private Person person;
        private bool firstRun;
        public MoveToPosition(AIDestinationSetter aiDestinationSetter, Vector3 position, AIPath aiPath, Person person)
        {
            this.person = person;
            this.aiDestinationSetter = aiDestinationSetter;
            UpdateTargetPosition(position);
            this.aiPath = aiPath;
            firstRun = true;
        }

        public override Result Run()
        {
            if (!aiPath.reachedDestination)
            {
                if (firstRun)
                {
                    person.PlayMovingAnimation();
                    firstRun = false;
                }
                
                return Result.Running;
            }
            else
            {
                firstRun = true;
                return Result.Success;
            }
        }

        public void UpdateTargetPosition(Vector3 position)
        {
            aiDestinationSetter.UpdateTargetPosition(position);
        }
    }
}
