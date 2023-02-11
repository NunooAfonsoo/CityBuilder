using Grid;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Tools;
using UnityEngine.UI;
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
        public Vector3 CurrentMouseGridPosition { get; private set; }
        private float nodeOffset;
        private bool dragging;
        private HashSet<Vector2> nodesSelected;

        //Cursor Marker and Tools
        private DeleteTool deleteTool;
        private Image cursorMarkerImage;
        private GatherResourcesTool gatherResourcesTool;
        private Resource resourceToGather;
        private Action onNewResourceSelected;

        //Controls NewInputSystem
        private Controls controls;
        private InputAction rightMouseCursor;
        private InputAction leftMouseCursor;

        private Vector3 mousePosition;
        private float nodeSize;

        private void Awake()
        {
            Instance = this;

            nodesSelected = new HashSet<Vector2>();
            deleteTool = null;
            gatherResourcesTool = null;
            cursorMarkerImage = cursorMarker.GetComponent<Image>();
        }

        private void Start()
        {
            nodeOffset = (float)Grid.Grid.Instance.NodeSize / 2f;

            ColorUtility.TryParseHtmlString(Colors.CursorMarkerColor, out Color color);
            cursorMarkerImage.color = color;

            nodeSize = Grid.Grid.Instance.NodeSize;
            cursorMarker.sizeDelta = new Vector2(nodeSize, nodeSize);
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
            cursorMarker.sizeDelta = new Vector2(nodeSize, nodeSize);
            if (nodesSelected.Count > 0)
            {
                CheckSelectedNodesForPossibleActions();
            }

            dragging = false;
        }

        private void HandleMouseDrag(Vector3 draggingNewMousePosition)
        {
            nodesSelected.Clear();

            Vector3 positionDiff = draggingNewMousePosition - CurrentMouseGridPosition;
            Vector2 bottomLeft = GetBottomLeftNodePosition(V3ToV2(positionDiff));


            if (positionDiff.x >= 0 && positionDiff.z >= 0)
            {
                cursorMarker.position = new Vector3(bottomLeft.x - nodeOffset, cursorMarker.position.y, bottomLeft.y - nodeOffset);
            }
            else if (positionDiff.x <= -nodeSize && positionDiff.z <= -nodeSize)
            {
                cursorMarker.position = new Vector3(bottomLeft.x - nodeOffset, cursorMarker.position.y, bottomLeft.y - nodeOffset);
            }
            else if (positionDiff.x <= -nodeSize)
            {
                cursorMarker.position = new Vector3(bottomLeft.x - nodeOffset, cursorMarker.position.y, bottomLeft.y - nodeOffset);
            }
            else if(positionDiff.z <= -nodeSize)
            {
                cursorMarker.position = new Vector3(bottomLeft.x - nodeOffset, cursorMarker.position.y, bottomLeft.y - nodeOffset);
            }
            cursorMarker.sizeDelta = new Vector2(Mathf.Abs(positionDiff.x) + transform.localScale.x * nodeSize, Mathf.Abs(positionDiff.z) + transform.localScale.x * nodeSize);
            UpdateNodesSelected(V3ToV2(positionDiff), bottomLeft);
        }

        private Vector2 GetBottomLeftNodePosition(Vector2 positionDiff)
        {
            if (positionDiff.x >= 0 && positionDiff.y >= 0)
            {
                return new Vector2(CurrentMouseGridPosition.x, CurrentMouseGridPosition.z);
            }
            else if (positionDiff.x <= -nodeSize && positionDiff.y <= -nodeSize)
            {
                return new Vector2(positionDiff.x + CurrentMouseGridPosition.x, positionDiff.y + CurrentMouseGridPosition.z);
            }
            else if (positionDiff.x <= -nodeSize)
            {
                return new Vector2(positionDiff.x + CurrentMouseGridPosition.x, CurrentMouseGridPosition.z);
            }
            else if (positionDiff.y <= -nodeSize)
            {
                return new Vector2(CurrentMouseGridPosition.x, positionDiff.y + CurrentMouseGridPosition.z);
            }
            return Vector2.zero;
        }

        private void HandleCursorMarkerPosition()
        {
            mousePosition = Grid.Grid.Instance.GetGridPositionFromWorldPosition(Mouse3D.Instance.GetMouseWorldPosition());
            mousePosition = nodeSize * mousePosition;

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
            foreach(Vector2 nodeGridPosition in nodesSelected)
            {

                Node node = Grid.Grid.Instance.GetCell((int)nodeGridPosition.x, (int)nodeGridPosition.y);

                deleteTool?.UseDeleteTool(node);
                gatherResourcesTool?.UseResourceTool(node.Resource, resourceToGather.GetType(), onNewResourceSelected);
            }

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
            ColorUtility.TryParseHtmlString(Colors.CursorMarkerColor, out Color color);
            cursorMarkerImage.color = color;

            rightMouseCursor.performed -= OnClickCancel;
        }

        public void CreateDeleteTool()
        {
            deleteTool = new DeleteTool();
            ColorUtility.TryParseHtmlString(Colors.CursorMarkerDeleteColor, out Color color);
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
            onNewResourceSelected += GatherResource;
        }

        private void OnClickCancel(InputAction.CallbackContext obj)
        {
            ResetDeleteTool();
            onNewResourceSelected -= GatherResource;
        }

        private Vector2 V3ToV2(Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        private void UpdateNodesSelected(Vector2 positionDiff, Vector2 bottomLeft)
        {
            Vector2 topRight = bottomLeft + new Vector2(Mathf.Abs(positionDiff.x), Mathf.Abs(positionDiff.y));
            for (float x = bottomLeft.x; x <= topRight.x; x += nodeSize)
            {
                for (float y = bottomLeft.y; y <= topRight.y; y += nodeSize)
                {
                    nodesSelected.Add(new Vector2(x, y));
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