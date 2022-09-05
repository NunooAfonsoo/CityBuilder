using System.Collections;
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
            PopulationManager populationManager = PopulationManager.Instance;
            populationManager.RemovePerson(person);

            if (resourceToGather.GetType() == typeof(ResourceTypes.Tree))
            {
                person.ChangePersonState(Person.PersonStates.ChoppingTree);
            }
            else if (resourceToGather.GetType() == typeof(Stone))
            {
                person.ChangePersonState(Person.PersonStates.MiningStone);
            }

            populationManager.RegisterPerson(person);

            MoveToPosition moveToPosition = new MoveToPosition(aiDestinationSetter, position, aiPath);
            this.children = new List<Task>()
            {
                moveToPosition,
                new HarvestResource(person, resourceToGather, moveToPosition)
            };
        }

    }
}