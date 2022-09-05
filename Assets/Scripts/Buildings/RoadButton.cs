using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cursor;
using UnityEngine.InputSystem;
using Grid;
using System;

namespace Buildings
{
    public class RoadButton : BuildingButton
    {
        protected override IEnumerator PlaceBuilding()
        {
            while (!Mouse.current.rightButton.wasPressedThisFrame)
            {
                Vector3Int cursorPosition = CursorManager.Instance.CurrentMouseGridPosition;
                buildingSO.MoveBuildingToPosition(cursorPosition);
                bool isPositionFree = buildingSO.IsPositionFree(cursorPosition);


                if (Mouse.current.leftButton.wasPressedThisFrame && isPositionFree)
                {
                    StartCoroutine(PlaceRoads());//buildingSO.BuildingPlaced(CursorManager.Instance.CurrentMouseGridPosition.x, CursorManager.Instance.CurrentMouseGridPosition.z);
                    break;
                }

                yield return null;
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                buildingSO.DestroyBuilding();
            }
        }

        public IEnumerator PlaceRoads()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            Vector3 cursorPosition = CursorManager.Instance.CurrentMouseGridPosition;
            while (!Mouse.current.leftButton.wasPressedThisFrame && !Mouse.current.rightButton.wasPressedThisFrame)
            {
                if (CursorManager.Instance.CurrentMouseGridPosition != cursorPosition)
                {
                    cursorPosition = CursorManager.Instance.CurrentMouseGridPosition;
                    ClearRoads();
                }

                yield return null;
            }
        }

        private void ClearRoads()
        {

        }
    }
}
