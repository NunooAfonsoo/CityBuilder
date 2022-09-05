using UnityEngine;

namespace ResourceTypes
{
    public class ResourceMarkerAnimation : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float floatingSpeed;
        private float maxHeight;
        private float minHeight;

        private bool goingUp;
        private void Start()
        {
            goingUp = true;
            maxHeight = transform.position.y + 0.3f;
            minHeight = transform.position.y - 0.3f;
            Vector3 scale = new Vector3(transform.localScale.x / transform.lossyScale.x, transform.localScale.y / transform.lossyScale.y, transform.localScale.z / transform.lossyScale.z);
            transform.localScale = scale;
        }

        void Update()
        {
            if(transform.position.y > maxHeight)
            {
                goingUp = false;
            }
            else if(transform.position.y < minHeight)
            {
                goingUp = true;
            }
            MoveGO();
            RotateGO();
        }

        private void RotateGO()
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
        }

        private void MoveGO()
        {
            if (goingUp)
            {
                transform.position += Vector3.up * floatingSpeed * Time.deltaTime;
            }
            else
            {
                transform.position += Vector3.up * -floatingSpeed * Time.deltaTime;
            }
        }
    }
}