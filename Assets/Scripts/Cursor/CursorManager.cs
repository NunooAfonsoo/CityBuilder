using Grid;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Tools;
using UnityEngine.UI;
using System.Linq;
using Constants;
using ResourceTypes;
using Population;
using System;

namespace Cursor
{
    public class CursorManager : MonoBehaviour
    {
        public static CursorManager Instance { get; private set; }

        //Dragging
        [Header("Dragging")]
        [SerializeField] private RectTransform cursorMarker;
        public Vector3Int CurrentMouseGridPosition{ get; private set; }
        private float nodeOffset;
        private bool dragging;
        private HashSet<Vector2Int> nodesSelected;

        //Cursor Marker and Tools
        private DeleteTool deleteTool;
        private Image cursorMarkerImage;
        private GatherResourcesTool gatherResourcesTool;
        private Resource resourceToGather;

        //Controls NewInputSystem
        private Controls controls;
        private InputAction rightMouseCursor;
        private InputAction leftMouseCursor;

        private Vector3Int mousePosition;
        private void Start()
        {

            Instance = this;
            nodeOffset = (float)Grid.Grid.Instance.NodeSize / 2f;
            nodesSelected = new HashSet<Vector2Int>();
            deleteTool = null;
            gatherResourcesTool = null;
            cursorMarkerImage = cursorMarker.GetComponent<Image>();

            Color color;
            ColorUtility.TryParseHtmlString(Colors.CursorMarkerColor, out color);
            cursorMarkerImage.color = color;
        }

        private void OnEnable()
        {
            controls = new Controls();
            controls.Cursor.Enable();

            rightMouseCursor = controls.Cursor.RightMouseButton;
            leftMouseCursor = controls.Cursor.LeftMouseButton;

            leftMouseCursor.started += DragStart;
            leftMouseCursor.canceled += DragEnd;
        }

        private void OnDisable()
        {
            controls.Cursor.Disable();
        }

        private void Update()
        {
            HandleCursorMarkerPosition();
        }

        private void DragStart(InputAction.CallbackContext obj)
        {
            dragging = true;
            CurrentMouseGridPosition = mousePosition;
        }

        private void DragEnd(InputAction.CallbackContext obj)
        {
            Node node = Grid.Grid.Instance.GetCell(CurrentMouseGridPosition.x, CurrentMouseGridPosition.z);

            cursorMarker.sizeDelta = new Vector2(1, 1);
            if (nodesSelected.Count > 0)
            {
                CheckSelectedNodesForPossibleActions();
            }

            dragging = false;
        }

        private void HandleMouseDrag(Vector3Int draggingNewMousePosition)
        {
            nodesSelected.Clear();

            Vector3Int positionDiff = draggingNewMousePosition - CurrentMouseGridPosition;
            Vector2Int bottomLeft = GetBottomLeftNodePosition(V3ToV2(positionDiff));


            if (positionDiff.x >= 0 && positionDiff.z >= 0)
            {
                cursorMarker.position = new Vector3(bottomLeft.x - nodeOffset, cursorMarker.position.y, bottomLeft.y - nodeOffset);
            }
            else if (positionDiff.x <= -1 && positionDiff.z <= -1)
            {
                cursorMarker.position = new Vector3(bottomLeft.x - nodeOffset, cursorMarker.position.y, bottomLeft.y - nodeOffset);
            }
            else if (positionDiff.x <= -1)
            {
                cursorMarker.position = new Vector3(bottomLeft.x - nodeOffset, cursorMarker.position.y, bottomLeft.y - nodeOffset);
            }
            else if(positionDiff.z <= -1)
            {
                cursorMarker.position = new Vector3(bottomLeft.x - nodeOffset, cursorMarker.position.y, bottomLeft.y - nodeOffset);
            }
            cursorMarker.sizeDelta = new Vector2(Mathf.Abs(positionDiff.x) + transform.localScale.x, Mathf.Abs(positionDiff.z) + transform.localScale.x);
            UpdateNodesSelected(V3ToV2(positionDiff), bottomLeft);
        }

