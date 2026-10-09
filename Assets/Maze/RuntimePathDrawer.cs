using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.UI;

public class RuntimePathDrawer : MonoBehaviour
{
    [Header("References")]
    public PathManager pathManager;
    public LineRenderer lineRenderer;
    public Camera mainCamera;
    public PlayerPathFollower playerFollower;
    public Sprite playerEmotion;
    public Sprite[] targetObjDottedLineSprite;
    public SpriteRenderer targetDotetedObj;

    public Slider sliderGaze;

    [Header("UI References")]
    public GameObject tryAgainPopup;
    public GameObject greatJobPopup;
    public List<GameObject> targetObj;

    [Header("Settings")]
    public float minimumDistanceBetweenWaypoints = 0.25f;
    public LayerMask obstacleLayer;
    public string targetTag = "mazeTarget"; // Target object ka Tag Name
    public float restartDelay = 0.5f;

    private bool isDragging = false;
    private bool isFailed = false;
    private bool isTargetReached = false;
    public GameObject[] targetGoal;
    private Vector3 lastWaypointPos;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }

        if (tryAgainPopup != null)
        {
            tryAgainPopup.SetActive(false);
        }

        if (obstacleLayer == 0)
        {
            obstacleLayer = LayerMask.GetMask("Obstacle");
        }
        DisableAllTagretObj();
        // FIX: Added index bounds check before accessing targetObj
        if (MazeTraverseManager.instance != null && targetObj != null)
        {
            int index = MazeTraverseManager.instance.mazeLevelIndex;
            if (index >= 0 && index < targetObj.Count)
            {
                if (targetObj[index] != null && targetObj[index].transform.parent != null)
                {
                    SpriteRenderer sr = targetObj[index].transform.parent.GetComponent<SpriteRenderer>();
                    if (sr != null) sr.enabled = true;
                }
            }
            else
            {
                Debug.LogWarning($"[RuntimePathDrawer] mazeLevelIndex ({index}) is out of targetObj bounds (Count: {targetObj.Count}).");
            }
            if (index >= 0 && index < targetObjDottedLineSprite.Length)
            {
                targetDotetedObj.sprite = targetObjDottedLineSprite[index];
            }

        }
    }
    public void DisableAllTagretObj()
    {
        for (int i = 0; i < targetObj.Count; i++)
        {
            targetObj[i].transform.GetChild(0).gameObject.SetActive(false);
            targetObj[i].transform.parent.GetComponent<SpriteRenderer>().enabled = false;

        }
        if (MazeTraverseManager.instance.isbedsheet)
        {
            targetObj[2].transform.GetChild(0).gameObject.SetActive(true);
        }
        if (MazeTraverseManager.instance.istravel)
        {
            targetObj[3].transform.GetChild(0).gameObject.SetActive(true);
        }
        if (MazeTraverseManager.instance.isblackBall)
        {
            targetObj[1].transform.GetChild(0).gameObject.SetActive(true);
        }
        if (MazeTraverseManager.instance.isheadPhone)
        {
            targetObj[0].transform.GetChild(0).gameObject.SetActive(true);
        }
        sliderGaze.GetComponent<Slider>().value = MazeTraverseManager.instance.mazeLevelIndex;
        switch (MazeTraverseManager.instance.mazeLevelIndex)
        {
            case 1:
                sliderGaze.GetComponent<Slider>().fillRect.GetComponent<Image>().color = Color.red;
                break;
            case 2:
                sliderGaze.GetComponent<Slider>().fillRect.GetComponent<Image>().color = Color.orange;
                break;
            case 3:
                sliderGaze.GetComponent<Slider>().fillRect.GetComponent<Image>().color = Color.yellow;
                break;
            case 4:
                sliderGaze.GetComponent<Slider>().fillRect.GetComponent<Image>().color = Color.green;
                break;
        }
    }

    void Update()
    {
        if (isFailed || isTargetReached) return;

        HandleInput();
    }

    private void HandleInput()
    {
        if (IsPressBegan())
        {
            StartDrawing();
        }
        else if (isDragging && IsPressHeld())
        {
            ContinueDrawing();
        }
        else if (isDragging && IsPressEnded())
        {
            StopDrawing();
        }
    }

    private void StartDrawing()
    {
        if (playerFollower != null)
        {
            playerFollower.ResetPath();
        }

        ClearExistingWaypoints();

        isDragging = true;
        isFailed = false;
        isTargetReached = false;
        Vector3 currentWorldPos = GetWorldTouchPosition();

        if (CheckDirectCollision(currentWorldPos))
        {
            TriggerFail();
            return;
        }

        CheckTargetCollision(currentWorldPos);

        AddWaypoint(currentWorldPos);
    }

    private void ContinueDrawing()
    {
        Vector3 currentWorldPos = GetWorldTouchPosition();

        if (Vector3.Distance(lastWaypointPos, currentWorldPos) < minimumDistanceBetweenWaypoints)
        {
            return;
        }

        // 1. Obstacle / Wall Collision Check
        bool lineHit = Physics2D.Linecast(lastWaypointPos, currentWorldPos, obstacleLayer);
        bool pointHit = CheckDirectCollision(currentWorldPos);

        if (lineHit || pointHit)
        {
            Debug.LogWarning("Wall Collision Detected! Restarting Scene...");
            TriggerFail();
            return;
        }

        // 2. mazeTarget Tag Check
        if (CheckTargetCollision(currentWorldPos))
        {
            return; // Target milne par drawing complete stop ho jayegi
        }

        AddWaypoint(currentWorldPos);
    }

    private bool CheckTargetCollision(Vector3 position)
    {
        Collider2D hitCollider = Physics2D.OverlapCircle(position, 0.2f);
        if (hitCollider != null && hitCollider.CompareTag(targetTag))
        {
            isDragging = false;
            isTargetReached = true;

            Debug.Log("<color=cyan>FFFFFFFFFFF ho gaya</color>");

            if (hitCollider.transform.childCount > 0)
            {
                hitCollider.transform.GetChild(0).gameObject.SetActive(true);
            }

            if (hitCollider.gameObject.name == "ball" && MazeTraverseManager.instance.mazeLevelIndex == 1)
            {
                MazeTraverseManager.instance.isblackBall = true;
                MazeTraverseManager.instance.mazeLevelIndex++;
                if (SimpleAudioPlayerTraversePath.instance != null)
                {
                    SimpleAudioPlayerTraversePath.instance.PlaySound1();
                }
                greatJobPopup.SetActive(true);
            }
            else if (hitCollider.gameObject.name == "Summer" && MazeTraverseManager.instance.mazeLevelIndex == 3)
            {
                MazeTraverseManager.instance.istravel = true;
                MazeTraverseManager.instance.mazeLevelIndex++;
                if (SimpleAudioPlayerTraversePath.instance != null)
                {
                    SimpleAudioPlayerTraversePath.instance.PlaySound1();
                }
                greatJobPopup.SetActive(true);
            }
            else if (hitCollider.gameObject.name == "blanket" && MazeTraverseManager.instance.mazeLevelIndex == 2)
            {
                MazeTraverseManager.instance.isbedsheet = true;
                MazeTraverseManager.instance.mazeLevelIndex++;
                if (SimpleAudioPlayerTraversePath.instance != null)
                {
                    SimpleAudioPlayerTraversePath.instance.PlaySound1();
                }
                greatJobPopup.SetActive(true);
            }
            else if (hitCollider.gameObject.name == "Headphones" && MazeTraverseManager.instance.mazeLevelIndex == 0)
            {
                MazeTraverseManager.instance.isheadPhone = true;
                MazeTraverseManager.instance.mazeLevelIndex++;
                if (SimpleAudioPlayerTraversePath.instance != null)
                {
                    SimpleAudioPlayerTraversePath.instance.PlaySound1();
                }
                greatJobPopup.SetActive(true);
                playerFollower.gameObject.GetComponent<SpriteRenderer>().sprite=playerEmotion;

            }
            else
            {
                if (SimpleAudioPlayerTraversePath.instance != null)
                {
                    SimpleAudioPlayerTraversePath.instance.PlaySound2();
                }

            }

            // Increment level index


            // SAFE ACCESS: Check if index is within targetObj bounds
            int nextIndex = MazeTraverseManager.instance.mazeLevelIndex;
            if (nextIndex >= 0 && nextIndex < targetObj.Count)
            {
                if (targetObj[nextIndex] != null && targetObj[nextIndex].transform.parent != null)
                {
                    var spriteRenderer = targetObj[nextIndex].transform.parent.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.enabled = true;
                    }
                }
            }
            else
            {
                Debug.Log("All targets completed or index out of bounds!");
            }


            // Start movement
            if (playerFollower != null && pathManager != null && pathManager.waypoints.Count > 0)
            {
                playerFollower.CallLoadScene();
            }

            return true;
        }

        return false;
    }

    private void StopDrawing()
    {
        if (isFailed || isTargetReached) return;

        isDragging = false;

        if (playerFollower != null && pathManager != null && pathManager.waypoints.Count > 1)
        {
            playerFollower.CallLoadScene();
        }
    }

    private bool CheckDirectCollision(Vector3 worldPoint)
    {
        Collider2D pointHit = Physics2D.OverlapCircle(worldPoint, 0.12f, obstacleLayer);
        return pointHit != null;
    }

    private void TriggerFail()
    {
        isDragging = false;
        isFailed = true;

        ClearExistingWaypoints();

        if (tryAgainPopup != null)
        {
            tryAgainPopup.SetActive(true);
            SimpleAudioPlayerTraversePath.instance.PlaySound2();
        }

        StartCoroutine(RestartSceneWithDelay());
    }

    private IEnumerator RestartSceneWithDelay()
    {
        yield return new WaitForSeconds(3.0f);
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void RestartDrawing()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    private void AddWaypoint(Vector3 worldPos)
    {
        worldPos.z = 0f;

        GameObject newWaypoint = new GameObject("Runtime_Waypoint_" + pathManager.waypoints.Count);
        newWaypoint.transform.position = worldPos;

        if (pathManager != null)
        {
            newWaypoint.transform.SetParent(pathManager.transform);
            pathManager.waypoints.Add(newWaypoint.transform);
        }

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = pathManager.waypoints.Count;
            lineRenderer.SetPosition(pathManager.waypoints.Count - 1, worldPos);
        }

        lastWaypointPos = worldPos;
    }

    private void ClearExistingWaypoints()
    {
        if (pathManager == null) return;

        foreach (Transform child in pathManager.waypoints)
        {
            if (child != null)
            {
                Destroy(child.gameObject);
            }
        }

        pathManager.waypoints.Clear();

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
    }

    private Vector3 GetWorldTouchPosition()
    {
        Vector2 screenPoint = GetScreenTouchPosition();
        float cameraZ = Mathf.Abs(mainCamera.transform.position.z);
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, cameraZ));
        worldPoint.z = 0f;
        return worldPoint;
    }

    private Vector2 GetScreenTouchPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }
        return Vector2.zero;
    }

    private bool IsPressBegan()
    {
        bool mouseClick = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool touchStart = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
        return mouseClick || touchStart;
    }

    private bool IsPressHeld()
    {
        bool mouseHeld = Mouse.current != null && Mouse.current.leftButton.isPressed;
        bool touchHeld = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed;
        return mouseHeld || touchHeld;
    }

    private bool IsPressEnded()
    {
        bool mouseUp = Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame;
        bool touchEnd = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame;
        return mouseUp || touchEnd;
    }
}