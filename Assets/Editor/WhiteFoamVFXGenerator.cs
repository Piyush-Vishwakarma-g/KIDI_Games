using UnityEngine;
using UnityEditor;

public class WhiteFoamVFXGenerator : EditorWindow
{
    private Vector3 spawnPosition = Vector3.zero;
    private float eruptionHeight = 1.2f;
    private float overflowRadius = 0.65f;
    private bool selectAfterCreate = true;

    [MenuItem("Tools/VFX/White Foam Reaction Generator")]
    public static void OpenWindow()
    {
        GetWindow<WhiteFoamVFXGenerator>("Foam VFX Generator");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("White Foam / Chemical Reaction", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Creates a reusable particle-effect setup in the currently open scene. " +
            "Place the generated root near your flask or experiment table.",
            MessageType.Info);

        spawnPosition = EditorGUILayout.Vector3Field("Root Position", spawnPosition);
        eruptionHeight = EditorGUILayout.Slider("Eruption Height", eruptionHeight, 0.2f, 4f);
        overflowRadius = EditorGUILayout.Slider("Overflow Radius", overflowRadius, 0.2f, 2f);
        selectAfterCreate = EditorGUILayout.Toggle("Select After Create", selectAfterCreate);

        GUILayout.Space(8);

        if (GUILayout.Button("Generate White Foam VFX in Current Scene", GUILayout.Height(34)))
        {
            GenerateVFX();
        }
    }

    private void GenerateVFX()
    {
        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Generate White Foam VFX");

        GameObject root = new GameObject("WhiteFoam_ChemicalReaction_VFX");
        Undo.RegisterCreatedObjectUndo(root, "Create Foam VFX Root");
        root.transform.position = spawnPosition;

        ParticleSystem eruption = CreateParticleSystem(
            root.transform, "Foam_Eruption",
            new Vector3(0f, eruptionHeight, 0f),
            new Color(1f, 1f, 1f, 1f),
            2.0f, 1.1f, 0.10f, 0.28f, 0.15f, 0.8f,
            120f, 1800, ParticleSystemShapeType.Cone, 0.18f, 25f,
            new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0f, 70),
                new ParticleSystem.Burst(0.25f, 35)
            });

        ParticleSystem overflow = CreateParticleSystem(
            root.transform, "Foam_Overflow",
            new Vector3(0f, 0.25f, 0f),
            new Color(1f, 1f, 1f, 0.95f),
            2.8f, 0.55f, 0.12f, 0.42f, 0.2f, 0.25f,
            65f, 2200, ParticleSystemShapeType.Sphere, overflowRadius, 0f,
            new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0f, 100),
                new ParticleSystem.Burst(0.35f, 55)
            });

        ParticleSystem bubbles = CreateParticleSystem(
            root.transform, "Foam_Bubbles",
            new Vector3(0f, eruptionHeight * 0.55f, 0f),
            new Color(1f, 1f, 1f, 0.9f),
            1.8f, 0.75f, 0.04f, 0.12f, 0.05f, 0.45f,
            22f, 900, ParticleSystemShapeType.Sphere, 0.22f, 0f,
            new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0f, 25),
                new ParticleSystem.Burst(0.4f, 20)
            });


ParticleSystem[] allSystems = { eruption, overflow, bubbles };

