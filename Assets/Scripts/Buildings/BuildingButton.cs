using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cursor;
using System;
using Constants;

namespace Buildings
{
    public class BuildingButton : MonoBehaviour
    {
        [SerializeField] protected BuildingSO buildingSO;

        private Button button;

        private bool isPositionFree;
        private bool previewingBuilding;

        private void Awake()
        {
            isPositionFree = true;
            previewingBuilding = false;

            button = GetComponent<Button>();
            button.onClick.AddListener(SelectBuilding);
        }

        private void Start()
        {
            InputManager.Instance.OnBuildingRotate += InputManager_OnBuilding_Rotate;
            InputManager.Instance.OnBuildingPlaced += TryPlaceBuilding;
            InputManager.Instance.OnBuildingPlacementCanceled += CancelBuildingPlacement;
        }

        private void SelectBuilding()
        {
            InputManager.Instance.EnableBuildingControls();

            buildingSO.CreateBuilding(Vector3.zero);

            previewingBuilding = true;
            StartCoroutine(PreviewBuilding());
        }

        protected virtual IEnumerator PreviewBuilding()
        {
            while (previewingBuilding)
            {
                Vector3 cursorPosition = CursorManager.Instance.CurrentMouseGridPosition;
                buildingSO.MoveBuildingToPosition(cursorPosition);
                isPositionFree = buildingSO.IsPositionFree(Grid.Grid.Instance.GetGridPositionFromWorldPosition(cursorPosition));

                buildingSO.SetBuildingMaterials(isPositionFree);

                yield return null;
            }
        }

        private void InputManager_OnBuilding_Rotate(object sender, EventArgs e)
        {
            buildingSO.RotateBuilding();
        }

        private void TryPlaceBuilding(object sender, EventArgs e)
        {
            if(isPositionFree)
            {
                buildingSO.BuildingPlaced((int)CursorManager.Instance.CurrentMouseGridPosition.x, (int)CursorManager.Instance.CurrentMouseGridPosition.z);
                buildingSO.CreateBuilding(Vector3.zero, buildingSO.BuildingRotation);
            }
        }

        private void CancelBuildingPlacement(object sender, EventArgs e)
        {
            InputManager.Instance.DisableBuildingControls();

            previewingBuilding = false;

            buildingSO.DestroyBuilding();
        }
    }
}