using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Practical cross-platform 3D granular simulation using Unity PhysX for collisions
/// and GPU-instanced MeshRenderers for rendering where Unity's renderer can batch them.
/// Create a Ground and container walls with colliders, then add this component to an empty object.
/// </summary>
public class Granular3DController : MonoBehaviour
{
    public enum GrainType { Sand, Salt, Sugar }

    [Header("References")]
    public Camera simulationCamera;
    public Transform pourPoint;
    public Material sandMaterial;
    public Material saltMaterial;
    public Material sugarMaterial;

    [Header("Simulation")]
    [Range(50, 2500)] public int maxGrains = 700;
    [Range(1, 100)] public int grainsPerPour = 18;
    [Min(0.01f)] public float grainDiameter = 0.075f;
    [Min(0.1f)] public float pourRadius = 0.22f;
    [Min(0f)] public float pourSpeed = 1.2f;
    [Min(0f)] public float initialDownwardSpeed = 0.4f;
    public GrainType selectedMaterial = GrainType.Sand;
    public bool holdToPour = false;

    [Header("Touch / mouse dragging")]
    [Min(0.1f)] public float dragDepth = 4f;
    [Min(0.1f)] public float dragMoveSpeed = 25f;
    public LayerMask grainLayerMask = ~0;

    [Header("Physics")]
    [Range(0f, 1f)] public float friction = 0.65f;
    [Range(0f, 1f)] public float bounciness = 0.02f;
    [Min(0.01f)] public float grainMass = 0.02f;

    private readonly List<GameObject> grains = new List<GameObject>();
    private readonly List<Rigidbody> rigidbodies = new List<Rigidbody>();
    private PhysicsMaterial grainPhysicsMaterial;
    private GameObject draggedObject;
    private Rigidbody draggedBody;
    private float grabbedDepth;
    private Vector3 dragTarget;
    private bool previousPointerDown;

    public int GrainCount => grains.Count;

    private void Awake()
    {
        if (simulationCamera == null) simulationCamera = Camera.main;
        if (pourPoint == null) pourPoint = transform;
        CreatePhysicsMaterial();
        EnableGpuInstancing(sandMaterial);
        EnableGpuInstancing(saltMaterial);
        EnableGpuInstancing(sugarMaterial);
    }

   
private void Start()
{
    Debug.Log("Granular3DController STARTED");
    Pour(grainsPerPour);
}

private void Update()
{
    if (Input.GetKeyDown(KeyCode.Space))
    {
        Debug.Log("SPACE PRESSED");
        Pour(grainsPerPour);
    }
}


    private void FixedUpdate()
    {
        if (draggedBody != null && draggedObject != null)
        {
            Vector3 next = Vector3.MoveTowards(
                draggedBody.position, dragTarget, dragMoveSpeed * Time.fixedDeltaTime);
            draggedBody.MovePosition(next);
        }
    }

    private void CreatePhysicsMaterial()
    {
        grainPhysicsMaterial = new PhysicsMaterial("GranularGrainPhysics")
{
    dynamicFriction = friction,
    staticFriction = friction,
    bounciness = bounciness,
    frictionCombine = PhysicsMaterialCombine.Average,
    bounceCombine = PhysicsMaterialCombine.Minimum
};
    }

    private static void EnableGpuInstancing(Material material)
    {
        if (material != null) material.enableInstancing = true;
    }

    public void SelectSand() { selectedMaterial = GrainType.Sand; }
    public void SelectSalt() { selectedMaterial = GrainType.Salt; }
    public void SelectSugar() { selectedMaterial = GrainType.Sugar; }

    // Hook this method to a UI Button's OnClick event.
    public void PourButton() { Pour(grainsPerPour); }

