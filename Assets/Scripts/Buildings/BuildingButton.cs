using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cursor;
using UnityEngine.InputSystem;
using System;

namespace Buildings
{
    public class BuildingButton : MonoBehaviour
    {
        private Button button;
        [SerializeField] protected BuildingSO buildingSO;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(delegate { SelectBuilding(0); } );
        }

        private void SelectBuilding(float rotation)
        {
            buildingSO.CreateBuilding(Vector3.zero, rotation);
            StartCoroutine(PlaceBuilding());
        }

        protected virtual IEnumerator PlaceBuilding()
        {
            while (!Mouse.current.rightButton.wasPressedThisFrame)
            {
                Vector3Int cursorPosition = CursorManager.Instance.CurrentMouseGridPosition;
                buildingSO.MoveBuildingToPosition(cursorPosition);
                bool isPositionFree = buildingSO.IsPositionFree(cursorPosition);

                if (Keyboard.current.rKey.wasPressedThisFrame)
                {
                    buildingSO.RotateBuilding();
                }

                if (Mouse.current.leftButton.wasPressedThisFrame && isPositionFree)
                {
                    buildingSO.BuildingPlaced(CursorManager.Instance.CurrentMouseGridPosition.x, CursorManager.Instance.CurrentMouseGridPosition.z);
                    buildingSO.CreateBuilding(Vector3.zero, buildingSO.BuildingRotation);
                }
                yield return null;
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                buildingSO.DestroyBuilding();
            }
        }
    }
}