        private Vector2Int GetBottomLeftNodePosition(Vector2Int positionDiff)
        {
            if (positionDiff.x >= 0 && positionDiff.y >= 0)
            {
                return new Vector2Int(CurrentMouseGridPosition.x, CurrentMouseGridPosition.z);
            }
            else if (positionDiff.x <= -1 && positionDiff.y <= -1)
            {
                return new Vector2Int(positionDiff.x + CurrentMouseGridPosition.x, positionDiff.y + CurrentMouseGridPosition.z);
            }
            else if (positionDiff.x <= -1)
            {
                return new Vector2Int(positionDiff.x + CurrentMouseGridPosition.x, CurrentMouseGridPosition.z);
            }
            else if (positionDiff.y <= -1)
            {
                return new Vector2Int(CurrentMouseGridPosition.x, positionDiff.y + CurrentMouseGridPosition.z);
            }
            return Vector2Int.zero;
        }

        private void HandleCursorMarkerPosition()
        {
            mousePosition = Grid.Grid.Instance.GetGridPositionFromWorldPosition(Mouse3D.Instance.GetMouseWorldPosition());

            if (CurrentMouseGridPosition != mousePosition && !dragging)
            {
                CurrentMouseGridPosition = mousePosition;
                cursorMarker.position = new Vector3(CurrentMouseGridPosition.x - nodeOffset, cursorMarker.position.y, CurrentMouseGridPosition.z - nodeOffset);
            }

            if(dragging)
            {
                HandleMouseDrag(mousePosition);
            }
        }

        private void CheckSelectedNodesForPossibleActions()
        {
            foreach(Vector2Int nodePosition in nodesSelected)
            {

                Node node = Grid.Grid.Instance.GetCell(nodePosition.x, nodePosition.y);

                deleteTool?.UseTool(node.Building);
                gatherResourcesTool?.UseTool(node.Resource, resourceToGather.GetType());
            }

            GatherResource();

            nodesSelected.Clear();
        }

        private void GatherResource()
        {
            PopulationManager populationManager = PopulationManager.Instance;
            HashSet<Person> idlePeople = populationManager.IdlePeople;
            Person[] prevIdlePeople = new Person[populationManager.IdlePeople.Count];
            populationManager.IdlePeople.CopyTo(prevIdlePeople);
            if (resourceToGather == null)
            {
                return;
            }

            foreach (Person person in prevIdlePeople)
            {
                if (idlePeople.Contains(person))
                {
                    Resource resourceToGather = ResourceGatheringManager.Instance.GetNextResourceToHarvest();
                    if (resourceToGather != null)
                    {
                        person.NewGatherResourceBT(resourceToGather);
                    }
                }
            }
        }

        public void ResetDeleteTool()
        {
            deleteTool = null;
            gatherResourcesTool = null;
            Color color;
            ColorUtility.TryParseHtmlString(Colors.CursorMarkerColor, out color);
            cursorMarkerImage.color = color;

            rightMouseCursor.performed -= OnClickCancel;
        }

        public void CreateDeleteTool()
        {
            deleteTool = new DeleteTool();
            Color color;
            ColorUtility.TryParseHtmlString(Colors.CursorMarkerDeleteColor, out color);
            cursorMarkerImage.color = color;

            rightMouseCursor.performed += OnClickCancel;
        }

        public void CreateGatherResourcesTool(Resource resource)
        {
            gatherResourcesTool = new GatherResourcesTool();
            resourceToGather = resource;

            Color color = Color.black;
            if(resourceToGather.GetType() == typeof(ResourceTypes.Tree))
            {
                ColorUtility.TryParseHtmlString(Colors.CursorMarkerGatherTreesColor, out color);
            }
            else if(resourceToGather.GetType() == typeof(ResourceTypes.Stone))
            {
                ColorUtility.TryParseHtmlString(Colors.CursorMarkerGatherStoneColor, out color);
            }
            cursorMarkerImage.color = color;


            rightMouseCursor.performed += OnClickCancel;
        }

        private void OnClickCancel(InputAction.CallbackContext obj)
        {
            ResetDeleteTool();
        }

        private Vector2Int V3ToV2(Vector3Int vector)
        {
            return new Vector2Int(vector.x, vector.z);
        }

        private void UpdateNodesSelected(Vector2Int positionDiff, Vector2Int bottomLeft)
        {
            Vector2Int topRight = bottomLeft + new Vector2Int(Mathf.Abs(positionDiff.x), Mathf.Abs(positionDiff.y));

            for (int x = bottomLeft.x; x <= topRight.x; x++)
            {
                for (int y = bottomLeft.y; y <= topRight.y; y++)
                {
                    nodesSelected.Add(new Vector2Int(x, y));
                }
            }
        }

        public void DisableCursorMarker()
        {
            cursorMarker.gameObject.SetActive(false);
        }

        public void EnableCursorMarker()
        {
            cursorMarker.gameObject.SetActive(true);
        }
    }
}