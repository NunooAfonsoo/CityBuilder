using UnityEngine;
using Constants;

namespace CameraNS
{
    public class CameraController : MonoBehaviour
    {
        private Transform cameraTransform;

        //HORIZONTAL
        [SerializeField] private AnimationCurve movementSpeedCurve;
        private float speed;
        [SerializeField] private float accelaration = 10f;

        //VERTICAL
        [SerializeField] private float minCamDistance = 5f;
        [SerializeField] private float maxCamDistance = 50f;
        [SerializeField] private AnimationCurve zoomSpeedCurve;

        //ROTATION
        [SerializeField] private AnimationCurve rotationSpeedCurve;


        //SCREEN EDGE MOTION
        [SerializeField] private bool showCamFocus;

        //used to update the position of the cam base obj
        private Vector3 targetPosition;

        private Vector3 horizontalVelocity;
        private Vector3 lastPosition;

        //where dragging started
        Vector3 startDrag;
        private float distanceToCamera;

        private void Awake()
        {
            cameraTransform = GetComponentInChildren<Camera>().transform;

            //SHOW CAM FOCUS POSITION
            GetComponent<MeshRenderer>().enabled = showCamFocus;

        }

        private void OnEnable()
        {
            InputManager.Instance.OnCameraRotate += InputManager_RotateCamera;
            InputManager.Instance.OnCameraZoom += InputManager_ZoomCamera;

            cameraTransform.LookAt(transform);
            lastPosition = transform.position;
        }

        private void OnDisable()
        {
            InputManager.Instance.OnCameraRotate -= InputManager_RotateCamera;
            InputManager.Instance.OnCameraZoom -= InputManager_ZoomCamera;
        }

        private void LateUpdate()
        {
            distanceToCamera = Vector3.Distance(cameraTransform.position, transform.position);
            GetKeyboardMovement();
            UpdateVelocity();
            UpdateBasePosition();
        }

        private void UpdateVelocity()
        {
            horizontalVelocity = (transform.position - lastPosition) / Time.deltaTime;
            horizontalVelocity.y = 0;
            lastPosition = transform.position;
        }

        private void GetKeyboardMovement()
        {
            Vector2 moveAmount = InputManager.Instance.GetCameraMovementAmount();
            Vector3 inputValue = moveAmount.x * GetCameraRight() + moveAmount.y * GetCameraForward();
            inputValue.Normalize();

            if (inputValue.sqrMagnitude > 0.1f)
            {
                targetPosition += inputValue;
            }
        }

        private Vector3 GetCameraRight()
        {
            Vector3 right = cameraTransform.right;
            right.y = 0;
            return right;
        }

        private Vector3 GetCameraForward()
        {
            Vector3 forward = cameraTransform.forward;
            forward.y = 0;
            return forward;
        }

        private void UpdateBasePosition()
        {
            if (targetPosition.sqrMagnitude > 0.1f)
            {
                speed = Mathf.Lerp(speed, movementSpeedCurve.Evaluate(distanceToCamera), Time.deltaTime * accelaration);
                transform.position += targetPosition * speed * Time.deltaTime;
            }

            targetPosition = Vector3.zero;
        }

        private void InputManager_RotateCamera(object sender, InputManager.OnCameraRotateArgs e)
        {
            if (!InputManager.Instance.GetRotateButtonPressed())
            {
                return;
            }

            Vector2 value = e.rotationAmount;
            transform.rotation = Quaternion.Euler(-value.y * rotationSpeedCurve.Evaluate(distanceToCamera) * Time.deltaTime + transform.rotation.eulerAngles.x, value.x * rotationSpeedCurve.Evaluate(distanceToCamera) * Time.deltaTime + transform.rotation.eulerAngles.y, 0f);


            float x = WrapAngle(transform.rotation.eulerAngles.x);
            x = Mathf.Clamp(x, -30, 30);

            transform.eulerAngles = new Vector3(x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }

        private static float WrapAngle(float angle)
        {
            angle %= 360;
            if (angle > 180)
                return angle - 360;

            return angle;
        }

        private void InputManager_ZoomCamera(object sender, InputManager.OnCameraZoomArgs e)
        {
            float zoomAmount = e.zoomAmount;

            float camDistance = Vector3.Distance(cameraTransform.position, transform.position);

            cameraTransform.LookAt(transform);
            if (zoomAmount < 0 && camDistance > minCamDistance)
            {
                cameraTransform.position = Vector3.MoveTowards(cameraTransform.position, transform.position, zoomSpeedCurve.Evaluate(distanceToCamera) * Time.deltaTime);
            }
            else if (zoomAmount > 0 && camDistance < maxCamDistance)
            {
                cameraTransform.position = Vector3.MoveTowards(cameraTransform.position, cameraTransform.position + (cameraTransform.position - transform.position), zoomSpeedCurve.Evaluate(distanceToCamera) * Time.deltaTime);
            }

            Vector3 direction = (cameraTransform.position - transform.position).normalized;
            if (camDistance < minCamDistance)
            {
                cameraTransform.position = Vector3.MoveTowards(cameraTransform.position, transform.position + direction * minCamDistance, zoomSpeedCurve.Evaluate(distanceToCamera) * Time.deltaTime);
            }
            else if (camDistance > maxCamDistance)
            {
                cameraTransform.position = Vector3.MoveTowards(cameraTransform.position, transform.position + direction * maxCamDistance, zoomSpeedCurve.Evaluate(distanceToCamera) * Time.deltaTime);
            }
        }
    }
}