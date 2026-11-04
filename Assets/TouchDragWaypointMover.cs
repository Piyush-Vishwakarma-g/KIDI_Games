using UnityEngine;
using UnityEngine.InputSystem;

public class TouchDragWaypointMover : MonoBehaviour
{
    [Header("Waypoint Path")]
    [SerializeField]
    private Waypoints waypoints;

    [Header("Drag Camera")]
    [SerializeField]
    private Camera dragCamera;

    [Header("Movement Settings")]
    [SerializeField]
    private float rotationSpeed = 8f;

    public GameObject sodaAnim;

    [Header("Debug")]
    [SerializeField]
    private bool showDebugLogs = true;

    private Transform currentWaypoint;
    private Transform originalWaypoint;
    private Vector3 originalPosition;
    private Vector3 originalRotation;
    private bool isDragging;
    private Plane dragPlane;

    private void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation.eulerAngles;

        if (dragCamera == null)
        {
            dragCamera = Camera.main;

            if (showDebugLogs && dragCamera != null)
            {
                Debug.Log("Drag Camera automatically set: " + dragCamera);
            }
        }

        if (dragCamera == null)
        {
            Debug.LogError("No drag camera is assigned and no MainCamera was found.", this);
            enabled = false;
            return;
        }

        if (waypoints == null)
        {
            Debug.LogError("WAYPOINTS ASSIGN NAHI HAI! Inspector mein Waypoints field mein Waypoints GameObject drag karo.");
            enabled = false;
            return;
        }

        if (waypoints.transform.childCount < 2)
        {
            Debug.LogError("WAYPOINTS KE ANDAR KAM SE KAM 2 CHILD WAYPOINTS CHAHIYE!");
            enabled = false;
            return;
        }

        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        originalWaypoint = currentWaypoint;

