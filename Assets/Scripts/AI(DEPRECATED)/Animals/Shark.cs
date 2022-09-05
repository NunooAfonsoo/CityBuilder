using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Population;

namespace Animals
{
    public class Shark : MonoBehaviour
    {
        /*
        const float MinPathUpdateTime = .2f;
        const float PathUpdateMoveThreshold = .5f;

        public Vector3 target;
        public float Speed = 2;
        public float TurnSpeed = 3;
        public float TurnDst = 5;
        public float StoppingDst = 0;
        Grid.Path path;

        [SerializeField] private Grid.ProceduralTerrain proceduralTerrain;

        private float attackYPos;
        private float moveYPos;
        float attackRotation;
        bool canMove;

        private void Awake()
        {
            attackYPos = 0.032f;
            moveYPos = -0.17f;
            attackRotation = -15;
            transform.position = new Vector3(transform.position.x, moveYPos, transform.position.z);
            canMove = true;
        }

        private void Update()
        {
            Vector3Int position = Grid.Grid.Instance.GetGridPositionFromWorldPosition(transform.position);
            Grid.Node node = Grid.Grid.Instance.GetCell(position.x, position.z);
            if(node.IsWater && canMove)
            {
                Move();
                
                //if(Vector3.Distance(Person.Instance.transform.position, transform.position) < 2)
                //{
                //    Attack();
                //}
                
            }
        }
        public void Move()
        {
            //target = Grid.Grid.Instance.GetGridPositionFromWorldPosition(Person.Instance.transform.position);
            transform.position = Vector3.MoveTowards(transform.position, target, Speed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, moveYPos, transform.position.z);


            //transform.LookAt(Person.Instance.transform);
        }

        public void Attack()
        {
            canMove = false;
            transform.position = new Vector3(transform.position.x, attackYPos, transform.position.z);

            transform.eulerAngles = new Vector3(attackRotation, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

            StartCoroutine(Kill());
        }

        private IEnumerator Kill()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            Vector3Int pos = Grid.Grid.Instance.GetGridPositionFromWorldPosition(transform.position);
            proceduralTerrain.TurnWaterBloody(Grid.Grid.Instance.GetCell(pos.x, pos.z));
            //Destroy(Person.Instance.gameObject, 0.1f);
            canMove = true;
        }


        public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                path = new Grid.Path(waypoints, transform.position, TurnDst, StoppingDst);

                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
            }
        }*/
    }
}
