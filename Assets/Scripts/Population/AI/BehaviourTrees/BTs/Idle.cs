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
            PopulationManager populationManager = PopulationManager.Instance;
            populationManager.RemovePerson(person);

            person.ChangePersonState(Person.PersonStates.Idling);

            populationManager.RegisterPerson(person);

            MoveToPosition moveToPosition = new MoveToPosition(aiDestinationSetter, position, aiPath);
            this.children = new List<Task>()
            {
                moveToPosition,
                new WaitIdling(person, moveToPosition)
            };
        }
    }
}
