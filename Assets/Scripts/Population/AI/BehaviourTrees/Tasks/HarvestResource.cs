using UnityEngine;
using ResourceTypes;
using Resources;

namespace Population
{
    public class HarvestResource : Task
    {
        private float timeSinceStart;
        Person person;
        Resource resource;
        MoveToPosition moveToPosition;
        public HarvestResource(Person person, Resource resource, MoveToPosition moveToPosition)
        {
            person.transform.LookAt(resource.transform);
            this.person = person;
            this.resource = resource;
            this.moveToPosition = moveToPosition;
            timeSinceStart = 0;
        }

        public override Result Run()
        {
            //PLAY ANIMATION
            if (timeSinceStart < 5 && resource != null)
            {
                person.transform.LookAt(resource.transform);
                timeSinceStart += Time.deltaTime;
                return Result.Running;
            }
            // //
            else
            {
                Resource nextResource = ResourceGatheringManager.Instance.GetNextResourceToHarvest();
                if(nextResource != null)
                {
                    float radianAngle = Random.Range(0f, 2 * Mathf.PI);
                    Vector3 gatherPosition = nextResource.transform.position + new Vector3(0.25f * Mathf.Cos(radianAngle), 0f, 0.25f * Mathf.Sin(radianAngle));

                    moveToPosition.UpdateTargetPosition(gatherPosition);
                }
                else
                {
                    person.NewIdleBT();
                }
                ResourceManager.Instance.AddResource(resource.GetType(), 2);

                resource.ChangeGatheredState(Resource.ResourceGatherStates.Gathered);
                timeSinceStart = 0;
                resource = nextResource;

                return Result.Success;
            }
        }
    }
}