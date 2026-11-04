using UnityEngine;

public class Waypoints : MonoBehaviour
{
    [Header("Scene View Waypoint Size")]
    [SerializeField]
    [Range(0f, 2f)]
    private float waypointSize = 1f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        foreach (Transform waypoint in transform)
        {
            Gizmos.DrawWireSphere(waypoint.position, waypointSize);
        }

        Gizmos.color = Color.red;

        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.DrawLine(
                transform.GetChild(i).position,
                transform.GetChild(i + 1).position
            );
        }

        if (transform.childCount > 1)
        {
            Gizmos.DrawLine(
                transform.GetChild(transform.childCount - 1).position,
                transform.GetChild(0).position
            );
        }
    }

    public Transform GetNextWaypoint(Transform currentWaypoint)
    {
        if (currentWaypoint == null)
        {
            return transform.GetChild(0);
        }

        if (currentWaypoint.GetSiblingIndex() < transform.childCount - 1)
        {
            return transform.GetChild(currentWaypoint.GetSiblingIndex() + 1);
        }
        else
        {
            return transform.GetChild(0);
        }
    }
}