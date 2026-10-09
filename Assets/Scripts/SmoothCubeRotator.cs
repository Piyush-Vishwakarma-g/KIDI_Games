using UnityEngine;
using UnityEngine.InputSystem;

public class SmoothCubeRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 0.1f;
    [SerializeField] private float damping = 8.0f;

    private Quaternion targetRotation;
    private bool isDraggingThisCube;
    private Vector2 lastPointerPosition;
    private Camera mainCamera;

    private void Start()
    {
        targetRotation = transform.rotation;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Pointer.current == null) return;
        if (mainCamera == null) mainCamera = Camera.main;

        bool pressed = Pointer.current.press.isPressed;
        Vector2 pointerPos = Pointer.current.position.ReadValue();

        // UI drop can miss OnEndDrag; never keep the cube locked after the finger is up.
        if (!pressed)
        {
            isDraggingThisCube = false;
            lastPointerPosition = pointerPos;
            if (GameManager.instance != null && GameManager.instance.isDraggingUI)
                GameManager.instance.EndUiDrag();

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * damping);
            return;
        }

        if (GameManager.instance != null && GameManager.instance.isDraggingUI)
        {
            isDraggingThisCube = false;
            lastPointerPosition = pointerPos;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * damping);
            return;
        }

        if (!isDraggingThisCube && HitsCube(pointerPos))
        {
            isDraggingThisCube = true;
            lastPointerPosition = pointerPos;
        }

        if (isDraggingThisCube)
        {
            Vector2 delta = pointerPos - lastPointerPosition;
            lastPointerPosition = pointerPos;

            if (delta.sqrMagnitude > 0.01f)
            {
                Quaternion xRotation = Quaternion.AngleAxis(delta.y * rotationSpeed, Vector3.right);
                Quaternion yRotation = Quaternion.AngleAxis(-delta.x * rotationSpeed, Vector3.up);
                targetRotation = yRotation * xRotation * targetRotation;
            }
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * damping);
    }

    private bool HitsCube(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
            return false;

        return hit.transform == transform || hit.transform.IsChildOf(transform);
    }
}
