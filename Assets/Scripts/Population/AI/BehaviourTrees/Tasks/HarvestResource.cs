using UnityEngine;
using ResourceTypes;
using Resources;

namespace Population
{
    public class HarvestResource : Task
    {
        private float timeSinceStart;
        private Person person;
        private Resource resource;
        private MoveToPosition moveToPosition;
        private bool firstRun;
        public HarvestResource(Person person, Resource resource, MoveToPosition moveToPosition)
        {

            person.transform.LookAt(resource.transform);
            this.person = person;
            this.resource = resource;
            this.moveToPosition = moveToPosition;
            timeSinceStart = 0;
            firstRun = true;
        }

        public override Result Run()
        {
            if (timeSinceStart < 5 && resource != null)
            {
                person.transform.LookAt(resource.transform);
                timeSinceStart += Time.deltaTime;
                if(firstRun)
                {
                    firstRun = false;

                    if(resource.GetType() == typeof(Stone))
                    {
                        person.ChangeAnimation(Person.PersonStates.MiningStone);
                    }
                    else if (resource.GetType() == typeof(ResourceTypes.Tree))
                    {
                        person.ChangeAnimation(Person.PersonStates.ChoppingTree);
                    }
                }
                return Result.Running;
            }
            else
            {
                resource.ChangeGatheredState(Resource.ResourceGatherStates.Gathered);

                resource = ResourceGatheringManager.Instance.GetNextResourceToHarvest();

                if (resource != null)
                {
                    float radianAngle = Random.Range(0f, 2 * Mathf.PI);
                    Vector3 gatherPosition = resource.transform.position + new Vector3(0.25f * Mathf.Cos(radianAngle), 0f, 0.25f * Mathf.Sin(radianAngle));

                    moveToPosition.UpdateTargetPosition(gatherPosition);
                }
                else
                {
                    person.NewIdleBT();
                }

                timeSinceStart = 0;

                firstRun = true;
                return Result.Success;
            }
        }
    }
}