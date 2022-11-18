using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Population
{
    public class WaitIdling : Task
    {
        private float timeToWait;
        private float timeSinceStart;
        private Person person;
        MoveToPosition moveToPosition;

        public WaitIdling(Person person, MoveToPosition moveToPosition)
        {
            this.person = person;
            this.moveToPosition = moveToPosition;
            Initialize();
        }

        public override Result Run()
        {
            if(timeSinceStart < timeToWait)
            {
                timeSinceStart += Time.deltaTime;
                if (person.PersonState != Person.PersonStates.Idling)
                {
                    person.ChangePersonState(Person.PersonStates.Idling);
                    person.SetAnimation(Person.PersonStates.Idling);
                }
                return Result.Running;
            }
            else
            {
                Initialize();
                moveToPosition.UpdateTargetPosition(person.CalculateNewIdleTargetPosition());
                return Result.Success;
            }
        }

        private void Initialize()
        {
            timeSinceStart = 0;
            timeToWait = Random.Range(2.0f, 5.0f);
        }
    }
}