    public void Pour(int count)
    {
        if (pourPoint == null) return;
        for (int i = 0; i < count && grains.Count < maxGrains; i++)
        {
            Vector2 disk = Random.insideUnitCircle * pourRadius;
            Vector3 position = pourPoint.position
                + pourPoint.right * disk.x
                + pourPoint.forward * disk.y
                + Vector3.up * Random.Range(0f, pourRadius);

            GameObject grain = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            grain.name = selectedMaterial + "_Grain";
            grain.transform.position = position;
            float diameter = grainDiameter * Random.Range(0.82f, 1.18f);
            grain.transform.localScale = Vector3.one * diameter;

            Renderer renderer = grain.GetComponent<Renderer>();
            Material chosen = GetMaterial(selectedMaterial);
            if (chosen != null)
            {
                chosen.enableInstancing = true;
                renderer.sharedMaterial = chosen;
            }

            Collider collider = grain.GetComponent<Collider>();
            collider.sharedMaterial = grainPhysicsMaterial;

            Rigidbody body = grain.AddComponent<Rigidbody>();
            body.mass = grainMass;
            body.linearDamping = 0.02f;
            body.angularDamping = 0.05f;
            body.useGravity = true;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.Discrete;
            body.maxAngularVelocity = 20f;
            body.linearVelocity = -pourPoint.up * initialDownwardSpeed
                + pourPoint.forward * Random.Range(-0.15f, 0.15f);

            grains.Add(grain);
            rigidbodies.Add(body);
        }
    }

    private Material GetMaterial(GrainType type)
    {
        switch (type)
        {
            case GrainType.Salt: return saltMaterial;
            case GrainType.Sugar: return sugarMaterial;
            default: return sandMaterial;
        }
    }

    private void HandlePointer()
    {
        bool down = false;
        bool pressed = false;
        bool released = false;
        Vector2 screenPosition = Vector2.zero;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            screenPosition = touch.position;
            down = touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled;
            pressed = touch.phase == TouchPhase.Began;
            released = touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled;
        }
        else
        {
            screenPosition = Input.mousePosition;
            down = Input.GetMouseButton(0);
            pressed = Input.GetMouseButtonDown(0);
            released = Input.GetMouseButtonUp(0);
        }

        if (simulationCamera == null) return;

        if (pressed && draggedObject == null)
        {
            Ray ray = simulationCamera.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, grainLayerMask))
            {
                Rigidbody body = hit.rigidbody;
                if (body != null && hit.collider.GetComponent<Renderer>() != null)
                {
                    draggedObject = body.gameObject;
                    draggedBody = body;
                    grabbedDepth = Vector3.Dot(
                        draggedObject.transform.position - simulationCamera.transform.position,
                        simulationCamera.transform.forward);
                    dragTarget = draggedObject.transform.position;
                    draggedBody.linearVelocity = Vector3.zero;
                    draggedBody.angularVelocity = Vector3.zero;
                    draggedBody.isKinematic = true;
                }
            }
        }

        if (down && draggedObject != null)
        {
            Ray ray = simulationCamera.ScreenPointToRay(screenPosition);
            dragTarget = ray.GetPoint(Mathf.Max(0.1f, grabbedDepth));
        }

        if (released || (!down && previousPointerDown))
            ReleaseDraggedGrain();

        previousPointerDown = down;
    }

    private bool PointerIsDown()
    {
        if (Input.touchCount > 0)
        {
            TouchPhase phase = Input.GetTouch(0).phase;
            return phase != TouchPhase.Ended && phase != TouchPhase.Canceled;
        }
        return Input.GetMouseButton(0);
    }

    private void ReleaseDraggedGrain()
    {
        if (draggedBody != null)
        {
            draggedBody.isKinematic = false;
            draggedBody.WakeUp();
        }
        draggedBody = null;
        draggedObject = null;
    }

    public void ClearSimulation()
    {
        ReleaseDraggedGrain();
        for (int i = 0; i < grains.Count; i++)
        {
            if (grains[i] != null) Destroy(grains[i]);
        }
        grains.Clear();
        rigidbodies.Clear();
    }

    private void OnDestroy()
    {
        if (grainPhysicsMaterial != null) Destroy(grainPhysicsMaterial);
        ReleaseDraggedGrain();
    }
}
