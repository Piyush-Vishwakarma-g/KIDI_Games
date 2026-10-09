using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PathPlayer : MonoBehaviour
{
    [Header("Movement")]
    public float moveSmooth = 22f;

    [Header("Path Validation")]
    public float validRadius = 0.33f;

    [Header("Maximum Movement Per Frame")]
    public float maxJumpDistance = 0.65f;

    [Header("Mouse Testing")]
    public bool allowMouseInEditor = true;

    private Camera cam;

    private TraversePathGame game;

    private List<Vector3> path;

    private bool dragging;

    private Vector3 dragOffset;

    private Vector3 lastValidPosition;

    private int activeFingerId = -1;


    // =========================================================
    // INITIALIZATION
    // =========================================================

    public void Initialize(
        Camera cameraRef,
        TraversePathGame owner,
        List<Vector3> solutionPath)
    {
        cam = cameraRef;
        game = owner;
        path = solutionPath;

        lastValidPosition = transform.position;
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (game == null)
            return;

        if (game.IsGameOver)
            return;

        HandleTouchInput();

        HandleMouseInput();

        transform.position = Vector3.Lerp(
            transform.position,
            lastValidPosition,
            moveSmooth * Time.deltaTime
        );
    }


    // =========================================================
    // TOUCH INPUT - NEW INPUT SYSTEM
    // =========================================================

    private void HandleTouchInput()
    {
        if (Touchscreen.current == null)
            return;

        var touches = Touchscreen.current.touches;

        foreach (var touch in touches)
        {
            int fingerId =
                touch.touchId.ReadValue();

            UnityEngine.InputSystem.TouchPhase phase =
                touch.phase.ReadValue();

            Vector2 screenPosition =
                touch.position.ReadValue();


            // ---------------------------------------------
            // TOUCH START
            // ---------------------------------------------

            if (phase ==
                UnityEngine.InputSystem.TouchPhase.Began)
            {
                if (activeFingerId != -1)
                    continue;

                activeFingerId = fingerId;

                HandlePointer(
                    screenPosition,
                    true,
                    false,
                    false
                );
            }


            // ---------------------------------------------
            // TOUCH MOVE
            // ---------------------------------------------

            if (fingerId == activeFingerId)
            {
                if (phase ==
                        UnityEngine.InputSystem.TouchPhase.Moved ||
                    phase ==
                        UnityEngine.InputSystem.TouchPhase.Stationary)
                {
                    HandlePointer(
                        screenPosition,
                        false,
                        true,
                        false
                    );
                }


                // -----------------------------------------
                // TOUCH END
                // -----------------------------------------

                if (phase ==
                        UnityEngine.InputSystem.TouchPhase.Ended ||
                    phase ==
                        UnityEngine.InputSystem.TouchPhase.Canceled)
                {
                    HandlePointer(
                        screenPosition,
                        false,
                        false,
                        true
                    );

                    activeFingerId = -1;
                }
            }
        }
    }


    // =========================================================
    // MOUSE INPUT - NEW INPUT SYSTEM
    // =========================================================

    private void HandleMouseInput()
    {
        if (!allowMouseInEditor)
            return;

        if (Mouse.current == null)
            return;

        Vector2 mousePosition =
            Mouse.current.position.ReadValue();


        // Mouse Down
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandlePointer(
                mousePosition,
                true,
                false,
                false
            );
        }


        // Mouse Hold / Drag
        if (Mouse.current.leftButton.isPressed)
        {
            HandlePointer(
                mousePosition,
                false,
                true,
                false
            );
        }


        // Mouse Up
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            HandlePointer(
                mousePosition,
                false,
                false,
                true
            );
        }
    }


    // =========================================================
    // POINTER HANDLING
    // =========================================================

    private void HandlePointer(
        Vector2 screenPosition,
        bool began,
        bool moved,
        bool ended)
    {
        if (cam == null)
            cam = Camera.main;

        if (cam == null)
            return;


        Vector3 world =
            cam.ScreenToWorldPoint(
                new Vector3(
                    screenPosition.x,
                    screenPosition.y,
                    Mathf.Abs(
                        cam.transform.position.z -
                        transform.position.z
                    )
                )
            );

        world.z = transform.position.z;


        // =====================================================
        // BEGIN DRAG
        // =====================================================

        if (began)
        {
            float distance =
                Vector2.Distance(
                    world,
                    transform.position
                );

            if (distance <= 0.75f)
            {
                dragging = true;

                dragOffset =
                    transform.position - world;
            }
        }


        // =====================================================
        // MOVE
        // =====================================================

        if (moved && dragging)
        {
            Vector3 desired =
                world + dragOffset;


            // ---------------------------------------------
            // LIMIT LARGE JUMPS
            // ---------------------------------------------

            float jump =
                Vector3.Distance(
                    desired,
                    transform.position
                );

            if (jump > maxJumpDistance)
            {
                desired =
                    Vector3.MoveTowards(
                        transform.position,
                        desired,
                        maxJumpDistance
                    );
            }


            // ---------------------------------------------
            // CHECK PATH
            // ---------------------------------------------

            bool valid =
                IsNearSolution(
                    desired,
                    out float progress
                );


            // ---------------------------------------------
            // WRONG PATH
            // ---------------------------------------------

            if (!valid)
            {
                game.WrongPath(desired);

                dragging = false;

                return;
            }


            // ---------------------------------------------
            // VALID PATH
            // ---------------------------------------------

            lastValidPosition = desired;

            transform.position =
                Vector3.Lerp(
                    transform.position,
                    desired,
                    moveSmooth * Time.deltaTime
                );


            game.SetProgress(progress);


            // ---------------------------------------------
            // FINISH
            // ---------------------------------------------

            if (progress >= 0.995f)
            {
                dragging = false;

                game.CompleteLevel();
            }
        }


        // =====================================================
        // END DRAG
        // =====================================================

        if (ended)
        {
            dragging = false;
        }
    }


    // =========================================================
    // PATH VALIDATION
    // =========================================================

    private bool IsNearSolution(
        Vector3 position,
        out float progress)
    {
        progress = 0f;

        if (path == null)
            return false;

        if (path.Count < 2)
            return false;


        float closestDistance =
            float.MaxValue;

        float closestProgress = 0f;

        float totalLength = 0f;


        // -----------------------------------------------------
        // TOTAL PATH LENGTH
        // -----------------------------------------------------

        for (int i = 0; i < path.Count - 1; i++)
        {
            totalLength +=
                Vector3.Distance(
                    path[i],
                    path[i + 1]
                );
        }


        // -----------------------------------------------------
        // FIND CLOSEST PATH POINT
        // -----------------------------------------------------

        float accumulated = 0f;


        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 a = path[i];

            Vector3 b = path[i + 1];

            Vector3 ab = b - a;

            float lengthSquared =
                ab.sqrMagnitude;


            float t = 0f;


            if (lengthSquared > 0.0001f)
            {
                t =
                    Mathf.Clamp01(
                        Vector3.Dot(
                            position - a,
                            ab
                        ) / lengthSquared
                    );
            }


            Vector3 closest =
                a + ab * t;


            float distance =
                Vector2.Distance(
                    position,
                    closest
                );


            if (distance < closestDistance)
            {
                closestDistance = distance;


                float segmentLength =
                    Vector3.Distance(
                        a,
                        b
                    );


                closestProgress =
                    (
                        accumulated +
                        segmentLength * t
                    )
                    /
                    Mathf.Max(
                        0.001f,
                        totalLength
                    );
            }


            accumulated +=
                Vector3.Distance(
                    a,
                    b
                );
        }


        progress =
            closestProgress;


        return
            closestDistance <= validRadius;
    }


    // =========================================================
    // RESET PLAYER
    // =========================================================

    public void ResetPlayer(
        Vector3 startPosition)
    {
        transform.position =
            startPosition;

        lastValidPosition =
            startPosition;

        dragging = false;

        activeFingerId = -1;
    }


    // =========================================================
    // FORCE STOP
    // =========================================================

    public void StopDragging()
    {
        dragging = false;

        activeFingerId = -1;
    }
}