using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerPathFollower : MonoBehaviour
{
    [Header("Path Settings")]
    public PathManager pathManager;
    public float moveSpeed = 4f;
    public float rotationSpeed = 10f;

    [Header("Scale Settings")]
    [Tooltip("Small scale of character when following path")]
    public Vector3 smallScale = new Vector3(0.6f, 0.6f, 0.6f); // Size small ho jayega
    public float scaleSpeed = 8f;

    [Header("Restart Settings")]
    public float restartDelay = 1.0f;

    [Header("Status")]
    public int currentWaypointIndex = 0;
    public bool isMoving = false;
    private bool isFollowingRealtime = false;
    private bool hasFinishedPath = false;
    private Vector3 originalScale;

    private void Awake()
    {
        // Character ka initial scale save karein
        originalScale = transform.localScale;
        if (originalScale == Vector3.zero)
        {
            originalScale = Vector3.one;
        }
    }

    /// <summary>
    /// Path draw shuru hote hi child 0 active hoga, size small hogi aur character sath sath move karega
    /// </summary>
    public void StartRealtimeFollowing()
    {
        isMoving = true;
        isFollowingRealtime = true;
        hasFinishedPath = false;

        // Character ka size small karein
        Vector3 targetScale = (smallScale != Vector3.zero) ? smallScale : (originalScale * 0.5f);
        transform.localScale = targetScale;

        // Idle sprite hide karein
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        // Child 0 (running animation) active karein
        if (transform.childCount > 0)
        {
            GameObject childObject = transform.GetChild(0).gameObject;
            childObject.SetActive(true);
        }

        // Agar first waypoint already hai to character ko first position par rakhein
        if (pathManager != null && pathManager.waypoints.Count > 0 && pathManager.waypoints[0] != null)
        {
            transform.position = pathManager.waypoints[0].position;
            currentWaypointIndex = 0;
        }
        else
        {
            currentWaypointIndex = 0;
        }
    }

    private void Update()
    {
        if (!isFollowingRealtime) return;

        if (pathManager == null || pathManager.waypoints == null || pathManager.waypoints.Count == 0)
            return;

        if (currentWaypointIndex >= pathManager.waypoints.Count)
            return;

        Transform targetWaypoint = pathManager.waypoints[currentWaypointIndex];
        if (targetWaypoint == null)
        {
            currentWaypointIndex++;
            return;
        }

        Vector3 targetPosition = targetWaypoint.position;

        // Dynamic catch-up speed: agar user aage tezi se draw kare to character sath sath pahuche
        float distanceToLead = Vector3.Distance(transform.position, pathManager.waypoints[pathManager.waypoints.Count - 1].position);
        float currentSpeed = moveSpeed;
        if (distanceToLead > 0.5f)
        {
            currentSpeed = Mathf.Max(moveSpeed, distanceToLead * 4f);
        }

        // Position movement: waypoint ki taraf move karein
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            currentSpeed * Time.deltaTime
        );

        // Rotation: jidhar move kar raha hai udhar face karein
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            if (direction.x < -0.05f)
            {
                transform.localRotation = Quaternion.Euler(180f, 0f, 180f);
            }
            else if (direction.x > 0.05f)
            {
                transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }

        // Agar current waypoint tak pahunch gaya
        if (Vector3.Distance(transform.position, targetPosition) <= 0.05f)
        {
            // Aage aur waypoints hain to agle waypoint ki taraf badhein
            if (currentWaypointIndex < pathManager.waypoints.Count - 1)
            {
                currentWaypointIndex++;
            }
            else if (hasFinishedPath)
            {
                // Target reach ho chuka hai aur character bhi target tak pahunch gaya
                OnPathCompleted();
            }
        }
    }

    /// <summary>
    /// Target collide hone par call karein
    /// </summary>
    public void OnTargetReached()
    {
        hasFinishedPath = true;

        if (pathManager != null && pathManager.waypoints.Count > 0)
        {
            int lastIndex = pathManager.waypoints.Count - 1;
            if (currentWaypointIndex >= lastIndex &&
                Vector3.Distance(transform.position, pathManager.waypoints[lastIndex].position) <= 0.1f)
            {
                OnPathCompleted();
            }
        }
    }

    private void OnPathCompleted()
    {
        if (!isFollowingRealtime && !isMoving) return;

        isFollowingRealtime = false;
        isMoving = false;
        Debug.Log("Path complete ho gaya hai! Restarting scene...");

        StartCoroutine(RestartSceneWithDelay());
    }

    public void StartMovement()
    {
        StartRealtimeFollowing();
        OnTargetReached();
    }

    public void CallLoadScene()
    {
        StartCoroutine(RestartSceneWithDelay());
    }

    private IEnumerator RestartSceneWithDelay()
    {
        yield return new WaitForSeconds(restartDelay > 0f ? restartDelay : 1.0f);
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void ResetPath()
    {
        StopAllCoroutines();
        isFollowingRealtime = false;
        isMoving = false;
        hasFinishedPath = false;
        currentWaypointIndex = 0;
        transform.localScale = originalScale;
        transform.localRotation = Quaternion.identity;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = true;

        if (transform.childCount > 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}