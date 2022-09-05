using System.Collections.Generic;
using UnityEngine;

/*
namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks
{
    public class MoveToShout : Task
    {
        public GameObject Player { get; private set; }
        public NPC Character { get; private set; }

        public OrcShout OrcShout { get; private set; }

        public float Range { get; private set; }
        public MoveToShout(NPC character, GameObject player, OrcShout orcShout, float range)
        {
            Player = player;
            Character = character;
            OrcShout = orcShout;
            Range = range;
        }

        public override Result Run()
        {
            if (OrcShout.GameObject == null || Vector3.Distance(Player.transform.position, Character.transform.position) <= Character.awakeDistance)
                return Result.Failure;

            if (Vector3.Distance(Character.transform.position, OrcShout.GameObject.transform.position) <= Range)
            {
                return Result.Success;
            }
            else
            {
                Character.MoveTo(OrcShout.GameObject.transform.position);
                return Result.Running;
            }
        }
    }
}
*/