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
                
                Vector3 cursorPosition = CursorManager.Instance.CurrentMouseGridPosition;
                buildingSO.MoveBuildingToPosition(cursorPosition);
                bool isPositionFree = buildingSO.IsPositionFree(Grid.Grid.Instance.GetGridPositionFromWorldPosition(cursorPosition));

                if (Keyboard.current.rKey.wasPressedThisFrame)
                {
                    buildingSO.RotateBuilding();
                }

                if (Mouse.current.leftButton.wasPressedThisFrame && isPositionFree)
                {
                    buildingSO.BuildingPlaced((int)CursorManager.Instance.CurrentMouseGridPosition.x, (int)CursorManager.Instance.CurrentMouseGridPosition.z);
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