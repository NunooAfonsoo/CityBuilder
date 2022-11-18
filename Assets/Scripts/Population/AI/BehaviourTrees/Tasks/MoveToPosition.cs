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
        Person person;
        public MoveToPosition(AIDestinationSetter aiDestinationSetter, Vector3 position, AIPath aiPath, Person person)
        {
            this.person = person;
            this.aiDestinationSetter = aiDestinationSetter;
            UpdateTargetPosition(position);
            this.aiPath = aiPath;
        }

        public override Result Run()
        {
            if (!aiPath.reachedEndOfPath && !aiPath.reachedDestination)
            {
                if (person.PersonState != Person.PersonStates.Moving)
                {
                    person.ChangePersonState(Person.PersonStates.Moving);
                    person.SetAnimation(Person.PersonStates.Moving);
                }
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
