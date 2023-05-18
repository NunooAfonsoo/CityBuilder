using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

namespace Population
{
    public class PersonAnimator : MonoBehaviour
    {
        private Animator animator;

        [SerializeField] private Person person;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            person.OnMoving += Person_OnMoving;
            person.OnAnimationChanged += Person_OnAnimationChanged;
        }

        private void Person_OnAnimationChanged(object sender, Person.AnimationChangedArgs e)
        {
            ChangeAnimation(e.state);
        }

        private void ChangeAnimation(Person.PersonStates state)
        {
            switch (state)
            {
                case Person.PersonStates.Idling:
                    animator.SetTrigger(AnimationStrings.IDLING);
                    break;
                case Person.PersonStates.ChoppingTree:
                case Person.PersonStates.MiningStone:
                    animator.SetTrigger(AnimationStrings.HARVESTING_RESOURCE);
                    break;
            }
        }

        private void Person_OnMoving(object sender, EventArgs e)
        {
            animator.SetTrigger(AnimationStrings.MOVING);
        }
    }
}
