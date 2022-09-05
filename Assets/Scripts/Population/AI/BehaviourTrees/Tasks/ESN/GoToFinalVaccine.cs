using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
namespace Assets.Scripts.BehaviourTrees
{

    public class GoToFinalVaccine : Task
    {
        Entity person;

        Lab lab;

        Transform labDoor;

        public GoToFinalVaccine(Entity person, Lab lab, Transform labDoor)
        {
            this.person = person;
            this.lab = lab;
            this.labDoor = labDoor;
        }

        public override Result Run()
        {
            if (lab == null || labDoor == null || !person.FinalVaccine || person.currentAction == Entity.CurrentAction.GettingCured || person.currentAction == Entity.CurrentAction.GoingToTheHospital)
                return Result.Failure;

            if (person.Arrived(labDoor.position))
            {
                if (person.GoInsideBuilding(lab))
                {
                    person.currentAction = Entity.CurrentAction.GoingToFinalVaccine;
                    int waitTime = Random.Range(5, 40);
                    person.InvokeRepeating("ResetMassVaccination", waitTime, 1f);
                    return Result.Success;
                }
                return Result.Failure;
            }
            else if (person.currentAction == Entity.CurrentAction.GoingToFinalVaccine && Vector3.Distance(person.navMeshAgent.destination, labDoor.position) <= 0.5f)
            {
                return Result.Success;
            }

            else if (person.FinalVaccine)
            {
                if (person.navMeshAgent.destination != labDoor.position) person.MoveTo(labDoor.position);
                person.currentAction = Entity.CurrentAction.GoingToFinalVaccine;
                return Result.Success;
            }
            else return Result.Failure;
        }

    }
}
*/
