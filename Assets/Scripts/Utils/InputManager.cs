using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Constants
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance;

        private Controls controls;

        public event EventHandler<OnCameraRotateArgs> OnCameraRotate;
        public class OnCameraRotateArgs : EventArgs
        {
            public Vector2 rotationAmount;

            public OnCameraRotateArgs(Vector2 rotationAmount)
            {
                this.rotationAmount = rotationAmount;
            }
        }

        public event EventHandler<OnCameraZoomArgs> OnCameraZoom;
        public class OnCameraZoomArgs : EventArgs
        {
            public float zoomAmount;

            public OnCameraZoomArgs(float zoomAmount)
            {
                this.zoomAmount = zoomAmount;
            }
        }

        public event EventHandler OnBuildingRotate;
        public event EventHandler OnBuildingPlaced;
        public event EventHandler OnBuildingPlacementCanceled;

        private void Awake()
        {
            Instance = this;

            controls = new Controls();
        }

        private void OnEnable()
        {
            controls.Camera.Rotate.performed += RotateCamera;
            controls.Camera.Zoom.performed += ZoomCamera;
            controls.Buildings.Rotate.performed += RotateBuilding;
            controls.Buildings.Place.performed += PlaceBuilding;
            controls.Buildings.Cancel.performed += CancelBuildingPlacement;

            controls.Camera.Enable();
        }

        private void OnDisable()
        {
            controls.Camera.Rotate.performed -= RotateCamera;
            controls.Camera.Zoom.performed -= ZoomCamera;

            controls.Camera.Disable();
            controls.Buildings.Disable();
        }

        private void RotateCamera(InputAction.CallbackContext obj)
        {
            OnCameraRotate?.Invoke(this, new OnCameraRotateArgs(obj.ReadValue<Vector2>()));
        }

        public Vector2 GetCameraMovementAmount()
        {
            return controls.Camera.Move.ReadValue<Vector2>();
        }

        private void ZoomCamera(InputAction.CallbackContext obj)
        {
            float scale = 100f;
            OnCameraZoom?.Invoke(this, new OnCameraZoomArgs(-obj.ReadValue<Vector2>().y / scale));
        }

        public bool GetRotateButtonPressed()
        {
            return controls.Camera.RotateButton.ReadValue<float>() == 1;
        }

        private void RotateBuilding(InputAction.CallbackContext obj)
        {
            OnBuildingRotate?.Invoke(this, EventArgs.Empty);
        }

        private void PlaceBuilding(InputAction.CallbackContext obj)
        {
            OnBuildingPlaced?.Invoke(this, EventArgs.Empty);
        }


        private void CancelBuildingPlacement(InputAction.CallbackContext obj)
        {
            OnBuildingPlacementCanceled?.Invoke(this, EventArgs.Empty);
        }

        public void EnableBuildingControls()
        {
            controls.Buildings.Enable();
        }

        public void DisableBuildingControls()
        {
            controls.Buildings.Disable();
        }
    }
}