        if (showDebugLogs)
        {
            Debug.Log("Starting waypoint: " + currentWaypoint.name);
        }
    }

    private void Update()
    {
        if (!TryGetPointerInput(
                out Vector2 pointerPosition,
                out bool pressedThisFrame,
                out bool isPressed,
                out bool releasedThisFrame))
        {
            return;
        }

        if (pressedThisFrame)
        {
            if (showDebugLogs)
            {
                Debug.Log("Pointer pressed: " + pointerPosition);
            }

            TryStartDrag(pointerPosition);
        }
        else if (isPressed && isDragging)
        {
            DragAlongPath(pointerPosition);
        }
        else if (releasedThisFrame && isDragging)
        {
            transform.position = originalPosition;
            transform.rotation = Quaternion.Euler(originalRotation);
            currentWaypoint = originalWaypoint;
            isDragging = false;

            if (showDebugLogs)
            {
                Debug.Log("Pointer released. Object returned to its original position.");
            }
        }
    }

    private bool TryGetPointerInput(
        out Vector2 pointerPosition,
        out bool pressedThisFrame,
        out bool isPressed,
        out bool releasedThisFrame)
    {
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            if (touch.press.isPressed ||
                touch.press.wasPressedThisFrame ||
                touch.press.wasReleasedThisFrame)
            {
                pointerPosition = touch.position.ReadValue();
                pressedThisFrame = touch.press.wasPressedThisFrame;
                isPressed = touch.press.isPressed;
                releasedThisFrame = touch.press.wasReleasedThisFrame;
                return true;
            }
        }

        if (Mouse.current != null)
        {
            pointerPosition = Mouse.current.position.ReadValue();
            pressedThisFrame = Mouse.current.leftButton.wasPressedThisFrame;
            isPressed = Mouse.current.leftButton.isPressed;
            releasedThisFrame = Mouse.current.leftButton.wasReleasedThisFrame;
            return true;
        }

        pointerPosition = default;
        pressedThisFrame = false;
        isPressed = false;
        releasedThisFrame = false;
        return false;
    }

    private void TryStartDrag(Vector2 touchPosition)
    {
        Ray ray = dragCamera.ScreenPointToRay(touchPosition);

        RaycastHit[] hits = Physics.RaycastAll(ray);
        RaycastHit? draggableHit = null;

        if (showDebugLogs)
        {
            Debug.Log("Raycast hits: " + hits.Length);

            foreach (RaycastHit hit in hits)
            {
                Debug.Log("Hit object: " + hit.transform.name);
            }
        }

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                draggableHit = hit;
                break;
            }
        }

        if (draggableHit.HasValue)
        {
            isDragging = true;
            dragPlane = new Plane(Vector3.up, transform.position);

            if (showDebugLogs)
            {
                Debug.Log("DRAG START HO GAYA: " + gameObject.name);
            }
        }
        else if (showDebugLogs && hits.Length > 0)
        {
            Debug.LogWarning("Pointer hit another object: " + hits[0].transform.name);
        }
        else if (showDebugLogs)
        {
            Debug.LogWarning("Raycast did not hit an object. Check the collider or camera.");
        }
    }

    private void DragAlongPath(Vector2 touchPosition)
    {
        Ray ray = dragCamera.ScreenPointToRay(touchPosition);

        if (!dragPlane.Raycast(ray, out float enterDistance))
        {
            if (showDebugLogs)
            {
                Debug.LogWarning("Touch plane par project nahi ho raha.");
            }

            return;
        }

        Vector3 touchWorldPosition = ray.GetPoint(enterDistance);

        transform.position = GetClosestPointOnCurrentSegment(touchWorldPosition);

        RotateTowardsCurrentWaypoint();
    }

    private Vector3 GetClosestPointOnCurrentSegment(Vector3 touchWorldPosition)
    {
        int waypointCount = waypoints.transform.childCount;

        for (int i = 0; i < waypointCount; i++)
        {
            Transform endWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
            Transform nextEndWaypoint = waypoints.GetNextWaypoint(endWaypoint);

            Vector3 currentPoint = GetClosestPointOnSegment(
                touchWorldPosition,
                currentWaypoint.position,
                endWaypoint.position,
                Vector3.up,
                out float currentDistance
            );
            GetClosestPointOnSegment(
                touchWorldPosition,
                endWaypoint.position,
                nextEndWaypoint.position,
                Vector3.up,
                out float nextDistance
            );

            if (nextDistance < currentDistance)
            {
                currentWaypoint = endWaypoint;
                if (showDebugLogs)
                {
                    Debug.Log("Next waypoint: " + currentWaypoint.name);
                }

                continue;
            }

            return currentPoint;
        }

        return currentWaypoint.position;
    }

    private static Vector3 GetClosestPointOnSegment(
        Vector3 touchPosition,
        Vector3 startPoint,
        Vector3 endPoint,
        Vector3 planeNormal,
        out float distanceSquared)
    {
        Vector3 segment = Vector3.ProjectOnPlane(endPoint - startPoint, planeNormal);
        float segmentLengthSquared = segment.sqrMagnitude;

        if (segmentLengthSquared < 0.0001f)
        {
            distanceSquared = Vector3.ProjectOnPlane(
                touchPosition - startPoint,
                planeNormal
            ).sqrMagnitude;
            return startPoint;
        }

        Vector3 touchOffset = Vector3.ProjectOnPlane(
            touchPosition - startPoint,
            planeNormal
        );
        float pathPosition = Mathf.Clamp01(
            Vector3.Dot(touchOffset, segment) / segmentLengthSquared
        );
        Vector3 closestPoint = startPoint + (endPoint - startPoint) * pathPosition;
        distanceSquared = Vector3.ProjectOnPlane(
            touchPosition - closestPoint,
            planeNormal
        ).sqrMagnitude;
        return closestPoint;
    }

    private void RotateTowardsCurrentWaypoint()
    {
        int currentWaypointIndex = currentWaypoint.GetSiblingIndex();
        if (currentWaypointIndex == 2 || currentWaypointIndex == 3)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, -90f);
            transform.GetChild(0).gameObject.SetActive(false);
            sodaAnim.SetActive(true);
            ChemistryLab1Manager.instance.modelUI3.SetActive(true);
            ChemistryLab1Manager.instance.modelUI2.SetActive(false);
              ChemistryLab1Manager.instance.vinegar.GetComponent<TouchDragVinegarWaypointMover>().enabled = true;

            return;
        }

        Transform targetWaypoint = waypoints.GetNextWaypoint(currentWaypoint);

        Vector3 direction = targetWaypoint.position - currentWaypoint.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(
            direction.normalized,
            Vector3.up
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}