using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using ResourceTypes;

namespace Population
{
    public class GatherResource : Sequence
    {
        public GatherResource(Person person, AIDestinationSetter aiDestinationSetter, Vector3 position, AIPath aiPath, Resource resourceToGather)
        {
            MoveToPosition moveToPosition = new MoveToPosition(aiDestinationSetter, position, aiPath, person);

            if (resourceToGather.GetType() == typeof(Stone) && (person.CurrentState != Person.PersonStates.MiningStone))
            {
                person.ChangeState(Person.PersonStates.MiningStone);
            }
            else if (resourceToGather.GetType() == typeof(ResourceTypes.Tree) && (person.CurrentState != Person.PersonStates.ChoppingTree))
            {
                person.ChangeState(Person.PersonStates.ChoppingTree);
            }

            this.children = new List<Task>()
            {
                moveToPosition,
                new HarvestResource(person, resourceToGather, moveToPosition)
            };
        }
    }
}