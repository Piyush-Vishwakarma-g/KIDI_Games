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

    [Header("Restart Settings")]
    public float restartDelay = 1.0f;

    [Header("Status")]
    private int currentWaypointIndex = 0;
    private bool isMoving = false;
    private Vector3 originalScale;

    private void Awake()
    {
        // Character ka initial scale save karein
        originalScale = transform.localScale;
    }

    public void StartMovement()
    {
        if (!isMoving && pathManager != null && pathManager.waypoints.Count > 0)
        {
            transform.position = pathManager.waypoints[0].position;
            currentWaypointIndex = 1;

            StartCoroutine(MoveAlongPath());
        }
    }

    IEnumerator MoveAlongPath()
    {
        isMoving = true;

        Vector3 targetScale = originalScale;
        this.gameObject.GetComponent<SpriteRenderer>().enabled = false;

        GameObject childObject = this.gameObject.transform.GetChild(0).gameObject;
        childObject.SetActive(true);

        while (currentWaypointIndex < pathManager.waypoints.Count)
        {
            Vector3 targetPosition = pathManager.waypoints[currentWaypointIndex].position;

            // X positive (+) half par -> x=180, y=0, z=180


            while (Vector3.Distance(transform.position, targetPosition) > 0.02f)
            {
                // Position movement
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );

                // Smooth scale transition
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 8f);

                // Main player rotation (Agar aap parent ka rotation disable karna chahte hain toh is section ko comment kar sakte hain)
                Vector3 direction = (targetPosition - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                    // X positive par 180 flip offset, negative par normal
                    //Quaternion targetRotation;
                    if (targetPosition.x < 0)
                    {
                        transform.localRotation = Quaternion.Euler(180f, 0f, 180f);
                        Debug.Log("111111111111111111111111");
                    }
                    else if (targetPosition.x > 0)
                    {
                         transform.localRotation = Quaternion.Euler(0f, 0f, 0);
                         Debug.Log("222222222222222222222222");
                    }

                    // Smooth transition
                   // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }

                yield return null;
            }

            transform.position = targetPosition;
            currentWaypointIndex++;
        }

        transform.localScale = targetScale;
        isMoving = false;
        Debug.Log("Path complete ho gaya hai! Restarting scene...");

        StartCoroutine(RestartSceneWithDelay());
    }

    public void CallLoadScene()
    {
        StartCoroutine(RestartSceneWithDelay());
    }

    private IEnumerator RestartSceneWithDelay()
    {
        yield return new WaitForSeconds(3.0f);
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void ResetPath()
    {
        StopAllCoroutines();
        isMoving = false;
        currentWaypointIndex = 0;
        transform.localScale = originalScale;
    }
}