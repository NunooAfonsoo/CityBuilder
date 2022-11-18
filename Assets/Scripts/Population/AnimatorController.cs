using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Population
{
    public class AnimatorController : MonoBehaviour
    {
        private Animator animator;

        private void Awake()
        {
            animator = transform.GetChild(0).GetComponent<Animator>();
        }

        public void SetAnimation(Person.PersonStates state)
        {
            switch(state)
            {
                case Person.PersonStates.Idling:
                case Person.PersonStates.Moving:
                    animator.SetTrigger("Moving");
                    break;
                case Person.PersonStates.ChoppingTree:
                case Person.PersonStates.MiningStone:
                    animator.SetTrigger("HarvestingResource");
                    break;
            }
        }
    }
}
