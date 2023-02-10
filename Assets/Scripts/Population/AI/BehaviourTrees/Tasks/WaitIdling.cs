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
        private MoveToPosition moveToPosition;
        private bool firstRun;

        public WaitIdling(Person person, MoveToPosition moveToPosition)
        {
            this.person = person;
            this.moveToPosition = moveToPosition;
            Initialize();
            firstRun = true;
        }

        public override Result Run()
        {
            if (timeSinceStart < timeToWait)
            {
                if(firstRun)
                {
                    person.ChangeAnimation(Person.PersonStates.Idling);
                    firstRun = false;
                }

                timeSinceStart += Time.deltaTime;
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
            firstRun = true;
            timeSinceStart = 0;
            timeToWait = Random.Range(2.0f, 5.0f);
        }
    }
}
