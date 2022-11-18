using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.InputSystem;
using ResourceTypes;

namespace Population
{
    [RequireComponent(typeof(AnimatorController))]
    public class Person : MonoBehaviour
    {
        [Header("PATHFOLLOWING")]
        private AIDestinationSetter aiDestinationSetter;
        private AIPath aiPath;
        Seeker seeker;
        Vector3 wanderStartPoint;

        public enum PersonStates
        {
            Moving,
            Idling,
            ChoppingTree,
            MiningStone,
            Working
        }

        [Header("Behaviour Tree")]
        private Task currentBehaviourTree;
        public PersonStates PersonState { get; private set; }

        private AnimatorController animator;
        void Start()
        {
            aiDestinationSetter = GetComponent<AIDestinationSetter>();
            aiPath = GetComponent<AIPath>();
            seeker = GetComponent<Seeker>();
            wanderStartPoint = transform.position;

            currentBehaviourTree = new Idle(this, aiDestinationSetter, CalculateNewIdleTargetPosition(), aiPath);
            PopulationManager.Instance.RegisterPerson(this);

            animator = GetComponent<AnimatorController>();
        }

        private void FixedUpdate()
        {
            currentBehaviourTree?.Run();
        }

        public void ChangePersonState(PersonStates state)
        {
            PersonState = state;
        }


        #region Behaviour Trees
        public void NewIdleBT()
        {
            currentBehaviourTree = new Idle(this, aiDestinationSetter, CalculateNewIdleTargetPosition(), aiPath);
        }

        public Vector3 CalculateNewIdleTargetPosition()
        {
            return new Vector3(wanderStartPoint.x + Random.Range(-3.0f, 3.0f), transform.position.y, wanderStartPoint.z + Random.Range(-3.0f, 3.0f));
        }

        public void NewGatherResourceBT(Resource resource)
        {
            Debug.Log("NewGatherResourceBT");
            float radianAngle = Random.Range(0f, 2 * Mathf.PI);
            Vector3 gatherPosition = resource.transform.position + new Vector3(0.25f * Mathf.Cos(radianAngle), 0f, 0.25f * Mathf.Sin(radianAngle));
            currentBehaviourTree = new GatherResource(this, aiDestinationSetter, gatherPosition, aiPath, resource);
        }
        #endregion

        public void SetAnimation(PersonStates state)
        {
            animator.SetAnimation(state);
        }
    }
}
