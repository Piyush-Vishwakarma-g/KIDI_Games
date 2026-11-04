using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    [SerializeField]
    private Waypoints waypoints;

    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float distanceThreshold = 0.1f;

    private Transform currentWaypoint;

    private void Start()
    {
        if (waypoints == null || waypoints.transform.childCount == 0)
        {
            enabled = false;
            return;
        }

        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);

        transform.position = currentWaypoint.position;

        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);

        if (currentWaypoint != null)
        {
            transform.LookAt(currentWaypoint);
        }
    }

    private void Update()
    {
        if (currentWaypoint == null)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            currentWaypoint.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, currentWaypoint.position) < distanceThreshold)
        {
            Transform nextWaypoint = waypoints.GetNextWaypoint(currentWaypoint);

            if (nextWaypoint == null || nextWaypoint == currentWaypoint)
            {
                return;
            }

            currentWaypoint = nextWaypoint;
            transform.LookAt(currentWaypoint);
        }

        Vector3 direction = currentWaypoint.position - transform.position;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                5f * Time.deltaTime
            );
        }
    }
}