foreach (ParticleSystem ps in allSystems)
{
    var collision = ps.collision;
    collision.enabled = true;
    collision.type = ParticleSystemCollisionType.World;
    collision.mode = ParticleSystemCollisionMode.Collision3D;
    collision.collidesWith = ~0;
    collision.lifetimeLoss = 1f;
    collision.sendCollisionMessages = true;

    var handler = Undo.AddComponent<FoamParticleCollisionHandler>(
        ps.gameObject
    );

    handler.allFoamSystems = allSystems;
}

        // Keep systems stopped in Edit Mode; they play when the scene enters Play Mode.
        SetPlayOnAwake(eruption, true);
        SetPlayOnAwake(overflow, true);
        SetPlayOnAwake(bubbles, true);

        Undo.CollapseUndoOperations(undoGroup);

        if (selectAfterCreate)
        {
            Selection.activeGameObject = root;
            EditorGUIUtility.PingObject(root);
        }

        Debug.Log("White Foam VFX created. Move the root to your flask and press Play to preview.");
    }

    private static ParticleSystem CreateParticleSystem(
        Transform parent,
        string objectName,
        Vector3 localPosition,
        Color startColor,
        float lifetime,
        float speed,
        float minSize,
        float maxSize,
        float gravity,
        float upwardVelocity,
        float emissionRate,
        int maxParticles,
        ParticleSystemShapeType shapeType,
        float shapeRadius,
        float coneAngle,
        ParticleSystem.Burst[] bursts)
    {
        GameObject go = new GameObject(objectName);
        Undo.RegisterCreatedObjectUndo(go, "Create " + objectName);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPosition;

        ParticleSystem ps = Undo.AddComponent<ParticleSystem>(go);
        var main = ps.main;
        main.duration = 3f;
        main.loop = true;
        main.playOnAwake = false;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.maxParticles = maxParticles;
        main.startLifetime = new ParticleSystem.MinMaxCurve(lifetime * 0.65f, lifetime);
        main.startSpeed = new ParticleSystem.MinMaxCurve(speed * 0.6f, speed);
        main.startSize = new ParticleSystem.MinMaxCurve(minSize, maxSize);
        main.startColor = startColor;
        main.gravityModifier = gravity;

        var emission = ps.emission;
        emission.enabled = true;
        emission.rateOverTime = emissionRate;
        emission.SetBursts(bursts);

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = shapeType;
        shape.radius = shapeRadius;
        if (shapeType == ParticleSystemShapeType.Cone)
        {
            shape.angle = coneAngle;
            shape.arc = 360f;
        }

        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;
        velocity.x = new ParticleSystem.MinMaxCurve(-0.18f, 0.18f);
        velocity.y = new ParticleSystem.MinMaxCurve(upwardVelocity * 0.45f, upwardVelocity);
        velocity.z = new ParticleSystem.MinMaxCurve(-0.18f, 0.18f);

        var colorLifetime = ps.colorOverLifetime;
        colorLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(new Color(0.94f, 0.98f, 1f), 0.7f),
                new GradientColorKey(Color.white, 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(1f, 0.12f),
                new GradientAlphaKey(0.85f, 0.72f),
                new GradientAlphaKey(0f, 1f)
            });
        colorLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

        var sizeLifetime = ps.sizeOverLifetime;
        sizeLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve(
            new Keyframe(0f, 0.35f),
            new Keyframe(0.25f, 1f),
            new Keyframe(0.72f, 0.8f),
            new Keyframe(1f, 0.05f));
        sizeLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.alignment = ParticleSystemRenderSpace.View;
        renderer.material = GetFoamMaterial();

        // Enable collision so particles can hit the ground.
var collision = ps.collision;
collision.enabled = true;
collision.type = ParticleSystemCollisionType.World;
collision.mode = ParticleSystemCollisionMode.Collision3D;
collision.collidesWith = ~0;

// Remove particles as soon as they collide.
collision.lifetimeLoss = 1f;
collision.sendCollisionMessages = false;
        return ps;
    }

    private static void SetPlayOnAwake(ParticleSystem ps, bool value)
    {
        var main = ps.main;
        main.playOnAwake = value;
    }

    private static Material cachedFoamMaterial;

    private static Material GetFoamMaterial()
    {
        if (cachedFoamMaterial != null)
            return cachedFoamMaterial;

        const string folder = "Assets/GeneratedVFX";
        const string materialPath = folder + "/WhiteFoamMaterial.mat";

        Material existing = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (existing != null)
        {
            cachedFoamMaterial = existing;
            return cachedFoamMaterial;
        }

        if (!AssetDatabase.IsValidFolder(folder))
            AssetDatabase.CreateFolder("Assets", "GeneratedVFX");

        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (shader == null) shader = Shader.Find("Particles/Standard Unlit");
        if (shader == null) shader = Shader.Find("Legacy Shaders/Particles/Alpha Blended");
        if (shader == null) shader = Shader.Find("Sprites/Default");

        if (shader == null)
        {
            Debug.LogWarning("Could not find a particle shader. Unity will use the default Particle System material.");
            return null;
        }

        Material material = new Material(shader);
        material.name = "WhiteFoamMaterial";
        material.color = Color.white;
        AssetDatabase.CreateAsset(material, materialPath);
        AssetDatabase.SaveAssets();

        cachedFoamMaterial = material;
        return cachedFoamMaterial;
    }
}
