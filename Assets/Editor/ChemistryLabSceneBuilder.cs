#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChemistryLabSceneBuilder : EditorWindow
{
    [MenuItem("Chemistry Lab/Generate Full Project Scene")]
    public static void GenerateScene()
    {
        // 1. Setup Main Camera & Directional Light
        SetupLightingAndCamera();

        // 2. Setup EventSystem for UI
        SetupEventSystem();

        // 3. Create Managers
        GameObject managersObj = new GameObject("--- MANAGERS ---");
        SafetyManager safetyManager = managersObj.AddComponent<SafetyManager>();
        ReactionController reactionController = managersObj.AddComponent<ReactionController>();

        // 4. Create Laboratory Table (3D World Environment)
        GameObject table = GameObject.CreatePrimitive(PrimitiveType.Cube);
        table.name = "Lab_Table";
        table.transform.position = new Vector3(0, -0.5f, 0);
        table.transform.localScale = new Vector3(6, 0.2f, 3);
        SetMaterialColor(table, new Color(0.15f, 0.15f, 0.18f)); // Dark workbench

        // 5. Create Containers
        // Central Erlenmeyer Flask
        GameObject flask = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        flask.name = "Erlenmeyer_Flask";
        flask.transform.position = new Vector3(0, 0.25f, 0);
        flask.transform.localScale = new Vector3(0.8f, 0.5f, 0.8f);
        SetMaterialColor(flask, new Color(0.8f, 0.9f, 1f, 0.4f), true); // Transparent glass
        ChemicalReaction chemicalReaction = flask.AddComponent<ChemicalReaction>();

        // Flask Foam Particle System Setup
        GameObject foamObj = new GameObject("Foam_Particle_System");
        foamObj.transform.SetParent(flask.transform);
        foamObj.transform.localPosition = new Vector3(0, 1.0f, 0);
        ParticleSystem ps = foamObj.AddComponent<ParticleSystem>();
        ConfigureFoamParticleSystem(ps);
        chemicalReaction.foamParticles = ps;

        // Beaker (Vinegar)
        GameObject beaker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        beaker.name = "Beaker_Vinegar";
        beaker.transform.position = new Vector3(1.8f, 0.25f, 0);
        beaker.transform.localScale = new Vector3(0.5f, 0.4f, 0.5f);
        SetMaterialColor(beaker, new Color(0.9f, 0.95f, 1f, 0.5f), true);
        DraggableContainer dragBeaker = beaker.AddComponent<DraggableContainer>();
        dragBeaker.chemicalType = DraggableContainer.ChemicalType.Vinegar;
        dragBeaker.flaskTarget = flask.transform;

        // Watch Glass (Baking Soda)
        GameObject watchGlass = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        watchGlass.name = "WatchGlass_Soda";
        watchGlass.transform.position = new Vector3(-1.8f, -0.1f, 0);
        watchGlass.transform.localScale = new Vector3(0.6f, 0.05f, 0.6f);
        SetMaterialColor(watchGlass, new Color(1f, 1f, 1f, 0.8f), true);
        DraggableContainer dragGlass = watchGlass.AddComponent<DraggableContainer>();
        dragGlass.chemicalType = DraggableContainer.ChemicalType.BakingSoda;
        dragGlass.flaskTarget = flask.transform;

        // 6. Setup Safety UI System (Canvas)
        SetupSafetyUI(safetyManager);

        Debug.Log(" Chemistry Lab Scene successfully generated with all elements, scripts, and links!");
    }

    private static void SetupLightingAndCamera()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            mainCam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
        }
        mainCam.transform.position = new Vector3(0, 1.2f, -3.5f);
        mainCam.transform.rotation = Quaternion.Euler(15f, 0, 0);
        mainCam.backgroundColor = new Color(0.08f, 0.09f, 0.12f);

        Light dirLight = Object.FindFirstObjectByType<Light>();
        if (dirLight == null)
        {
            GameObject lightObj = new GameObject("Directional Light");
            dirLight = lightObj.AddComponent<Light>();
            dirLight.type = LightType.Directional;
        }
        dirLight.transform.rotation = Quaternion.Euler(50f, -30f, 0);
        dirLight.intensity = 1.2f;
    }

    private static void SetupEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
    }

    private static void SetupSafetyUI(SafetyManager safetyManager)
    {
        // Create UI Canvas
        GameObject canvasObj = new GameObject("Safety_UI_Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Character Container UI
        GameObject charObj = new GameObject("Character_Avatar", typeof(RectTransform), typeof(Image));
        charObj.transform.SetParent(canvasObj.transform, false);
        RectTransform charRect = charObj.GetComponent<RectTransform>();
        charRect.anchoredPosition = new Vector3(-350, -50, 0);
        charRect.sizeDelta = new Vector2(250, 350);
        charObj.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 0.5f); // Placeholder Character

        // Drop Zone: Face (Goggles)
        GameObject faceZone = new GameObject("GogglesDropZone", typeof(RectTransform), typeof(Image), typeof(SafetyDropZone));
        faceZone.transform.SetParent(charObj.transform, false);
        RectTransform faceRect = faceZone.GetComponent<RectTransform>();
        faceRect.anchoredPosition = new Vector3(0, 100, 0);
        faceRect.sizeDelta = new Vector2(120, 60);
        faceZone.GetComponent<Image>().color = new Color(0f, 1f, 0f, 0.3f); // Green target hint
        faceZone.GetComponent<SafetyDropZone>().requiredType = SafetyDropZone.EquipmentType.Goggles;

        // Drop Zone: Hands (Gloves)
        GameObject handZone = new GameObject("GlovesDropZone", typeof(RectTransform), typeof(Image), typeof(SafetyDropZone));
        handZone.transform.SetParent(charObj.transform, false);
        RectTransform handRect = handZone.GetComponent<RectTransform>();
        handRect.anchoredPosition = new Vector3(0, -60, 0);
        handRect.sizeDelta = new Vector2(160, 60);
        handZone.GetComponent<Image>().color = new Color(0f, 1f, 0f, 0.3f); // Green target hint
        handZone.GetComponent<SafetyDropZone>().requiredType = SafetyDropZone.EquipmentType.Gloves;

        // Side Panel for Equipment Selection
        GameObject panelObj = new GameObject("Equipment_Panel", typeof(RectTransform), typeof(Image));
        panelObj.transform.SetParent(canvasObj.transform, false);
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchoredPosition = new Vector3(400, 0, 0);
        panelRect.sizeDelta = new Vector2(180, 300);
        panelObj.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.15f, 0.8f);

        // Draggable Goggles Item
        GameObject gogglesItem = new GameObject("UI_Goggles", typeof(RectTransform), typeof(Image), typeof(CanvasGroup), typeof(DraggableSafetyItem));
        gogglesItem.transform.SetParent(panelObj.transform, false);
        gogglesItem.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 60, 0);
        gogglesItem.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 50);
        gogglesItem.GetComponent<Image>().color = Color.cyan;
        gogglesItem.GetComponent<DraggableSafetyItem>().itemType = DraggableSafetyItem.ItemType.Goggles;

        // Draggable Gloves Item
        GameObject glovesItem = new GameObject("UI_Gloves", typeof(RectTransform), typeof(Image), typeof(CanvasGroup), typeof(DraggableSafetyItem));
        glovesItem.transform.SetParent(panelObj.transform, false);
        glovesItem.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -60, 0);
        glovesItem.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 50);
        glovesItem.GetComponent<Image>().color = Color.yellow;
        glovesItem.GetComponent<DraggableSafetyItem>().itemType = DraggableSafetyItem.ItemType.Gloves;
    }

    private static void ConfigureFoamParticleSystem(ParticleSystem ps)
    {
        var main = ps.main;
        main.startColor = Color.white;
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(1.0f, 2.0f);
        main.playOnAwake = false;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 8f;
        shape.radius = 0.1f;
    }

    private static void SetMaterialColor(GameObject obj, Color color, bool transparent = false)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = color;
            if (transparent)
            {
                mat.SetFloat("_Mode", 3); // Transparent mode
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
            }
            renderer.sharedMaterial = mat;
        }
    }
}
#endif