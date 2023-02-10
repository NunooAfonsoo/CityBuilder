using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.InputSystem;
using ResourceTypes;
using System;

namespace Population
{
    public class Person : MonoBehaviour
    {
        [Header("PATHFOLLOWING")]
        private AIDestinationSetter aiDestinationSetter;
        public  AIPath aiPath;
        Vector3 wanderStartPoint;

        public enum PersonStates
        {
            Idling,
            ChoppingTree,
            MiningStone,
            Working
        }

        [Header("Behaviour Tree")]
        private Task currentBehaviourTree;
        public PersonStates CurrentState { get; private set; }
        public event EventHandler<StateChangedArgs> OnStateChanged;
        public class StateChangedArgs : EventArgs
        {
            public PersonStates oldState;
            public PersonStates newState;

            public StateChangedArgs(PersonStates oldState, PersonStates newState)
            {
                this.oldState = oldState;
                this.newState = newState;
            }
        }


        public event EventHandler OnMoving;
        public event EventHandler<AnimationChangedArgs> OnAnimationChanged;
        public class AnimationChangedArgs : EventArgs
        {
            public PersonStates state;

            public AnimationChangedArgs(PersonStates state)
            {
                this.state = state;
            }
        }

        private void Awake()
        {
            aiDestinationSetter = GetComponent<AIDestinationSetter>();
            aiPath = GetComponent<AIPath>();
            wanderStartPoint = transform.position;
        }

        void Start()
        {
            currentBehaviourTree = new Idle(this, aiDestinationSetter, CalculateNewIdleTargetPosition(), aiPath);

            PopulationManager.Instance.PersonSpawned(this);
        }

        private void FixedUpdate()
        {
            currentBehaviourTree?.Run();
        }

        public void PlayMovingAnimation()
        {
            OnMoving?.Invoke(this, EventArgs.Empty);
        }

        public void ChangeState(PersonStates state)
        {
            OnStateChanged?.Invoke(this, new StateChangedArgs(CurrentState, state));
            CurrentState = state;
        }

        public void ChangeAnimation(PersonStates state)
        {
            OnAnimationChanged?.Invoke(this, new AnimationChangedArgs(state));
        }

        #region Behaviour Trees
        public void NewIdleBT()
        {
            currentBehaviourTree = null;
            currentBehaviourTree = new Idle(this, aiDestinationSetter, CalculateNewIdleTargetPosition(), aiPath);
        }

        public Vector3 CalculateNewIdleTargetPosition()
        {
            return new Vector3(wanderStartPoint.x + UnityEngine.Random.Range(-3.0f, 3.0f), transform.position.y, wanderStartPoint.z + UnityEngine.Random.Range(-3.0f, 3.0f));
        }

        public void NewGatherResourceBT(Resource resource)
        {
            currentBehaviourTree = null;
            float radianAngle = UnityEngine.Random.Range(0f, 2 * Mathf.PI);
            Vector3 gatherPosition = resource.transform.position + new Vector3(0.25f * Mathf.Cos(radianAngle), 0f, 0.25f * Mathf.Sin(radianAngle));
            currentBehaviourTree = new GatherResource(this, aiDestinationSetter, gatherPosition, aiPath, resource);
        }
        #endregion

    }
}
