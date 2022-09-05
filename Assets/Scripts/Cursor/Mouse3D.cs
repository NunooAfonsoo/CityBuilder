using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cursor
{
    public class Mouse3D : MonoBehaviour {

        public static Mouse3D Instance { get; private set; }

        [SerializeField] private LayerMask mouseColliderLayerMask = new LayerMask();

        private void Awake() {
            Instance = this;
        }

        private void Update() {

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue()));
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mouseColliderLayerMask)) {
                //transform.position = raycastHit.point;
            }
        }

        public Vector3 GetMouseWorldPosition() {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Mouse.current.position.x.ReadValue(),Mouse.current.position.y.ReadValue()));
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mouseColliderLayerMask)) {
                return raycastHit.point;
            } else {
                return Vector3.zero;
            }
        }
    }
}
