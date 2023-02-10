using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Population
{
    public class Idle : Sequence
    {
        public Idle(Person person, AIDestinationSetter aiDestinationSetter, Vector3 position, AIPath aiPath)
        {
            MoveToPosition moveToPosition = new MoveToPosition(aiDestinationSetter, position, aiPath, person);

            person.ChangeState(Person.PersonStates.Idling);

            this.children = new List<Task>()
            {
                moveToPosition,
                new WaitIdling(person, moveToPosition)
            };
        }
    }
}
